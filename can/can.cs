using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peak.Can.Basic;

namespace can
{
    using TPCANHandle = System.UInt16;

    public enum SupportedVendor : byte
    {
        PEAK,
    }

    public class CanRxMsgArgs : EventArgs
    {
        public readonly UInt32 msgId;
        public readonly byte msgType;
        public readonly byte len;
        public readonly byte[] data;
        public readonly UInt64 timestamp;
        public CanRxMsgArgs(UInt32 msgId, byte msgType, byte[] data, UInt64 timestamp)
        {
            this.msgId = msgId;
            this.msgType = msgType;
            if (data != null) {
                this.len = (byte)data.Length;
            } else {
                this.len = 0;
            }
            if (this.len > 0) {
                this.data = new byte[this.len];
                data.CopyTo(this.data, 0);
            }
            this.timestamp = timestamp;
        }
    }

    public delegate void CanRxMsgHandler(object sender, CanRxMsgArgs e);
    public delegate void CanTxHook(object sender, CanRxMsgArgs e);

    public class can_peak
    {
        #region Members
        private TPCANHandle m_PcanHandle;
        private TPCANBaudrate m_Baudrate;
        private bool bConnected = false;
        public event CanRxMsgHandler CanRxMsgEvent;
        public event CanTxHook CanTxHookEvent;
        private System.Threading.AutoResetEvent m_ReceiveEvent;
        private System.Threading.Thread m_ReadThread;
        #endregion

        #region Methods

        #region Event Handlers

        #region Constructor/Destructor
        public can_peak()
        {
            this.m_PcanHandle = 0x00;
            this.m_Baudrate = TPCANBaudrate.PCAN_BAUD_250K;
            this.m_ReadThread = null;
        }

        ~can_peak()
        {
            if (this.m_PcanHandle != 0x00) {
                PCANBasic.Uninitialize(this.m_PcanHandle);
            }
            if (this.m_ReadThread != null) {
                this.m_ReadThread.Abort();
                this.m_ReadThread.Join();
                this.m_ReadThread = null;
            }
        }
        #endregion // Constructor/Destructor

        #region Message Handler
        private void CANReadThreadFunc()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            iBuffer = Convert.ToUInt32(m_ReceiveEvent.SafeWaitHandle.DangerousGetHandle().ToInt32());
            // Sets the handle of the Receive-Event.
            //
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK) {
                Console.WriteLine("pcan::CANReadThreadFunc Error: " + stsResult);
                return;
            }

            // While this mode is selected
            while (true) //(rdbEvent.Checked)
            {
                // Waiting for Receive-Event
                // 
                if (m_ReceiveEvent.WaitOne(50)) {
                    TPCANMsg CANMsg;
                    TPCANTimestamp CANTimeStamp;
                    do {
                        stsResult = PCANBasic.Read(this.m_PcanHandle, out CANMsg, out CANTimeStamp);
                        if (stsResult == TPCANStatus.PCAN_ERROR_OK) {
                            if (this.CanRxMsgEvent != null) {
                                UInt32 msgId = CANMsg.ID;
                                byte msgType = (byte)CANMsg.MSGTYPE;
                                byte[] data = null;
                                if (CANMsg.LEN > 0) {
                                    data = new byte[CANMsg.LEN];
                                    Array.Copy(CANMsg.DATA, 0, data, 0, data.Length);

                                    UInt64 timestamp_us = (UInt64)( CANTimeStamp.micros ) +
                                                    ( (UInt64)( CANTimeStamp.millis ) * 1000 ) +
                                                    ( (UInt64)( CANTimeStamp.millis_overflow ) * ( 2 ^ 32 ) );

                                    this.CanRxMsgEvent(this, new CanRxMsgArgs(msgId, msgType, data, timestamp_us));
                                }
                            }
                        }
                    } while (this.bConnected && ( !Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY) ));
                }
            }
        }
        #endregion // Message Handler

        #endregion // Event Handlers

        public bool Connect(TPCANHandle pcanHandle, TPCANBaudrate baudrate)
        {
            TPCANStatus stsResult;

            if (( pcanHandle < PCANBasic.PCAN_USBBUS1 ) ||
               ( pcanHandle > PCANBasic.PCAN_USBBUS8 )) {
                Console.WriteLine("Only PCAN USB is supported!");
                return false;
            }

            this.m_PcanHandle = pcanHandle;
            this.m_Baudrate = baudrate;

            stsResult = PCANBasic.Initialize(this.m_PcanHandle, this.m_Baudrate);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK) {
                Console.WriteLine("An error occurred: " + stsResult);
                return false;
            }

            /* Create thread for reading incoming CAN messages */
            this.m_ReceiveEvent = new System.Threading.AutoResetEvent(false);
            System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(this.CANReadThreadFunc);
            if (this.m_ReadThread != null) {
                /* Destroy previous thread instance */
                this.m_ReadThread.Abort();
                this.m_ReadThread.Join();
                this.m_ReadThread = null;
            }
            this.m_ReadThread = new System.Threading.Thread(threadDelegate);
            this.m_ReadThread.Name = "pcan_usb worker";
            this.m_ReadThread.IsBackground = true;
            this.m_ReadThread.Start();

            this.bConnected = true;

            return true;
        }

        public void Disconnect()
        {
            if (this.m_PcanHandle != 0x00) {
                PCANBasic.Uninitialize(this.m_PcanHandle);
                this.m_PcanHandle = 0x00;
            }
            if (this.m_ReadThread != null) {
                this.m_ReadThread.Abort();
                this.m_ReadThread.Join();
                this.m_ReadThread = null;
            }

            this.bConnected = false;
        }

        public bool SendStandard(UInt32 can_id, byte[] data)
        {
            if (this.bConnected) {
                TPCANStatus stsResult;
                if (this.CanTxHookEvent != null) {
                    this.CanTxHookEvent(this, new CanRxMsgArgs(can_id, 0, data, 0));
                }
                TPCANMsg CANMsg = new TPCANMsg();
                CANMsg.ID = can_id;
                CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                CANMsg.LEN = Convert.ToByte(data.Length);
                CANMsg.DATA = new byte[8];
                Array.Copy(data, 0, CANMsg.DATA, 0, data.Length);
                stsResult = PCANBasic.Write(this.m_PcanHandle, ref CANMsg);
                if (stsResult == TPCANStatus.PCAN_ERROR_OK) {
                    return true;
                } else {
                    Console.WriteLine("pcan::SendStandard error: " + stsResult);
                    return false;
                }
            } else {
                Console.WriteLine("pcan::SendStandard error: Not Connected");
                return false;
            }
        }

        #endregion // Methods
    }
}
