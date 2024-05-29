using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Peak.Can.Basic;
using can_hw;

namespace LSS
{
    using TPCANHandle = System.UInt16;

    public partial class LssManager : Form
    {
        private const UInt16 COBID_MASTER_TO_SLAVE = 0x7E5;
        private const UInt16 COBID_SLAVE_TO_MASTER = 0x7E4;
        private enum LSS_CS : byte
        {
            SWITCH_MODE_GLOBAL = 0x04,
            CONFIGURE_NODE_ID = 0x11,
            STORE_CONFIGURATION = 0x17,
            SWITCH_MODE_SELECT_VENDOR_ID = 0x40,
            SWITCH_MODE_SELECT_PRODUCT_ID = 0x41,
            SWITCH_MODE_SELECT_REV_NUM = 0x42,
            SWITCH_MODE_SELECT_SERIAL_NUM = 0x43,
            SWITCH_MODE_SELECTIVE_REPLY = 0x44,
            INQUIRE_IDENTITY_VENDOR = 0x5A,
            INQUIRE_IDENTITY_PRODUCT = 0x5B,
            INQUIRE_IDENTITY_REVISION = 0x5C,
            INQUIRE_IDENTITY_SERIAL_NUMBER = 0x5D,
            INQUIRE_NODE_ID = 0x5E,
        }
        private enum SCAN_STATE
        {
            IDLE = 0,
            INQUIRE,
            SELECTIVE_MODE,
        }
        #region Members
        #region CAN Adapter
        private TPCANHandle pcanHandle;
        private TPCANBaudrate m_baudrate;
        private TPCANHandle[] m_HandlesArray;
        private pcan_usb can_Peak = null;
        #endregion
        #region LSS
        private SCAN_STATE scanState = SCAN_STATE.IDLE;
        private List<UInt32> vendorList;
        private List<UInt32> productList;
        private List<UInt32> revList;
        private List<UInt32> serialNumList;
        private UInt32 scanStateTick;
        private UInt32 selectedVendorID;
        private UInt32 selectedProductID;
        private UInt32 selectedRevNum;
        private UInt32 selectedSerialNum;
        private byte selectedNodeId;
        private bool bSelected;
        private byte configureID_error;
        private byte configureID_specError;
        private byte store_error;
        private byte store_specError;
        private TreeNode rootNode;
        #endregion
        private formUpdate formUpdateNode;
        #endregion

        public LssManager()
        {
            InitializeComponent();
            this.can_Peak = new pcan_usb();
            this.can_Peak.CanRxMsgEvent += LSS_ReplyPacket;
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
            this.rootNode = treeView_network.Nodes.Add("root", "Found 0 Node");
            this.vendorList = new List<uint>();
            this.productList = new List<uint>();
            this.revList = new List<uint>();
            this.serialNumList = new List<uint>();
        }

        #region Helpers
        private void set_obj_states(bool connected)
        {
            this.cbb_channel.Enabled = !connected;
            this.cbb_baudrates.Enabled = !connected;
            this.btn_HwRefresh.Enabled = !connected;
            this.groupBox_network.Enabled = connected;
        }
        private bool IsFound(List<UInt32> list, UInt32 value)
        {
            bool ret = false;
            if(list.Count > 0) {
                var arr = list.ToArray();
                for(int i = 0; i < arr.Length; i++) {
                    if(arr[i] == value) {
                        ret = true;
                        break;
                    }
                }
            }
            return ( ret );
        }
        #endregion

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
                    this.can_Peak.Connect(this.pcanHandle, this.m_baudrate);
                    this.set_obj_states(true);
                    this.btn_Connect.Text = "Disconnect";
                } else {
                    this.can_Peak.Disconnect();
                    this.set_obj_states(false);
                    this.btn_Connect.Text = "Connect";
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }
        private void btn_ScanNetwork_Click(object sender, EventArgs e)
        {
            if (scanState == SCAN_STATE.IDLE) {
                btn_ScanNetwork.Enabled = false;
                this.scanState = SCAN_STATE.INQUIRE;
                this.timer_ScanSM.Interval = 10;
                this.timer_ScanSM.Enabled = true;
            }
        }
        #endregion

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

        private void LssManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.can_Peak.Disconnect();
        }

        #region LSS
        private void LSS_SwitchModeGlobal(byte mode)
        {
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.SWITCH_MODE_GLOBAL;
            if(mode == 0) {
                data[1] = 0; // Operation Mode
                this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
            } else if(mode == 1) {
                data[1] = 1; // Configuration Mode
                this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
            }
        }
        private void LSS_SwitchModeSelective(UInt32 vendorId, UInt32 productCode,
                        UInt32 revisionNumber, UInt32 serialNumber)
        {
            byte[] data = new byte[8];
            this.bSelected = false;
            this.selectedVendorID = vendorId;
            this.selectedProductID = productCode;
            this.selectedRevNum = revisionNumber;
            this.selectedSerialNum = serialNumber;

            // Vendor ID
            data[0] = (byte)LSS_CS.SWITCH_MODE_SELECT_VENDOR_ID;
            BitConverter.GetBytes(this.selectedVendorID).CopyTo(data, 1);
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);

            // Product Code
            data[0] = (byte)LSS_CS.SWITCH_MODE_SELECT_PRODUCT_ID;
            BitConverter.GetBytes(this.selectedProductID).CopyTo(data, 1);
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);

            // Revision Number
            data[0] = (byte)LSS_CS.SWITCH_MODE_SELECT_REV_NUM;
            BitConverter.GetBytes(this.selectedRevNum).CopyTo(data, 1);
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);

            // Serial Number
            data[0] = (byte)LSS_CS.SWITCH_MODE_SELECT_SERIAL_NUM;
            BitConverter.GetBytes(this.selectedSerialNum).CopyTo(data, 1);
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_InquireVendor()
        {
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.INQUIRE_IDENTITY_VENDOR;
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_InquireProduct()
        {
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.INQUIRE_IDENTITY_PRODUCT;
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_InquireRevision()
        {
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.INQUIRE_IDENTITY_REVISION;
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_InquireSerialNumber()
        {
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.INQUIRE_IDENTITY_SERIAL_NUMBER;
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_InquireNodeId()
        {
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.INQUIRE_NODE_ID;
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_ConfigureNodeId(byte newId)
        {
            this.configureID_error = 0xFF;
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.CONFIGURE_NODE_ID;
            data[1] = newId;
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_StoreConfiguration()
        {
            this.store_error = 0xFF;
            byte[] data = new byte[8];
            data[0] = (byte)LSS_CS.STORE_CONFIGURATION;
            this.can_Peak.SendStandard(COBID_MASTER_TO_SLAVE, data);
        }
        private void LSS_ReplyPacket(object sender, CanRxMsgArgs e)
        {
            if(e.msgId == COBID_SLAVE_TO_MASTER) {
                switch((LSS_CS)e.data[0]) {
                    case LSS_CS.CONFIGURE_NODE_ID: {
                        this.configureID_error = e.data[1];
                        this.configureID_specError = e.data[2];
                        break;
                    }
                    case LSS_CS.STORE_CONFIGURATION: {
                        this.store_error = e.data[1];
                        this.store_specError = e.data[2];
                        break;
                    }
                    case LSS_CS.INQUIRE_IDENTITY_VENDOR: {
                        UInt32 value = BitConverter.ToUInt32(e.data, 1);
                        if(!IsFound(vendorList, value)) {
                            vendorList.Add(value);
                            Console.WriteLine("Vendor found: 0x" + value.ToString("X8"));
                        }
                        break;
                    }
                    case LSS_CS.INQUIRE_IDENTITY_PRODUCT: {
                        UInt32 value = BitConverter.ToUInt32(e.data, 1);
                        if (!IsFound(productList, value)) {
                            productList.Add(value);
                            Console.WriteLine("Product 0x" + value.ToString("X8") + " added");
                        }
                        break;
                    }
                    case LSS_CS.INQUIRE_IDENTITY_REVISION: {
                        UInt32 value = BitConverter.ToUInt32(e.data, 1);
                        if (!IsFound(revList, value)) {
                            revList.Add(value);
                            Console.WriteLine("Rev 0x" + value.ToString("X8") + " added");
                        }
                        break;
                    }
                    case LSS_CS.INQUIRE_IDENTITY_SERIAL_NUMBER: {
                        UInt32 sn = BitConverter.ToUInt32(e.data, 1);
                        Console.WriteLine("Node Found with SN: 0x" + sn.ToString("X8"));
                        serialNumList.Add(sn);
                        break;
                    }
                    case LSS_CS.SWITCH_MODE_SELECTIVE_REPLY: {
                        this.bSelected = true;
                        break;
                    }
                    case LSS_CS.INQUIRE_NODE_ID: {
                        this.selectedNodeId = e.data[1];
                        break;
                    }
                    default: {
                        break;
                    }
                }
            }
        }
        #endregion

        private void timer_ScanSM_Tick(object sender, EventArgs e)
        {
            scanStateTick++;
            switch(scanState) {
                case SCAN_STATE.IDLE: {
                    timer_ScanSM.Enabled = false;
                    btn_ScanNetwork.Enabled = true;
                    break;
                }
                case SCAN_STATE.INQUIRE: {
                    rootNode.Nodes.Clear();
                    vendorList.Clear();
                    productList.Clear();
                    revList.Clear();
                    serialNumList.Clear();
                    LSS_SwitchModeGlobal(1);
                    LSS_InquireVendor();
                    Thread.Sleep(100);
                    LSS_InquireProduct();
                    Thread.Sleep(100);
                    LSS_InquireRevision();
                    Thread.Sleep(100);
                    LSS_InquireSerialNumber();
                    Thread.Sleep(100);
                    LSS_SwitchModeGlobal(0);
                    rootNode.Text = "Found " + serialNumList.Count + " Nodes";
                    if (serialNumList.Count > 0) {
                        var arr = serialNumList.ToArray();
                        for(int i = 0; i < arr.Length; i++) {
                            LssTreeNode node = new LssTreeNode(arr[i]);
                            rootNode.Nodes.Add(node);
                        }
                        scanState = SCAN_STATE.SELECTIVE_MODE;
                    } else {
                        this.scanState = SCAN_STATE.IDLE;
                    }
                    break;
                }
                case SCAN_STATE.SELECTIVE_MODE: {
                    var arrVendor = this.vendorList.ToArray();
                    var arrProduct = this.productList.ToArray();
                    var arrRev = this.revList.ToArray();
                    // Search the 128-bit LSS ID
                    for(int i = 0; i < rootNode.Nodes.Count; i++) {
                        LssTreeNode node = (LssTreeNode)rootNode.Nodes[i];
                        for (int j = 0; j < arrVendor.Length; j++) {
                            for(int k = 0; k < arrProduct.Length; k++) {
                                for(int l = 0; l < arrRev.Length; l++) {
                                    LSS_SwitchModeSelective(arrVendor[j], arrProduct[k], arrRev[l], node.serialNumber);
                                    Thread.Sleep(50);
                                    if(this.bSelected) {
                                        node.vendor = this.selectedVendorID;
                                        node.product = this.selectedProductID;
                                        node.rev = this.selectedRevNum;
                                        LSS_InquireNodeId();
                                        Thread.Sleep(50);
                                        node.SetNodeId(this.selectedNodeId);
                                        LSS_SwitchModeGlobal(0);
                                        Thread.Sleep(10);
                                        break;
                                    }
                                }
                                if(this.bSelected) {
                                    break;
                                }
                            }
                            if(this.bSelected) {
                                break;
                            }
                        }
                    }
                    LSS_SwitchModeGlobal(0);
                    scanState = SCAN_STATE.IDLE;
                    break;
                }
                default: {
                    scanState = SCAN_STATE.IDLE;
                    break;
                }
            }
        }

        private void treeView_network_DoubleClick(object sender, EventArgs e)
        {
            if (null == this.treeView_network.SelectedNode) {
                return;
            }
            if (1 != this.treeView_network.SelectedNode.Level) {
                return;
            }
            if (typeof(LssTreeNode) != this.treeView_network.SelectedNode.GetType()) {
                return;
            }
            LssTreeNode node = (LssTreeNode)this.treeView_network.SelectedNode;
            Console.WriteLine("Configure 0x" + node.serialNumber.ToString("X8"));
            this.formUpdateNode = new formUpdate(node.vendor, node.product, node.rev, node.serialNumber, node.nodeId);
            var result = this.formUpdateNode.ShowDialog();
            if(result == DialogResult.OK) {
                if(formUpdateNode.newID != node.nodeId) {
                    if(((formUpdateNode.newID > 0) && (formUpdateNode.newID <= 127)) ||
                        (formUpdateNode.newID == 0xFF)) {
                        LSS_SwitchModeSelective(node.vendor, node.product, node.rev, node.serialNumber);
                        Thread.Sleep(50);
                        if (this.bSelected) {
                            LSS_ConfigureNodeId(formUpdateNode.newID);
                            Thread.Sleep(50);
                            if (this.configureID_error == 0) {
                                LSS_StoreConfiguration();
                                Thread.Sleep(50);
                                if (this.store_error == 0) {
                                    MessageBox.Show("Configured Successfully");
                                } else {
                                    String strError = "Store Failed with Error: " + this.store_error;
                                    if (this.configureID_error == 0xFF) {
                                        strError += ", Specific: " + this.store_specError;
                                    }
                                    MessageBox.Show(strError);
                                }
                            } else {
                                String strError = "Configure ID Failed with Error: " + this.configureID_error;
                                if (this.configureID_error == 0xFF) {
                                    strError += ", Specific: " + this.configureID_specError;
                                }
                                MessageBox.Show(strError);
                            }
                        }
                        LSS_SwitchModeGlobal(0);
                    } else {
                        MessageBox.Show("Invalid Node ID: 0x" + formUpdateNode.newID.ToString("X2"));
                    }
                }
            }
        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            // Reset Network (All nodes)
            byte[] data = new byte[2];
            data[0] = 0x81;
            data[1] = 0;
            can_Peak.SendStandard(0, data);
        }
    }

    public class LssTreeNode : TreeNode
    {
        public UInt32 vendor;
        public UInt32 product;
        public UInt32 rev;
        public UInt32 serialNumber { get; private set; }
        public byte nodeId { get; private set; }
        public bool nodeConfigured { get; private set; }

        public LssTreeNode(UInt32 serialNumber)
        {
            this.serialNumber = serialNumber;
            this.vendor = 0;
            this.product = 0;
            this.rev = 0;
            this.nodeConfigured = false;
            this.Text = "SN: 0x" + serialNumber.ToString("X8") + ", ID: --";
        }

        public void SetNodeId(byte nodeId)
        {
            this.nodeId = nodeId;
            this.Text = "SN: 0x" + serialNumber.ToString("X8") + ", ID: 0x" + nodeId.ToString("X2");
            if ((nodeId > 0) && (nodeId < 128)) {
                this.nodeConfigured = true;
            } else {
                this.nodeConfigured = false;
                this.Text += "(UNCONFIGURED)";
            }
        }
    }
}
