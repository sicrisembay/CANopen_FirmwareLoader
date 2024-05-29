using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Peak.Can.Basic;
using can_hw;
using libCanopenSimple;

namespace FirmwareLoader
{
    using TPCANHandle = System.UInt16;

    public partial class FirmwareLoader : Form
    {
        public struct SNMTState
        {
            public byte state;
            public DateTime lastupdate;
            public bool dirty;
            public ListViewItem LVI;
            public bool isnew;
            public string statemsg;
        }

        #region Members
        private TPCANHandle pcanHandle;
        private TPCANBaudrate m_baudrate;
        private TPCANHandle[] m_HandlesArray;
        private pcan_usb can_Peak = null;
        #region File
        private IntelHexEntry hexEntry;
        private string m_filePath;
        #endregion // File
        private byte nodeId;
        private bool isBootDevice = false;
        private libCanopenSimple.libCanopenSimple lco = new libCanopenSimple.libCanopenSimple();
        private Dictionary<UInt32, string> sdoerrormessages = new Dictionary<UInt32, string>();
        Dictionary<UInt32, List<byte>> sdotransferdata = new Dictionary<uint, List<byte>>();
        Dictionary<UInt16, string> errcode = new Dictionary<ushort, string>();
        Dictionary<UInt16, string> errbit = new Dictionary<ushort, string>();
        Dictionary<UInt16, SNMTState> NMTstate = new Dictionary<ushort, SNMTState>();
        List<SNMTState> dirtyNMTstates = new List<SNMTState>();
        List<ListViewItem> listitem_monitor = new List<ListViewItem>();
        #endregion // Members

        #region Methods
        public FirmwareLoader()
        {
            InitializeComponent();

            this.can_Peak = new pcan_usb();
            this.hexEntry = new IntelHexEntry();

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

            this.lco.dbglevel = debuglevel.DEBUG_ALL;
            this.lco.nmtecevent += this.log_NMTEC;
            this.lco.nmtevent += this.log_NMT;
            this.lco.sdoevent += this.log_SDO;
            this.lco.emcyevent += this.log_EMCY;
            this.interror();

            this.isBootDevice = false;
        }

        private void FirmwareLoader_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.lco.close();
        }

        #region Helpers
        private void set_obj_states(bool connected)
        {
            this.cbb_channel.Enabled = !connected;
            this.cbb_baudrates.Enabled = !connected;
            this.btn_HwRefresh.Enabled = !connected;
            this.textBox_NodeID.Enabled = !connected;
            this.groupBox_Command.Enabled = connected;
            
            if(connected) {
                this.label_status.Text = "Status: Adapter connected";
            } else {
                this.label_status.Text = "Status: Adapter disconnected";
            }
        }
        #endregion

        #region Intel Hex Parser
        #endregion // Intel Hex Parser

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
                    this.nodeId = Convert.ToByte(this.textBox_NodeID.Text);
                    this.lco.open(this.pcanHandle, this.m_baudrate);
                    if (this.lco.isopen()) {
                        this.set_obj_states(true);
                        this.btn_Connect.Text = "Disconnect";
                    }
                } else {
                    this.lco.close();

                    this.set_obj_states(false);
                    this.btn_Connect.Text = "Connect";
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void button_OpenFile_Click(object sender, EventArgs e)
        {
            FileStream m_fileStreamReader;
            BinaryReader m_binaryReader;
            byte[] m_fileContent;


            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Intel Hex File (*.hex)|*.hex|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                this.hexEntry.isValid = false;
                var fileSize = new FileInfo(openFileDialog.FileName).Length;
                this.m_filePath = openFileDialog.FileName;
                m_fileStreamReader = File.Open(this.m_filePath, FileMode.Open);
                m_binaryReader = new BinaryReader(m_fileStreamReader);
                m_fileContent = new byte[fileSize];
                for (int i = 0; i < fileSize; i++) {
                    m_fileContent[i] = m_binaryReader.ReadByte();
                }
                m_binaryReader.Close();
                m_binaryReader = null;
                m_fileStreamReader.Close();
                m_fileStreamReader = null;

                this.hexEntry = new IntelHexEntry(m_fileContent);
                if (this.hexEntry.isValid) {
                    this.label_status.Text = "Status: Hex file loaded";
                } else {
                    this.label_status.Text = "Status: Invalid hex file";
                }
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

        #region Timer Handlers
        private void timer_update_Tick(object sender, EventArgs e)
        {
            int linelimit = 100;
            if (this.listitem_monitor.Count != 0) {
                lock (listitem_monitor) {
                    listView_monitor.BeginUpdate();
                    listView_monitor.Items.AddRange(listitem_monitor.ToArray());
                    listitem_monitor.Clear();
                    if (listView_monitor.Items.Count > 2) {
                        listView_monitor.EnsureVisible(listView_monitor.Items.Count - 1);
                    }

                    while (listView_monitor.Items.Count > linelimit) {
                        listView_monitor.Items.RemoveAt(0);
                    }

                    listView_monitor.EndUpdate();

                }
            }

            if (this.hexEntry.isValid && ( this.hexEntry.Count() > 0 )) {
                int progVal = ( 100 * this.hexEntryIdx ) / this.hexEntry.Count();
                if (progVal > 100) {
                    progVal = 100;
                }
                this.progressBar_download.Value = progVal;
            } else {
                this.progressBar_download.Value = 0;
            }
        }
        #endregion

        #region CANopen
        private void log_NMT(canpacket payload, DateTime dt)
        {
            string[] items = new string[6];
            items[0] = dt.ToString("MM/dd/yyyy HH:mm:ss.fff");
            items[1] = "NMT";
            items[2] = string.Format("{0:x3}", payload.cob);
            items[3] = "";
            items[4] = BitConverter.ToString(payload.data).Replace("-", string.Empty);

            string msg = "";

            if (payload.data.Length != 2)
                return;

            switch (payload.data[0]) {
                case 0x01:
                    msg = "Enter operational";
                    break;
                case 0x02:
                    msg = "Enter stop";
                    break;
                case 0x80:
                    msg = "Enter pre-operational";
                    break;
                case 0x81:
                    msg = "Reset node";
                    break;
                case 0x82:
                    msg = "Reset communications";
                    break;

            }

            if (payload.data[1] == 0) {
                msg += " - All nodes";
            } else {
                msg += string.Format(" - Node 0x{0:x2}", payload.data[1]);
            }

            items[5] = msg;

            Console.WriteLine(msg);

            ListViewItem i = new ListViewItem(items);
            i.ForeColor = Color.Red;
            lock (listitem_monitor) {
                listitem_monitor.Add(i);
            }

            //appendfile(items);

        }
        private void BootUp_cb(SDO sdo)
        {
            if(sdo.state == SDO.SDO_STATE.SDO_ERROR) {
                this.isBootDevice = false;
                return;
            }

            UInt32 deviceType = BitConverter.ToUInt32(sdo.databuffer, 0);
            if(deviceType == 0x424F4F54) { // "BOOT"
                this.isBootDevice = true;
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Bootloader device found"; });
            } else {
                this.isBootDevice = false;
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Bootloader device not found"; });
            }
        }
        private void log_NMTEC(canpacket payload, DateTime dt)
        {
            string[] items = new string[6];
            items[0] = dt.ToString("MM/dd/yyyy HH:mm:ss.fff");
            items[1] = "NMTEC";
            items[2] = string.Format("{0:x3}", payload.cob);
            items[3] = string.Format("{0:x3}", payload.cob & 0x0FF);
            items[4] = BitConverter.ToString(payload.data).Replace("-", string.Empty);

            string msg = "";
            switch (payload.data[0]) {
                case 0:
                    msg = "BOOT";
                    /* Any SDO transaction will force run the bootloader in the target */
                    this.lco.SDOread(this.nodeId, 0x1000, 0x00, BootUp_cb);
                    break;
                case 4:
                    msg = "STOPPED";
                    break;
                case 5:
                    msg = "Heart Beat";
                    break;
                case 0x7f:
                    msg = "Heart Beat (Pre op)";
                    break;
            }

            items[5] = msg;

            ListViewItem i = new ListViewItem(items);

            i.ForeColor = Color.DarkGreen;

//            appendfile(items);

            lock (listitem_monitor) {
                listitem_monitor.Add(i);
            }

            lock (NMTstate) {
                byte node = (byte)( payload.cob & 0x0FF );

                if (NMTstate.ContainsKey(node)) {
                    SNMTState s = NMTstate[node];
                    s.lastupdate = dt;
                    s.dirty = true;
                    s.state = payload.data[0];
                    s.isnew = false;
                    s.statemsg = msg;
                    NMTstate[node] = s;
                    dirtyNMTstates.Add(NMTstate[node]);
                } else {
                    SNMTState s = new SNMTState();
                    s.lastupdate = dt;
                    s.dirty = true;
                    s.state = payload.data[0];
                    s.statemsg = msg;
                    string[] ss = new string[3];
                    ss[0] = DateTime.Now.ToString();
                    ss[1] = string.Format("0x{0:x2} ({1})", node, node);
                    ss[2] = msg;

                    ListViewItem newitem = new ListViewItem(ss);
                    s.LVI = newitem;
                    s.isnew = true;

                    NMTstate.Add(node, s);
                    dirtyNMTstates.Add(NMTstate[node]);
                }
            }
        }
        private void log_SDO(canpacket payload, DateTime dt)
        {
            string[] items = new string[6];
            items[0] = dt.ToString("MM/dd/yyyy HH:mm:ss.fff");
            items[1] = "SDO";
            items[2] = string.Format("{0:x3}", payload.cob);

            if (payload.cob >= 0x580 && payload.cob < 0x600) {
                items[3] = string.Format("{0:x3}", ( ( payload.cob + 0x80 ) & 0x0FF ));
            } else {
                items[3] = string.Format("{0:x3}", payload.cob & 0x0FF);
            }

            items[4] = BitConverter.ToString(payload.data).Replace("-", string.Empty);

            string msg = "";


            int SCS = payload.data[0] >> 5; //7-5

            int n = ( 0x03 & ( payload.data[0] >> 2 ) ); //3-2 data size for normal packets
            int e = ( 0x01 & ( payload.data[0] >> 1 ) ); // expidited flag
            int s = ( payload.data[0] & 0x01 ); // data size set flag //also in block
            int c = s;

            int sn = ( 0x07 & ( payload.data[0] >> 1 ) ); //3-1 data size for segment packets
            int t = ( 0x01 & ( payload.data[0] >> 4 ) );  //toggle flag

            int cc = ( 0x01 & ( payload.data[0] >> 2 ) );



            UInt16 index = (UInt16)( payload.data[1] + ( payload.data[2] << 8 ) );
            byte sub = payload.data[3];


            int valid = 7;
            int validsn = 7;


            if (n != 0)
                valid = 8 - ( 7 - n );

            if (sn != 0)
                validsn = 8 - ( 7 - sn );


            if (payload.cob >= 0x580 && payload.cob <= 0x600) {
                string mode = "";
                string sdoproto = "";

                string setsize = "";

                switch (SCS) {
                    case 0:
                        mode = "upload segment response";
                        sdoproto = string.Format("{0} {1} Valid bytes = {2} {3}", mode, t == 1 ? "TOG ON" : "TOG OFF", validsn, c == 0 ? "MORE" : "END");

                        if (c == 1) {
                            //ipdo.endsdo(payload.cob, index, sub, null);
                            //END
                        }

                        if (sdotransferdata.ContainsKey(payload.cob)) {

                            for (int x = 1; x <= validsn; x++) {
                                sdotransferdata[payload.cob].Add(payload.data[x]);
                            }

                            if (c == 1) {

                                StringBuilder hex = new StringBuilder(sdotransferdata[payload.cob].Count * 2);
                                StringBuilder ascii = new StringBuilder(sdotransferdata[payload.cob].Count * 2);
                                foreach (byte b in sdotransferdata[payload.cob]) {
                                    hex.AppendFormat("{0:x2} ", b);
                                    ascii.AppendFormat("{0}", (char)Convert.ToChar(b));
                                }

                                //  textBox_info.Invoke(new MethodInvoker(delegate
                                //  {
                                //      textBox_info.AppendText(String.Format("SDO UPLOAD COMPLETE for cob 0x{0:x3}\r\n", payload.cob))
                                //
                                //      textBox_info.AppendText(hex.ToString() + "\r\n");
                                //     textBox_info.AppendText(ascii.ToString() + "\r\n\r\n");
                                //
                                //                                }));
                            }

                        }

                        break;
                    case 1:
                        mode = "download segment response";
                        sdoproto = string.Format("{0} {1}", mode, t == 1 ? "TOG ON" : "TOG OFF");
                        break;
                    case 2:
                        mode = "initate upload response";
                        string nbytes = "";

                        if (e == 1 && s == 1) {
                            //n is valid
                            nbytes = string.Format("Valid bytes = {0}", 4 - n);
                        }

                        if (e == 0 && s == 1) {
                            byte[] size = new byte[4];
                            Array.Copy(payload.data, 4, size, 0, 4);
                            UInt32 isize = (UInt32)BitConverter.ToUInt32(size, 0);
                            nbytes = string.Format("Bytes = {0}", isize);

                            if (sdotransferdata.ContainsKey(payload.cob))
                                sdotransferdata.Remove(payload.cob);

                            sdotransferdata.Add(payload.cob, new List<byte>());
                        }

                        sdoproto = string.Format("{0} {1} {2} 0x{3:x4}/{4:x2}", mode, nbytes, e == 1 ? "Normal" : "Expedite", index, sub);
                        break;
                    case 3:
                        mode = "initate download response";
                        sdoproto = string.Format("{0} 0x{1:x4}/{2:x2}", mode, index, sub);



                        break;

                    case 5:
                        mode = "Block download response";

                        byte segperblock = payload.data[4];
                        sdoproto = string.Format("{0} 0x{1:x4}/{2:x2} Blksize = {3}", mode, cc == 0 ? "NO SERVER CRC" : "SERVER CRC", index, sub, segperblock);

                        break;


                    default:
                        mode = string.Format("SCS {0}", SCS);
                        break;

                }



                msg = sdoproto;


            } else {
                //Client to server

                string mode = "";
                string sdoproto = "";

                switch (SCS) {
                    case 0:
                        mode = "download segment request";
                        sdoproto = string.Format("{0} {1} Valid bytes = {2} {3}", mode, t == 1 ? "TOG ON" : "TOG OFF", validsn, c == 0 ? "MORE" : "END");


                        if (sdotransferdata.ContainsKey(payload.cob)) {

                            for (int x = 1; x <= validsn; x++) {
                                sdotransferdata[payload.cob].Add(payload.data[x]);
                            }

                            if (c == 1) {

                                StringBuilder hex = new StringBuilder(sdotransferdata[payload.cob].Count * 2);
                                StringBuilder ascii = new StringBuilder(sdotransferdata[payload.cob].Count * 2);
                                foreach (byte b in sdotransferdata[payload.cob]) {
                                    hex.AppendFormat("{0:x2} ", b);
                                    ascii.AppendFormat("{0}", (char)Convert.ToChar(b));
                                }

                                //sdoproto += "\nDATA = " + hex.ToString() + "(" + ascii + ")";

                                /*  textBox_info.Invoke(new MethodInvoker(delegate
                                  {
                                      textBox_info.AppendText(String.Format("SDO DOWNLOAD COMPLETE for cob 0x{0:x3}\n", payload.cob));

                                      textBox_info.AppendText(hex.ToString() + "\n");
                                      textBox_info.AppendText(ascii.ToString() + "\n");
                                  }));*/


                                //Console.WriteLine(hex.ToString());
                                //Console.WriteLine(ascii.ToString());

                                sdotransferdata.Remove(payload.cob);
                            }
                        }


                        break;
                    case 1:
                        string nbytes = "";

                        if (e == 1 && s == 1) {
                            //n is valid
                            nbytes = string.Format("Valid bytes = {0}", 4 - n);
                        }

                        if (e == 0 && s == 1) {
                            byte[] size2 = new byte[4];
                            Array.Copy(payload.data, 4, size2, 0, 4);
                            UInt32 isize2 = (UInt32)BitConverter.ToUInt32(size2, 0);
                            nbytes = string.Format("Bytes = {0}", isize2);
                        }

                        mode = "initate download request";
                        sdoproto = string.Format("{0} {1} {2} 0x{3:x4}/{4:x2}", mode, nbytes, e == 1 ? "Normal" : "Expedite", index, sub);
                        if (sdotransferdata.ContainsKey(payload.cob))
                            sdotransferdata.Remove(payload.cob);

                        sdotransferdata.Add(payload.cob, new List<byte>());

                        break;
                    case 2:
                        mode = "initate upload request";
                        sdoproto = string.Format("{0} 0x{1:x4}/{2:x2}", mode, index, sub);
                        break;
                    case 3:
                        mode = "upload segment request";
                        sdoproto = string.Format("{0} {1}", mode, t == 1 ? "TOG ON" : "TOG OFF");
                        break;

                    case 5:
                        mode = "Block download";
                        sdoproto = string.Format("{0}", mode);
                        break;

                    case 6:
                        mode = "Initate Block download request";

                        byte[] size = new byte[4];
                        Array.Copy(payload.data, 4, size, 0, 4);
                        UInt32 isize = (UInt32)BitConverter.ToUInt32(size, 0);

                        sdoproto = string.Format("{0} 0x{1:x4}/{2:x2} Size = {3}", mode, cc == 0 ? "NO CLIENT CRC" : "CLIENT CRC", index, sub, isize);
                        break;


                    default:
                        mode = string.Format("CSC {0}", SCS);
                        break;

                }


                msg = sdoproto;

            }


            if (( payload.data[0] & 0x80 ) != 0) {
                byte[] errorcode = new byte[4];
                errorcode[0] = payload.data[4];
                errorcode[1] = payload.data[5];
                errorcode[2] = payload.data[6];
                errorcode[3] = payload.data[7];

                UInt32 err = BitConverter.ToUInt32(errorcode, 0);

                if (sdoerrormessages.ContainsKey(err)) {

                    msg += " " + sdoerrormessages[err];

                }

            } else {
//                if (ipdo != null)
//                    msg += " " + ipdo.decodesdo(payload.cob, index, sub, payload.data);
            }


            items[5] = msg;
//            appendfile(items);


            ListViewItem i = new ListViewItem(items);
            if (( payload.data[0] & 0x80 ) != 0) {
                i.BackColor = Color.Orange;
            }
            i.ForeColor = Color.DarkBlue;
            lock (listitem_monitor) {
                listitem_monitor.Add(i);
            }

        }
        private void log_EMCY(canpacket payload, DateTime dt)
        {
            string[] items = new string[6];
            string[] items2 = new string[5];

            items[0] = dt.ToString("MM/dd/yyyy HH:mm:ss.fff");
            items[1] = "EMCY";
            items[2] = string.Format("{0:x3}", payload.cob);
            items[3] = string.Format("{0:x3}", payload.cob - 0x080);
            items[4] = BitConverter.ToString(payload.data).Replace("-", string.Empty);
            //items[4] = "EMCY";

            items2[0] = dt.ToString("MM/dd/yyyy HH:mm:ss.fff");
            items2[1] = items[2];
            items2[2] = items[3];

            UInt16 code = (UInt16)( payload.data[0] + ( payload.data[1] << 8 ) );
            byte bits = (byte)( payload.data[3] );
            UInt32 info = (UInt32)( payload.data[4] + ( payload.data[5] << 8 ) + ( payload.data[6] << 16 ) + ( payload.data[7] << 24 ) );

            if (errcode.ContainsKey(code)) {

                string bitinfo;

                if (errbit.ContainsKey(bits)) {
                    bitinfo = errbit[bits];
                } else {
                    bitinfo = string.Format("bits 0x{0:x2}", bits);
                }

                items[5] = string.Format("Error: {0} - {1} info 0x{2:x8}", errcode[code], bitinfo, info);
            } else {
                items[5] = string.Format("Error code 0x{0:x4} bits 0x{1:x2} info 0x{2:x8}", code, bits, info);
            }

            items2[3] = items[5];

            ListViewItem i = new ListViewItem(items);
            ListViewItem i2 = new ListViewItem(items2);

            i.ForeColor = Color.White;
            i2.ForeColor = Color.White;

            if (code == 0) {
                i.BackColor = Color.Green;
                i2.BackColor = Color.Green;

            } else {
                i.BackColor = Color.Red;
                i2.BackColor = Color.Red;

            }

            lock (listitem_monitor) {
                listitem_monitor.Add(i);
            }

//            lock (EMClistitems)
//                EMClistitems.Add(i2);

//            appendfile(items);
        }
        private void interror()
        {

            errcode.Add(0x0000, "error Reset or No Error");
            errcode.Add(0x1000, "Generic Error");
            errcode.Add(0x2000, "Current");
            errcode.Add(0x2100, "device input side");
            errcode.Add(0x2200, "Current inside the device");
            errcode.Add(0x2300, "device output side");
            errcode.Add(0x3000, "Voltage");
            errcode.Add(0x3100, "Mains Voltage");
            errcode.Add(0x3200, "Voltage inside the device");
            errcode.Add(0x3300, "Output Voltage");
            errcode.Add(0x4000, "Temperature");
            errcode.Add(0x4100, "Ambient Temperature");
            errcode.Add(0x4200, "Device Temperature");
            errcode.Add(0x5000, "Device Hardware");
            errcode.Add(0x6000, "Device Software");
            errcode.Add(0x6100, "Internal Software");
            errcode.Add(0x6200, "User Software");
            errcode.Add(0x6300, "Data Set");
            errcode.Add(0x7000, "Additional Modules");
            errcode.Add(0x8000, "Monitoring");
            errcode.Add(0x8100, "Communication");
            errcode.Add(0x8110, "CAN Overrun (Objects lost)");
            errcode.Add(0x8120, "CAN in Error Passive Mode");
            errcode.Add(0x8130, "Life Guard Error or Heartbeat Error");
            errcode.Add(0x8140, "recovered from bus off");
            errcode.Add(0x8150, "CAN-ID collision");
            errcode.Add(0x8200, "Protocol Error");
            errcode.Add(0x8210, "PDO not processed due to length error");
            errcode.Add(0x8220, "PDO length exceeded");
            errcode.Add(0x8230, "destination object not available");
            errcode.Add(0x8240, "Unexpected SYNC data length");
            errcode.Add(0x8250, "RPDO timeout");
            errcode.Add(0x9000, "External Error");
            errcode.Add(0xF000, "Additional Functions");
            errcode.Add(0xFF00, "Device specific");

            errcode.Add(0x2310, "Current at outputs too high (overload)");
            errcode.Add(0x2320, "Short circuit at outputs");
            errcode.Add(0x2330, "Load dump at outputs");
            errcode.Add(0x3110, "Input voltage too high");
            errcode.Add(0x3120, "Input voltage too low");
            errcode.Add(0x3210, "Internal voltage too high");
            errcode.Add(0x3220, "Internal voltage too low");
            errcode.Add(0x3310, "Output voltage too high");
            errcode.Add(0x3320, "Output voltage too low");

            errbit.Add(0x00, "Error Reset or No Error");
            errbit.Add(0x01, "CAN bus warning limit reached");
            errbit.Add(0x02, "Wrong data length of the received CAN message");
            errbit.Add(0x03, "Previous received CAN message wasn't processed yet");
            errbit.Add(0x04, "Wrong data length of received PDO");
            errbit.Add(0x05, "Previous received PDO wasn't processed yet");
            errbit.Add(0x06, "CAN receive bus is passive");
            errbit.Add(0x07, "CAN transmit bus is passive");
            errbit.Add(0x08, "Wrong NMT command received");
            errbit.Add(0x09, "(unused)");
            errbit.Add(0x0A, "(unused)");
            errbit.Add(0x0B, "(unused)");
            errbit.Add(0x0C, "(unused)");
            errbit.Add(0x0D, "(unused)");
            errbit.Add(0x0E, "(unused)");
            errbit.Add(0x0F, "(unused)");

            errbit.Add(0x10, "(unused)");
            errbit.Add(0x11, "(unused)");
            errbit.Add(0x12, "CAN transmit bus is off");
            errbit.Add(0x13, "CAN module receive buffer has overflowed");
            errbit.Add(0x14, "CAN transmit buffer has overflowed");
            errbit.Add(0x15, "TPDO is outside SYNC window");
            errbit.Add(0x16, "(unused)");
            errbit.Add(0x17, "(unused)");
            errbit.Add(0x18, "SYNC message timeout");
            errbit.Add(0x19, "Unexpected SYNC data length");
            errbit.Add(0x1A, "Error with PDO mapping");
            errbit.Add(0x1B, "Heartbeat consumer timeout");
            errbit.Add(0x1C, "Heartbeat consumer detected remote node reset");
            errbit.Add(0x1D, "(unused)");
            errbit.Add(0x1E, "(unused)");
            errbit.Add(0x1F, "(unused)");

            errbit.Add(0x20, "Emergency message wasn't sent");
            errbit.Add(0x21, "(unused)");
            errbit.Add(0x22, "Microcontroller has just started");
            errbit.Add(0x23, "(unused)");
            errbit.Add(0x24, "(unused)");
            errbit.Add(0x25, "(unused)");
            errbit.Add(0x26, "(unused)");
            errbit.Add(0x27, "(unused)");

            errbit.Add(0x28, "Wrong parameters to CO_errorReport() function");
            errbit.Add(0x29, "Timer task has overflowed");
            errbit.Add(0x2A, "Unable to allocate memory for objects");
            errbit.Add(0x2B, "test usage");
            errbit.Add(0x2C, "Software error");
            errbit.Add(0x2D, "Object dictionary does not match the software");
            errbit.Add(0x2E, "Error in calculation of device parameters");
            errbit.Add(0x2F, "Error with access to non volatile device memory");

            sdoerrormessages.Add(0x05030000, "Toggle bit not altered");
            sdoerrormessages.Add(0x05040000, "SDO protocol timed out");
            sdoerrormessages.Add(0x05040001, "Command specifier not valid or unknown");
            sdoerrormessages.Add(0x05040002, "Invalid block size in block mode");
            sdoerrormessages.Add(0x05040003, "Invalid sequence number in block mode");
            sdoerrormessages.Add(0x05040004, "CRC error (block mode only)");
            sdoerrormessages.Add(0x05040005, "Out of memory");
            sdoerrormessages.Add(0x06010000, "Unsupported access to an object");
            sdoerrormessages.Add(0x06010001, "Attempt to read a write only object");
            sdoerrormessages.Add(0x06010002, "Attempt to write a read only object");
            sdoerrormessages.Add(0x06020000, "Object does not exist");
            sdoerrormessages.Add(0x06040041, "Object cannot be mapped to the PDO");
            sdoerrormessages.Add(0x06040042, "Number and length of object to be mapped exceeds PDO length");
            sdoerrormessages.Add(0x06040043, "General parameter incompatibility reasons");
            sdoerrormessages.Add(0x06040047, "General internal incompatibility in device");
            sdoerrormessages.Add(0x06060000, "Access failed due to hardware error");
            sdoerrormessages.Add(0x06070010, "Data type does not match, length of service parameter does not match");
            sdoerrormessages.Add(0x06070012, "Data type does not match, length of service parameter too high");
            sdoerrormessages.Add(0x06070013, "Data type does not match, length of service parameter too short");
            sdoerrormessages.Add(0x06090011, "Sub index does not exist");
            sdoerrormessages.Add(0x06090030, "Invalid value for parameter (download only).");
            sdoerrormessages.Add(0x06090031, "Value range of parameter written too high");
            sdoerrormessages.Add(0x06090032, "Value range of parameter written too low");
            sdoerrormessages.Add(0x06090036, "Maximum value is less than minimum value.");
            sdoerrormessages.Add(0x060A0023, "Resource not available: SDO connection");
            sdoerrormessages.Add(0x08000000, "General error");
            sdoerrormessages.Add(0x08000020, "Data cannot be transferred or stored to application");
            sdoerrormessages.Add(0x08000021, "Data cannot be transferred or stored to application because of local control");
            sdoerrormessages.Add(0x08000022, "Data cannot be transferred or stored to application because of present device state");
            sdoerrormessages.Add(0x08000023, "Object dictionary not present or dynamic generation fails");
            sdoerrormessages.Add(0x08000024, "No data available");
        }
        #endregion // CANopen

        #endregion // Methods


        private int hexEntryIdx = 0;
        void DomainDownload_cb(SDO sdo)
        {
            if(sdo.state == SDO.SDO_STATE.SDO_ERROR) {
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status:  **ERROR ** 0x1F50 0x01 Download Failed at " + this.hexEntryIdx; });
                return;
            }

            this.hexEntryIdx++;
            if(this.hexEntry.isValid && (this.hexEntryIdx < this.hexEntry.Count())) {
                this.lco.SDOwrite(this.nodeId, 0x1F50, 0x01, this.hexEntry.GetRecord(this.hexEntryIdx), DomainDownload_cb);
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Programming (" + this.hexEntryIdx + "/" + this.hexEntry.Count() + ")"; });                
            } else {
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Programm done"; });                
            }

        }
        void EraseSector_cb(SDO sdo)
        {
            if (sdo.state == SDO.SDO_STATE.SDO_ERROR) {
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status:  **ERROR ** 0x2000 0x01 0x03 EraseSector "; });
                return;
            }

            // Start Domain download
            this.hexEntryIdx = 0;
            this.lco.SDOwrite(this.nodeId, 0x1F50, 0x01, this.hexEntry.GetRecord(this.hexEntryIdx), DomainDownload_cb);
            this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Programming (" + this.hexEntryIdx + "/" + this.hexEntry.Count() + ")"; });            
        }
        void SetEraseSectorMask_cb(SDO sdo)
        {
            if (sdo.state == SDO.SDO_STATE.SDO_ERROR) {
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status:  **ERROR ** 0x2000 0x02 SetEraseSectorMask"; });
                return;
            }

            // Erase Sector
            UInt16 eraseCmd = 0x0003;
            byte[] command = BitConverter.GetBytes(eraseCmd);
            this.lco.SDOwrite(this.nodeId, 0x2000, 0x01, command, this.EraseSector_cb, timeout: 20);
            this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Erasing flash"; });
        }

        private void button_Program_Click(object sender, EventArgs e)
        {
            if (!this.isBootDevice) {
                this.label_status.Text = "Status: Boot device not found";
                return;
            }

            if (this.hexEntry.isValid) {
                /* 
                 * Erase Flash Sectors 
                 */
                // Set Erase Sector Mask
                UInt16 sectorMask = 0x00FE; // Sector Mask (B, C, D, E, F, G, H)
                byte[] command = BitConverter.GetBytes(sectorMask);
                this.lco.SDOwrite(this.nodeId, 0x2000, 0x02, command, this.SetEraseSectorMask_cb);
                this.label_status.Text = "Status: Setting erase sector mask";
            } else {
                this.label_status.Text = "Status: Invalid hex file";
            }
        }

        private void RunAppCb(SDO sdo)
        {
            if (sdo.state == SDO.SDO_STATE.SDO_ERROR) {
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Invalid app crc"; });
                return;
            }

            UInt32 command = 1;
            this.lco.SDOwrite(this.nodeId, 0x1F51, 0x01, command, null);
            this.isBootDevice = false;
            this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: App running"; });
        }

        private void button_runApp_Click(object sender, EventArgs e)
        {
            if (!this.isBootDevice) {
                this.label_status.Text = "Status: Boot device not found";
                return;
            }
            /* Check CRC */
            UInt16 command = 0x01;
            this.lco.SDOwrite(this.nodeId, 0x2000, 0x01, command, this.RunAppCb);
            this.label_status.Text = "Status: Checking App";
        }

        private void CrcCheckDone(SDO sdo)
        {
            if (sdo.state == SDO.SDO_STATE.SDO_ERROR) {
                this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: CRC Failed"; });
                return;
            }
            this.label_status.BeginInvoke((MethodInvoker)delegate () { this.label_status.Text = "Status: Verification OK"; });
        }

        private void button_checkCrc_Click(object sender, EventArgs e)
        {
            if(!this.isBootDevice) {
                this.label_status.Text = "Status: Boot device not found";
                return;
            }
            /* Check CRC */
            UInt16 command = 0x01;
            this.lco.SDOwrite(this.nodeId, 0x2000, 0x01, command, this.CrcCheckDone);
            this.label_status.Text = "Status: Verifying";
        }

        private void button_NmtReset_Click(object sender, EventArgs e)
        {
            this.lco.NMT_ResetNode(this.nodeId);
            this.label_status.Text = "Status: NMT reset sent to node:" + this.nodeId;
        }
    }

    public class IntelHexEntry
    {
        private List<byte[]> entries;
        public bool isValid;

        public IntelHexEntry()
        {
            this.isValid = false;
        }
        public IntelHexEntry(byte[] byteStream)
        {
            UInt32 i = 0;
            byte count;
            byte[] entry;

            this.isValid = false;
            this.entries = new List<byte[]>();

            while (i < byteStream.Length) {
                /* Check start code */
                if (':' != byteStream[i]) {
                    // skip
                    i++;
                    continue;
                }
                /* Get available bytes in stream */
                var availableBytes = byteStream.Length - ( i + 1 );
                /* Get count */
                char[] countCharArray = { (char)byteStream[i + 1], (char)byteStream[i + 2] };
                count = Convert.ToByte(new string(countCharArray), 16);
                if (availableBytes < ( ( count + 5 ) * 2 )) {
                    Console.WriteLine("Available data: " + availableBytes + ", Count: " + ( count + 5 ) * 2);
                    this.isValid = false;
                    return;
                }
                /* Calculate checksum */
                UInt32 checksum = 0;
                entry = new byte[count + 5];
                for (UInt32 j = 0; j < ( count + 5 ); j++) {
                    byte data = Convert.ToByte(new String(new char[] { (char)byteStream[( i + 1 ) + ( j * 2 )], (char)byteStream[( i + 1 ) + ( j * 2 ) + 1] }), 16);
                    entry[j] = data;
                    checksum += data;
                }
                if (0 != ( checksum & 0xff )) {
                    /* checksum failed */
                    Console.WriteLine("Invalid Checksum: record " + ( this.entries.Count + 1 ));
                    this.isValid = false;
                    return;
                }
                /* Valid Entry */
                this.entries.Add(entry);

                /* Next entry */
                i = i + 1 + ( ( (UInt32)count + 5 ) * 2 );
            }

            this.isValid = true;
        }

        public int Count()
        {
            if (this.isValid) {
                return ( this.entries.Count );
            } else {
                return 0;
            }
        }

        public byte[] GetRecord(int index)
        {
            if (this.isValid != true) {
                return null;
            }
            if (index >= this.entries.Count) {
                return null;
            }

            return ( this.entries[index] );
        }
    }
}
