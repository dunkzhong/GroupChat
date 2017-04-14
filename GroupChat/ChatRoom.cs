using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace GroupChat
{
    enum MessageType
    {
        WM_USER = 0x0400,//系统自定义消息0x0000~0x0400 
        Broadcast, BroadcastReply, ChatMessage, FileMessage, FileRequest, FileRequestReply, ClearUsers, UpdateProgressBar, FileReceiveSuccess
    }

    public partial class ChatRoom : Form
    {
        
        public static readonly int UDP_DATA_MAX_SIZE = 1024;
        public static readonly int TCP_DATA_MAX_SIZE = 10 * 1024 * 1024;
        public static readonly int TCP_LISTEN_MAX_SIZE = 1024;
        public static readonly int LISTEN_PORT = 2048;
        public static readonly int FILE_PORT = 2049;
        public static readonly char SEPARATOR = '`';
        public static readonly string WINDOWS_NAME = "局域网群聊";
        public static readonly string COMPUTER_NAME = Dns.GetHostName();
        public static readonly string IP_ADDRESS = GetInternalIP();


        //获取内网IP
        public static string GetInternalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private static ChatRoom chatRoom;
        private static readonly object locker = new object();

        private static Socket sendFileSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static IPEndPoint tcpPoint = new IPEndPoint(IPAddress.Parse(IP_ADDRESS), FILE_PORT);

        public static Socket fileAcceptSocket()
        {
            return sendFileSocket.Accept();
        }

        private ChatRoom()
        {
            InitializeComponent();
            sendFileSocket.Bind(tcpPoint);
            sendFileSocket.Listen(TCP_LISTEN_MAX_SIZE);
        }

        public static ChatRoom GetRoom()
        {
            //多线程同时运行到这里，会同时通过这个判断条件执行条件内的代码
            if (chatRoom == null)
            {
                //多线程同时运行到这里后，只能有一个线程通过lock锁，其他线程会被挂起
                lock (locker)
                {
                    // 再次判断如果类的实例是否创建，如果不存在则实例化，反之就直接输出类的实例
                    if (chatRoom == null)
                    {
                        chatRoom = new ChatRoom();

                    }
                }
            }
            return chatRoom;
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch ((MessageType)m.Msg)
            {
                case MessageType.Broadcast:
                    {
                        Win32API.My_lParam ml = new Win32API.My_lParam();
                        Type t = ml.GetType();
                        ml = (Win32API.My_lParam)m.GetLParam(t);
                        updateUserList(ml.s);
                    }
                    break;

                case MessageType.BroadcastReply:
                    {
                        Win32API.My_lParam ml = new Win32API.My_lParam();
                        Type t = ml.GetType();
                        ml = (Win32API.My_lParam)m.GetLParam(t);
                        updateUserList(ml.s);
                    }
                    break;

                case MessageType.ChatMessage:
                    {
                        Win32API.My_lParam ml = new Win32API.My_lParam();
                        Type t = ml.GetType();
                        ml = (Win32API.My_lParam)m.GetLParam(t);
                        string[] msg = ml.s.Split(SEPARATOR);

                        string owner = msg[0] + "(" + msg[1] + ")";
                        string updateTime = DateTime.Now.ToLongTimeString();
                        string data = msg[2];
                        string str = owner;
                        str += updateTime + Environment.NewLine;
                        str += data + Environment.NewLine;
                        str += Environment.NewLine;
                        record_chat.AppendText(str);
                    }
                    break;

                case MessageType.FileMessage:
                    {
                        Win32API.My_lParam ml = new Win32API.My_lParam();
                        Type t = ml.GetType();
                        ml = (Win32API.My_lParam)m.GetLParam(t);

                        string[] msg = ml.s.Split(SEPARATOR);
                        string owner = msg[0] + "(" + msg[1] + ")";
                        string updateTime = DateTime.Now.ToLongTimeString();
                        string filePath = msg[2];
                        string fileSize = msg[3];

                        string str = owner;
                        str += updateTime + Environment.NewLine;
                        str += "发送文件：" + Path.GetFileName(filePath);
                        str += " 大小(字节)：" + fileSize;
                        str += Environment.NewLine;
                        str += Environment.NewLine;

                        record_chat.AppendText(str);

                        updateFileList(filePath, updateTime, fileSize, msg[1]);
                    }
                    break;


                case MessageType.FileRequestReply:
                    {
                        Win32API.My_lParam ml = new Win32API.My_lParam();
                        Type t = ml.GetType();
                        ml = (Win32API.My_lParam)m.GetLParam(t);
                        Transmission receiveFile = new Transmission(ml.s);
                        receiveFile.Show();
                    }
                    break;

                case MessageType.ClearUsers:
                    {
                        list_user.Items.Clear();
                        break;
                    }

                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }



        private void updateUserList(string msg)
        {
            string[] user = msg.Split(SEPARATOR);

            ListViewItem lviComputerName = new ListViewItem();
            ListViewItem.ListViewSubItem lviIP = new ListViewItem.ListViewSubItem();

            lviComputerName.Text = user[0];
            lviIP.Text = user[1];

            lviComputerName.SubItems.Add(lviIP);

            bool flag = true;
            for (int i = 0; i < this.list_user.Items.Count; i++)
            {
                if (lviIP.Text == this.list_user.Items[i].SubItems[1].Text)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                this.list_user.Items.Add(lviComputerName);
            }

            lbUserCount.Text = "在线人数：  " + this.list_user.Items.Count;
        }

        private void updateFileList(string filePath, string updateTime, string fileSize, string owner)
        {

            ListViewItem lviFilePath = new ListViewItem();
            ListViewItem.ListViewSubItem lviUpdateTime = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem lviFileSize = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem lviOwner = new ListViewItem.ListViewSubItem();

            lviFilePath.Text = filePath;
            lviUpdateTime.Text = updateTime;
            lviFileSize.Text = fileSize;
            lviOwner.Text = owner;

            lviFilePath.SubItems.Add(lviUpdateTime);
            lviFilePath.SubItems.Add(lviFileSize);
            lviFilePath.SubItems.Add(lviOwner);

            bool flag = true;
            for (int i = 0; i < this.list_user.Items.Count; i++)
            {
                if (lviFilePath.Text == this.list_user.Items[i].SubItems[0].Text && lviOwner.Text == this.list_user.Items[i].SubItems[3].Text)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                this.list_file.Items.Add(lviFilePath);
            }

        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChatRoom_Load(object sender, EventArgs e)
        {
            this.Text = WINDOWS_NAME;
            ControlMessage();
            UpdateFriends();
        }

        private void ControlMessage()
        {
            Thread listenThread = new Thread(new ThreadStart(ListenClass.StartListen));
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void UpdateFriends()
        {
            Thread listenThread = new Thread(new ThreadStart(Broadcast.IntervalBroadcast));
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            string data = message_chat.Text.Trim();
            if (data == String.Empty)
            {
                MessageBox.Show("发送不能为空！");
            }
            else
            {
                string info = string.Join(SEPARATOR + "", new string[] { MessageType.ChatMessage + "", COMPUTER_NAME, IP_ADDRESS, data });
                UdpSendMessage.SendToAll(info);
            }
            message_chat.Clear();
            message_chat.Focus();
        }

        private void btn_upload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            FileInfo FI;
            Dlg.Filter = "所有文件(*.*)|*.*";
            Dlg.CheckFileExists = true;

            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                FI = new FileInfo(Dlg.FileName);
                string info = string.Join(SEPARATOR + "", new string[] { MessageType.FileMessage + "", COMPUTER_NAME, IP_ADDRESS, Dlg.FileName, FI.Length + "" });
                UdpSendMessage.SendToAll(info);
            }
            else
            {
                MessageBox.Show("取消上传");
            }
        }

        private void list_file_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (list_file.SelectedItems.Count == 1)
            {
                DialogResult result = MessageBox.Show("确定接收文件（"+list_file.SelectedItems[0].Text+"）吗？","tips",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string ipAddress = list_file.SelectedItems[0].SubItems[3].Text;
                    string filePath = list_file.SelectedItems[0].SubItems[0].Text;
                    string fileSize = list_file.SelectedItems[0].SubItems[2].Text;
                    
                    string info = string.Join(SEPARATOR+"",new string[]{MessageType.FileRequest+"",IP_ADDRESS,filePath,fileSize});
                    UdpSendMessage.SendToOne(ipAddress,info);
                }
                else
                {
                    MessageBox.Show("取消接收");
                }
            }
            else
            {
                MessageBox.Show("应选且仅选一个文件");
            }
        }

    

    }
}
