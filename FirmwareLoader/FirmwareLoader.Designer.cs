
namespace FirmwareLoader
{
    partial class FirmwareLoader
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
            this.groupBox_Connection = new System.Windows.Forms.GroupBox();
            this.cbb_baudrates = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbb_channel = new System.Windows.Forms.ComboBox();
            this.textBox_NodeID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_HwRefresh = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.groupBox_Connection.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Connection
            // 
            this.groupBox_Connection.Controls.Add(this.cbb_baudrates);
            this.groupBox_Connection.Controls.Add(this.label3);
            this.groupBox_Connection.Controls.Add(this.cbb_channel);
            this.groupBox_Connection.Controls.Add(this.textBox_NodeID);
            this.groupBox_Connection.Controls.Add(this.label2);
            this.groupBox_Connection.Controls.Add(this.label1);
            this.groupBox_Connection.Controls.Add(this.btn_HwRefresh);
            this.groupBox_Connection.Controls.Add(this.btn_Connect);
            this.groupBox_Connection.Location = new System.Drawing.Point(12, 21);
            this.groupBox_Connection.Name = "groupBox_Connection";
            this.groupBox_Connection.Size = new System.Drawing.Size(520, 121);
            this.groupBox_Connection.TabIndex = 51;
            this.groupBox_Connection.TabStop = false;
            this.groupBox_Connection.Text = "Connection";
            // 
            // cbb_baudrates
            // 
            this.cbb_baudrates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_baudrates.Location = new System.Drawing.Point(150, 43);
            this.cbb_baudrates.Name = "cbb_baudrates";
            this.cbb_baudrates.Size = new System.Drawing.Size(152, 21);
            this.cbb_baudrates.TabIndex = 2;
            this.cbb_baudrates.SelectedIndexChanged += new System.EventHandler(this.cbb_baudrates_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 49;
            this.label3.Text = "Node ID:";
            // 
            // cbb_channel
            // 
            this.cbb_channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_channel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_channel.Location = new System.Drawing.Point(22, 43);
            this.cbb_channel.Name = "cbb_channel";
            this.cbb_channel.Size = new System.Drawing.Size(119, 21);
            this.cbb_channel.TabIndex = 1;
            this.cbb_channel.SelectedIndexChanged += new System.EventHandler(this.cbb_channel_SelectedIndexChanged);
            // 
            // textBox_NodeID
            // 
            this.textBox_NodeID.Location = new System.Drawing.Point(70, 80);
            this.textBox_NodeID.Name = "textBox_NodeID";
            this.textBox_NodeID.Size = new System.Drawing.Size(42, 20);
            this.textBox_NodeID.TabIndex = 3;
            this.textBox_NodeID.Text = "10";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(147, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 15);
            this.label2.TabIndex = 43;
            this.label2.Text = "Baudrate:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(19, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 42;
            this.label1.Text = "Channel:";
            // 
            // btn_HwRefresh
            // 
            this.btn_HwRefresh.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_HwRefresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_HwRefresh.Location = new System.Drawing.Point(310, 43);
            this.btn_HwRefresh.Name = "btn_HwRefresh";
            this.btn_HwRefresh.Size = new System.Drawing.Size(65, 57);
            this.btn_HwRefresh.TabIndex = 46;
            this.btn_HwRefresh.Text = "Refresh";
            this.btn_HwRefresh.Click += new System.EventHandler(this.btn_HwRefresh_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(381, 43);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(96, 57);
            this.btn_Connect.TabIndex = 6;
            this.btn_Connect.Text = "Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // FirmwareLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox_Connection);
            this.Name = "FirmwareLoader";
            this.Text = "Firmware Loader";
            this.groupBox_Connection.ResumeLayout(false);
            this.groupBox_Connection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Connection;
        private System.Windows.Forms.ComboBox cbb_baudrates;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbb_channel;
        private System.Windows.Forms.TextBox textBox_NodeID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_HwRefresh;
        private System.Windows.Forms.Button btn_Connect;
    }
}

