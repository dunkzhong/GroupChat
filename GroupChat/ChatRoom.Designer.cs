namespace GroupChat
{
    partial class ChatRoom
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.message_chat = new System.Windows.Forms.RichTextBox();
            this.btn_send = new System.Windows.Forms.Button();
            this.btn_upload = new System.Windows.Forms.LinkLabel();
            this.btn_quit = new System.Windows.Forms.Button();
            this.record_chat = new System.Windows.Forms.RichTextBox();
            this.list_file = new System.Windows.Forms.ListView();
            this.file_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.up_time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.file_size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.up_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.list_user = new System.Windows.Forms.ListView();
            this.computer_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ip_address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbUserCount = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // message_chat
            // 
            this.message_chat.Location = new System.Drawing.Point(219, 310);
            this.message_chat.Name = "message_chat";
            this.message_chat.Size = new System.Drawing.Size(494, 131);
            this.message_chat.TabIndex = 0;
            this.message_chat.Text = "";
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(363, 447);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(120, 38);
            this.btn_send.TabIndex = 1;
            this.btn_send.Text = "发送";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // btn_upload
            // 
            this.btn_upload.AutoSize = true;
            this.btn_upload.Location = new System.Drawing.Point(647, 295);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(53, 12);
            this.btn_upload.TabIndex = 3;
            this.btn_upload.TabStop = true;
            this.btn_upload.Text = "上传文件";
            this.btn_upload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btn_upload_LinkClicked);
            // 
            // btn_quit
            // 
            this.btn_quit.Location = new System.Drawing.Point(527, 447);
            this.btn_quit.Name = "btn_quit";
            this.btn_quit.Size = new System.Drawing.Size(120, 38);
            this.btn_quit.TabIndex = 2;
            this.btn_quit.Text = "退出";
            this.btn_quit.UseVisualStyleBackColor = true;
            this.btn_quit.Click += new System.EventHandler(this.btn_quit_Click);
            // 
            // record_chat
            // 
            this.record_chat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.record_chat.Location = new System.Drawing.Point(219, 42);
            this.record_chat.Name = "record_chat";
            this.record_chat.ReadOnly = true;
            this.record_chat.Size = new System.Drawing.Size(494, 244);
            this.record_chat.TabIndex = 4;
            this.record_chat.Text = "";
            // 
            // list_file
            // 
            this.list_file.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.file_name,
            this.up_time,
            this.file_size,
            this.up_name});
            this.list_file.Location = new System.Drawing.Point(719, 19);
            this.list_file.Name = "list_file";
            this.list_file.Size = new System.Drawing.Size(454, 466);
            this.list_file.TabIndex = 6;
            this.list_file.UseCompatibleStateImageBehavior = false;
            this.list_file.View = System.Windows.Forms.View.Details;
            this.list_file.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.list_file_MouseDoubleClick);
            // 
            // file_name
            // 
            this.file_name.Text = "文件";
            this.file_name.Width = 134;
            // 
            // up_time
            // 
            this.up_time.Text = "更新时间";
            this.up_time.Width = 94;
            // 
            // file_size
            // 
            this.file_size.Text = "大小（字节）";
            this.file_size.Width = 102;
            // 
            // up_name
            // 
            this.up_name.Text = "上传者";
            this.up_name.Width = 97;
            // 
            // list_user
            // 
            this.list_user.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.computer_name,
            this.ip_address});
            this.list_user.Location = new System.Drawing.Point(18, 19);
            this.list_user.Name = "list_user";
            this.list_user.Size = new System.Drawing.Size(195, 466);
            this.list_user.TabIndex = 5;
            this.list_user.UseCompatibleStateImageBehavior = false;
            this.list_user.View = System.Windows.Forms.View.Details;
            // 
            // computer_name
            // 
            this.computer_name.Text = "电脑名";
            this.computer_name.Width = 90;
            // 
            // ip_address
            // 
            this.ip_address.Text = "IP地址";
            this.ip_address.Width = 86;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbUserCount);
            this.panel1.Controls.Add(this.list_user);
            this.panel1.Controls.Add(this.list_file);
            this.panel1.Controls.Add(this.record_chat);
            this.panel1.Controls.Add(this.btn_quit);
            this.panel1.Controls.Add(this.btn_upload);
            this.panel1.Controls.Add(this.btn_send);
            this.panel1.Controls.Add(this.message_chat);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1185, 512);
            this.panel1.TabIndex = 9;
            // 
            // lbUserCount
            // 
            this.lbUserCount.AutoSize = true;
            this.lbUserCount.Location = new System.Drawing.Point(18, 488);
            this.lbUserCount.Name = "lbUserCount";
            this.lbUserCount.Size = new System.Drawing.Size(71, 12);
            this.lbUserCount.TabIndex = 7;
            this.lbUserCount.Text = "在线人数：0";
            // 
            // ChatRoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 512);
            this.Controls.Add(this.panel1);
            this.Name = "ChatRoom";
            this.Text = "局域网群聊";
            this.Load += new System.EventHandler(this.ChatRoom_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox message_chat;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.LinkLabel btn_upload;
        private System.Windows.Forms.Button btn_quit;
        private System.Windows.Forms.RichTextBox record_chat;
        private System.Windows.Forms.ListView list_file;
        private System.Windows.Forms.ColumnHeader file_name;
        private System.Windows.Forms.ColumnHeader up_time;
        private System.Windows.Forms.ColumnHeader file_size;
        private System.Windows.Forms.ColumnHeader up_name;
        private System.Windows.Forms.ListView list_user;
        private System.Windows.Forms.ColumnHeader computer_name;
        private System.Windows.Forms.ColumnHeader ip_address;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbUserCount;




    }
}

