
namespace LSS
{
    partial class LssManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LssManager));
            this.groupBox_Connection = new System.Windows.Forms.GroupBox();
            this.cbb_baudrates = new System.Windows.Forms.ComboBox();
            this.cbb_channel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_HwRefresh = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.groupBox_network = new System.Windows.Forms.GroupBox();
            this.button_Reset = new System.Windows.Forms.Button();
            this.treeView_network = new System.Windows.Forms.TreeView();
            this.btn_ScanNetwork = new System.Windows.Forms.Button();
            this.timer_ScanSM = new System.Windows.Forms.Timer(this.components);
            this.groupBox_Connection.SuspendLayout();
            this.groupBox_network.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Connection
            // 
            this.groupBox_Connection.Controls.Add(this.cbb_baudrates);
            this.groupBox_Connection.Controls.Add(this.cbb_channel);
            this.groupBox_Connection.Controls.Add(this.label2);
            this.groupBox_Connection.Controls.Add(this.label1);
            this.groupBox_Connection.Controls.Add(this.btn_HwRefresh);
            this.groupBox_Connection.Controls.Add(this.btn_Connect);
            this.groupBox_Connection.Location = new System.Drawing.Point(12, 12);
            this.groupBox_Connection.Name = "groupBox_Connection";
            this.groupBox_Connection.Size = new System.Drawing.Size(526, 88);
            this.groupBox_Connection.TabIndex = 52;
            this.groupBox_Connection.TabStop = false;
            this.groupBox_Connection.Text = "Connection";
            // 
            // cbb_baudrates
            // 
            this.cbb_baudrates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_baudrates.Location = new System.Drawing.Point(182, 43);
            this.cbb_baudrates.Name = "cbb_baudrates";
            this.cbb_baudrates.Size = new System.Drawing.Size(152, 21);
            this.cbb_baudrates.TabIndex = 2;
            this.cbb_baudrates.SelectedIndexChanged += new System.EventHandler(this.cbb_baudrates_SelectedIndexChanged);
            // 
            // cbb_channel
            // 
            this.cbb_channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_channel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_channel.Location = new System.Drawing.Point(22, 43);
            this.cbb_channel.Name = "cbb_channel";
            this.cbb_channel.Size = new System.Drawing.Size(154, 21);
            this.cbb_channel.TabIndex = 1;
            this.cbb_channel.SelectedIndexChanged += new System.EventHandler(this.cbb_channel_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(179, 25);
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
            this.btn_HwRefresh.Location = new System.Drawing.Point(340, 25);
            this.btn_HwRefresh.Name = "btn_HwRefresh";
            this.btn_HwRefresh.Size = new System.Drawing.Size(65, 44);
            this.btn_HwRefresh.TabIndex = 46;
            this.btn_HwRefresh.Text = "Refresh";
            this.btn_HwRefresh.Click += new System.EventHandler(this.btn_HwRefresh_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(411, 25);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(96, 44);
            this.btn_Connect.TabIndex = 6;
            this.btn_Connect.Text = "Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // groupBox_network
            // 
            this.groupBox_network.Controls.Add(this.button_Reset);
            this.groupBox_network.Controls.Add(this.treeView_network);
            this.groupBox_network.Controls.Add(this.btn_ScanNetwork);
            this.groupBox_network.Location = new System.Drawing.Point(12, 106);
            this.groupBox_network.Name = "groupBox_network";
            this.groupBox_network.Size = new System.Drawing.Size(526, 455);
            this.groupBox_network.TabIndex = 53;
            this.groupBox_network.TabStop = false;
            this.groupBox_network.Text = "Network";
            // 
            // button_Reset
            // 
            this.button_Reset.Location = new System.Drawing.Point(6, 68);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(85, 47);
            this.button_Reset.TabIndex = 2;
            this.button_Reset.Text = "Reset Bus";
            this.button_Reset.UseVisualStyleBackColor = true;
            this.button_Reset.Click += new System.EventHandler(this.button_Reset_Click);
            // 
            // treeView_network
            // 
            this.treeView_network.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView_network.Location = new System.Drawing.Point(101, 19);
            this.treeView_network.Name = "treeView_network";
            this.treeView_network.Size = new System.Drawing.Size(406, 418);
            this.treeView_network.TabIndex = 1;
            this.treeView_network.DoubleClick += new System.EventHandler(this.treeView_network_DoubleClick);
            // 
            // btn_ScanNetwork
            // 
            this.btn_ScanNetwork.Location = new System.Drawing.Point(6, 19);
            this.btn_ScanNetwork.Name = "btn_ScanNetwork";
            this.btn_ScanNetwork.Size = new System.Drawing.Size(85, 43);
            this.btn_ScanNetwork.TabIndex = 0;
            this.btn_ScanNetwork.Text = "Scan";
            this.btn_ScanNetwork.UseVisualStyleBackColor = true;
            this.btn_ScanNetwork.Click += new System.EventHandler(this.btn_ScanNetwork_Click);
            // 
            // timer_ScanSM
            // 
            this.timer_ScanSM.Interval = 1;
            this.timer_ScanSM.Tick += new System.EventHandler(this.timer_ScanSM_Tick);
            // 
            // LssManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 573);
            this.Controls.Add(this.groupBox_network);
            this.Controls.Add(this.groupBox_Connection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LssManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Layer Setting Service Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LssManager_FormClosing);
            this.groupBox_Connection.ResumeLayout(false);
            this.groupBox_network.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Connection;
        private System.Windows.Forms.ComboBox cbb_baudrates;
        private System.Windows.Forms.ComboBox cbb_channel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_HwRefresh;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.GroupBox groupBox_network;
        private System.Windows.Forms.Button btn_ScanNetwork;
        private System.Windows.Forms.TreeView treeView_network;
        private System.Windows.Forms.Timer timer_ScanSM;
        private System.Windows.Forms.Button button_Reset;
    }
}

