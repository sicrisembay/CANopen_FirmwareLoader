
namespace LSS
{
    partial class formUpdate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && ( components != null )) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formUpdate));
            this.label_vendor = new System.Windows.Forms.Label();
            this.label_product = new System.Windows.Forms.Label();
            this.label_revision = new System.Windows.Forms.Label();
            this.label_sn = new System.Windows.Forms.Label();
            this.label_nodeId = new System.Windows.Forms.Label();
            this.button_configure = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_newID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label_vendor
            // 
            this.label_vendor.AutoSize = true;
            this.label_vendor.Location = new System.Drawing.Point(26, 23);
            this.label_vendor.Name = "label_vendor";
            this.label_vendor.Size = new System.Drawing.Size(56, 13);
            this.label_vendor.TabIndex = 0;
            this.label_vendor.Text = "Vendor: ---";
            // 
            // label_product
            // 
            this.label_product.AutoSize = true;
            this.label_product.Location = new System.Drawing.Point(26, 47);
            this.label_product.Name = "label_product";
            this.label_product.Size = new System.Drawing.Size(59, 13);
            this.label_product.TabIndex = 0;
            this.label_product.Text = "Product: ---";
            // 
            // label_revision
            // 
            this.label_revision.AutoSize = true;
            this.label_revision.Location = new System.Drawing.Point(26, 71);
            this.label_revision.Name = "label_revision";
            this.label_revision.Size = new System.Drawing.Size(63, 13);
            this.label_revision.TabIndex = 0;
            this.label_revision.Text = "Revision: ---";
            // 
            // label_sn
            // 
            this.label_sn.AutoSize = true;
            this.label_sn.Location = new System.Drawing.Point(26, 95);
            this.label_sn.Name = "label_sn";
            this.label_sn.Size = new System.Drawing.Size(37, 13);
            this.label_sn.TabIndex = 0;
            this.label_sn.Text = "SN: ---";
            // 
            // label_nodeId
            // 
            this.label_nodeId.AutoSize = true;
            this.label_nodeId.Location = new System.Drawing.Point(26, 119);
            this.label_nodeId.Name = "label_nodeId";
            this.label_nodeId.Size = new System.Drawing.Size(62, 13);
            this.label_nodeId.TabIndex = 0;
            this.label_nodeId.Text = "Node ID: ---";
            // 
            // button_configure
            // 
            this.button_configure.Location = new System.Drawing.Point(178, 56);
            this.button_configure.Name = "button_configure";
            this.button_configure.Size = new System.Drawing.Size(132, 33);
            this.button_configure.TabIndex = 1;
            this.button_configure.Text = "Configure";
            this.button_configure.UseVisualStyleBackColor = true;
            this.button_configure.Click += new System.EventHandler(this.button_configure_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(178, 95);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(130, 36);
            this.button_cancel.TabIndex = 2;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "New Node ID (hex):";
            // 
            // textBox_newID
            // 
            this.textBox_newID.Location = new System.Drawing.Point(276, 25);
            this.textBox_newID.Name = "textBox_newID";
            this.textBox_newID.Size = new System.Drawing.Size(34, 20);
            this.textBox_newID.TabIndex = 4;
            this.textBox_newID.Text = "FF";
            // 
            // formUpdate
            // 
            this.AcceptButton = this.button_configure;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(327, 155);
            this.Controls.Add(this.textBox_newID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_configure);
            this.Controls.Add(this.label_nodeId);
            this.Controls.Add(this.label_sn);
            this.Controls.Add(this.label_revision);
            this.Controls.Add(this.label_product);
            this.Controls.Add(this.label_vendor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formUpdate";
            this.Text = "LSS Slave Update";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_vendor;
        private System.Windows.Forms.Label label_product;
        private System.Windows.Forms.Label label_revision;
        private System.Windows.Forms.Label label_sn;
        private System.Windows.Forms.Label label_nodeId;
        private System.Windows.Forms.Button button_configure;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_newID;
    }
}