using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Peak.Can.Basic;
using can;

namespace FirmwareLoader
{
    using TPCANHandle = System.UInt16;

    public partial class FirmwareLoader : Form
    {
        #region Members
        private TPCANHandle pcanHandle;
        private TPCANBaudrate m_baudrate;
        private TPCANHandle[] m_HandlesArray;
        private can_peak can_Peak = null;
        #endregion // Members

        #region Methods
        public FirmwareLoader()
        {
            InitializeComponent();

            this.can_Peak = new can_peak();

            this.m_HandlesArray = new TPCANHandle[]
            {
                PCANBasic.PCAN_USBBUS1,
                PCANBasic.PCAN_USBBUS2,
                PCANBasic.PCAN_USBBUS3,
                PCANBasic.PCAN_USBBUS4,
                PCANBasic.PCAN_USBBUS5,
                PCANBasic.PCAN_USBBUS6,
                PCANBasic.PCAN_USBBUS7,
                PCANBasic.PCAN_USBBUS8,
            };

            this.set_obj_states(false);
            this.btn_HwRefresh_Click(this, null);
        }

        #region Helpers
        private void set_obj_states(bool connected)
        {
            this.cbb_channel.Enabled = !connected;
            this.cbb_baudrates.Enabled = !connected;
            this.btn_HwRefresh.Enabled = !connected;
            this.textBox_NodeID.Enabled = !connected;
        }
        #endregion

        #region Message Handlers
        private void CanRxHandler(object sender, CanRxMsgArgs e)
        {
            DateTime timestamp = DateTime.Now;
            var ticks = timestamp.Ticks;

            string rawStr = e.msgId.ToString("X4") + "h :";
            for (int i = 0; i < e.data.Length; i++) {
                rawStr += " " + e.data[i].ToString("X2") + "h";
            }
            rawStr += Environment.NewLine;

            string logStr = ticks.ToString() + " rx " + e.msgId.ToString("X4");
            for (int i = 0; i < e.data.Length; i++) {
                logStr += " " + e.data[i].ToString("X2");
            }
            logStr += Environment.NewLine;

            Console.WriteLine(logStr);
        }

        void CanTxHookHandler(object sender, CanRxMsgArgs e)
        {
            DateTime timestamp = DateTime.Now;
            var ticks = timestamp.Ticks;

            string logStr = ticks.ToString() + " tx " + e.msgId.ToString("X4");
            if (e.data != null) {
                for (int i = 0; i < e.data.Length; i++) {
                    logStr += " " + e.data[i].ToString("X2");
                }
            }
            logStr += Environment.NewLine;

            Console.WriteLine(logStr);
        }
        #endregion // Message Handlers

        #region Button Event Handlers
        private void btn_HwRefresh_Click(object sender, EventArgs e)
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            // Clears the Channel combioBox and fill it again with 
            // the PCAN-Basic handles for no-Plug&Play hardware and
            // the detected Plug&Play hardware
            cbb_channel.Items.Clear();
            cbb_baudrates.Items.Clear();
            try {
                for (int i = 0; i < m_HandlesArray.Length; i++) {
                    // Includes all no-Plug&Play Handles
                    if (( m_HandlesArray[i] >= PCANBasic.PCAN_USBBUS1 ) &&
                        ( m_HandlesArray[i] <= PCANBasic.PCAN_USBBUS8 )) {
                        stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_CONDITION, out iBuffer, sizeof(UInt32));
                        if (( stsResult == TPCANStatus.PCAN_ERROR_OK ) && ( iBuffer == PCANBasic.PCAN_CHANNEL_AVAILABLE )) {
                            cbb_channel.Items.Add(string.Format("PCAN-USB{0}", m_HandlesArray[i] & 0x0F));
                        }
                    }
                }
                cbb_channel.SelectedIndex = cbb_channel.Items.Count - 1;
            } catch (DllNotFoundException) {
                MessageBox.Show("Unable to find the library: PCANBasic.dll !", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }
            cbb_baudrates.Items.Clear();
            string[] baudName = Enum.GetNames(typeof(TPCANBaudrate));
            for (int i = 0; i < baudName.Length; i++) {
                cbb_baudrates.Items.Add(baudName[i]);
            }
            cbb_baudrates.SelectedIndex = 0;
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try {
                if (this.btn_Connect.Text == "Connect") {
                    if (this.can_Peak.Connect(this.pcanHandle, this.m_baudrate)) {
                        this.can_Peak.CanRxMsgEvent += this.CanRxHandler;
                        this.can_Peak.CanTxHookEvent += this.CanTxHookHandler;

                        this.set_obj_states(true);
                        this.btn_Connect.Text = "Disconnect";
                    }
                } else {
                    this.can_Peak.CanRxMsgEvent -= this.CanRxHandler;
                    this.can_Peak.CanTxHookEvent -= this.CanTxHookHandler;
                    this.can_Peak.Disconnect();

                    this.set_obj_states(false);
                    this.btn_Connect.Text = "Connect";
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion // Button Event Handlers

        #region Combobox Event Handlers
        private void cbb_channel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strTemp = cbb_channel.Text;
            strTemp = strTemp.Substring(strTemp.IndexOf('B') + 1, 1);
            strTemp = "0x5" + strTemp;
            this.pcanHandle = Convert.ToByte(strTemp, 16);
        }
        private void cbb_baudrates_SelectedIndexChanged(object sender, EventArgs e)
        {
            TPCANBaudrate[] baudValue = (TPCANBaudrate[])Enum.GetValues(typeof(TPCANBaudrate));
            this.m_baudrate = baudValue[cbb_baudrates.SelectedIndex];
        }
        #endregion // Combobox Event Handlers

        #endregion // Methods
    }
}
