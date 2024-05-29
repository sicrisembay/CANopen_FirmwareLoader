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
using can_hw;

namespace LSS
{
    public partial class formUpdate : Form
    {
        public byte newID { get; private set; }

        public formUpdate(UInt32 vendor, UInt32 product, UInt32 rev, UInt32 sn, byte nodeId)
        {
            InitializeComponent();
            this.label_vendor.Text = "Vendor: 0x" + vendor.ToString("X8");
            this.label_product.Text = "Product: 0x" + product.ToString("X8");
            this.label_revision.Text = "Revision: " + ( rev >> 16 ).ToString("D2") + "." + ( rev & 0xFFFF ).ToString("D2");
            this.label_sn.Text = "SN: 0x" + sn.ToString("X8");
            this.label_nodeId.Text = "Node ID: 0x" + nodeId.ToString("X2");
        }

        private void button_configure_Click(object sender, EventArgs e)
        {
            try {
                this.newID = Convert.ToByte(this.textBox_newID.Text, 16);
                this.DialogResult = DialogResult.OK;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {

        }
    }
}
