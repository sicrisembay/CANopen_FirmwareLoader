
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
            this.components = new System.ComponentModel.Container();
            this.groupBox_Connection = new System.Windows.Forms.GroupBox();
            this.cbb_baudrates = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbb_channel = new System.Windows.Forms.ComboBox();
            this.textBox_NodeID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_HwRefresh = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.button_OpenFile = new System.Windows.Forms.Button();
            this.listView_monitor = new System.Windows.Forms.ListView();
            this.columnHeader_timestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_cob = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_node = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_payload = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_info = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.button_Program = new System.Windows.Forms.Button();
            this.progressBar_download = new System.Windows.Forms.ProgressBar();
            this.button_runApp = new System.Windows.Forms.Button();
            this.button_checkCrc = new System.Windows.Forms.Button();
            this.groupBox_Command = new System.Windows.Forms.GroupBox();
            this.button_NmtReset = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.groupBox_Connection.SuspendLayout();
            this.groupBox_Command.SuspendLayout();
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
            // button_OpenFile
            // 
            this.button_OpenFile.Location = new System.Drawing.Point(12, 50);
            this.button_OpenFile.Name = "button_OpenFile";
            this.button_OpenFile.Size = new System.Drawing.Size(106, 28);
            this.button_OpenFile.TabIndex = 52;
            this.button_OpenFile.Text = "Open File";
            this.button_OpenFile.UseVisualStyleBackColor = true;
            this.button_OpenFile.Click += new System.EventHandler(this.button_OpenFile_Click);
            // 
            // listView_monitor
            // 
            this.listView_monitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_monitor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_timestamp,
            this.columnHeader_type,
            this.columnHeader_cob,
            this.columnHeader_node,
            this.columnHeader_payload,
            this.columnHeader_info});
            this.listView_monitor.HideSelection = false;
            this.listView_monitor.Location = new System.Drawing.Point(12, 220);
            this.listView_monitor.Name = "listView_monitor";
            this.listView_monitor.Size = new System.Drawing.Size(662, 275);
            this.listView_monitor.TabIndex = 53;
            this.listView_monitor.UseCompatibleStateImageBehavior = false;
            this.listView_monitor.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_timestamp
            // 
            this.columnHeader_timestamp.Text = "Timestamp";
            this.columnHeader_timestamp.Width = 140;
            // 
            // columnHeader_type
            // 
            this.columnHeader_type.Text = "Type";
            // 
            // columnHeader_cob
            // 
            this.columnHeader_cob.Text = "COB";
            // 
            // columnHeader_node
            // 
            this.columnHeader_node.Text = "Node";
            // 
            // columnHeader_payload
            // 
            this.columnHeader_payload.Text = "Payload";
            // 
            // columnHeader_info
            // 
            this.columnHeader_info.Text = "Info";
            this.columnHeader_info.Width = 1000;
            // 
            // timer_update
            // 
            this.timer_update.Enabled = true;
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // button_Program
            // 
            this.button_Program.Location = new System.Drawing.Point(12, 82);
            this.button_Program.Name = "button_Program";
            this.button_Program.Size = new System.Drawing.Size(106, 28);
            this.button_Program.TabIndex = 55;
            this.button_Program.Text = "Program";
            this.button_Program.UseVisualStyleBackColor = true;
            this.button_Program.Click += new System.EventHandler(this.button_Program_Click);
            // 
            // progressBar_download
            // 
            this.progressBar_download.Location = new System.Drawing.Point(12, 179);
            this.progressBar_download.Name = "progressBar_download";
            this.progressBar_download.Size = new System.Drawing.Size(520, 26);
            this.progressBar_download.TabIndex = 56;
            // 
            // button_runApp
            // 
            this.button_runApp.Location = new System.Drawing.Point(12, 146);
            this.button_runApp.Name = "button_runApp";
            this.button_runApp.Size = new System.Drawing.Size(106, 28);
            this.button_runApp.TabIndex = 57;
            this.button_runApp.Text = "Run";
            this.button_runApp.UseVisualStyleBackColor = true;
            this.button_runApp.Click += new System.EventHandler(this.button_runApp_Click);
            // 
            // button_checkCrc
            // 
            this.button_checkCrc.Location = new System.Drawing.Point(12, 114);
            this.button_checkCrc.Name = "button_checkCrc";
            this.button_checkCrc.Size = new System.Drawing.Size(106, 28);
            this.button_checkCrc.TabIndex = 57;
            this.button_checkCrc.Text = "Verify";
            this.button_checkCrc.UseVisualStyleBackColor = true;
            this.button_checkCrc.Click += new System.EventHandler(this.button_checkCrc_Click);
            // 
            // groupBox_Command
            // 
            this.groupBox_Command.Controls.Add(this.button_NmtReset);
            this.groupBox_Command.Controls.Add(this.button_OpenFile);
            this.groupBox_Command.Controls.Add(this.button_runApp);
            this.groupBox_Command.Controls.Add(this.button_Program);
            this.groupBox_Command.Controls.Add(this.button_checkCrc);
            this.groupBox_Command.Location = new System.Drawing.Point(538, 21);
            this.groupBox_Command.Name = "groupBox_Command";
            this.groupBox_Command.Size = new System.Drawing.Size(134, 184);
            this.groupBox_Command.TabIndex = 58;
            this.groupBox_Command.TabStop = false;
            this.groupBox_Command.Text = "Commands";
            // 
            // button_NmtReset
            // 
            this.button_NmtReset.Location = new System.Drawing.Point(12, 18);
            this.button_NmtReset.Name = "button_NmtReset";
            this.button_NmtReset.Size = new System.Drawing.Size(106, 28);
            this.button_NmtReset.TabIndex = 52;
            this.button_NmtReset.Text = "NMT Reset";
            this.button_NmtReset.UseVisualStyleBackColor = true;
            this.button_NmtReset.Click += new System.EventHandler(this.button_NmtReset_Click);
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_status.Location = new System.Drawing.Point(12, 156);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(142, 17);
            this.label_status.TabIndex = 59;
            this.label_status.Text = "Status: Disconnected";
            // 
            // FirmwareLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 507);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.groupBox_Command);
            this.Controls.Add(this.progressBar_download);
            this.Controls.Add(this.listView_monitor);
            this.Controls.Add(this.groupBox_Connection);
            this.Name = "FirmwareLoader";
            this.Text = "Firmware Loader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FirmwareLoader_FormClosing);
            this.groupBox_Connection.ResumeLayout(false);
            this.groupBox_Connection.PerformLayout();
            this.groupBox_Command.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button button_OpenFile;
        private System.Windows.Forms.ListView listView_monitor;
        private System.Windows.Forms.ColumnHeader columnHeader_timestamp;
        private System.Windows.Forms.ColumnHeader columnHeader_type;
        private System.Windows.Forms.ColumnHeader columnHeader_cob;
        private System.Windows.Forms.ColumnHeader columnHeader_node;
        private System.Windows.Forms.ColumnHeader columnHeader_payload;
        private System.Windows.Forms.ColumnHeader columnHeader_info;
        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Button button_Program;
        private System.Windows.Forms.ProgressBar progressBar_download;
        private System.Windows.Forms.Button button_runApp;
        private System.Windows.Forms.Button button_checkCrc;
        private System.Windows.Forms.GroupBox groupBox_Command;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Button button_NmtReset;
    }
}

