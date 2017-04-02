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
        public static readonly int BROADCAST_INTERVAL = 10000;
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
            Thread listenThread = new Thread(new ThreadStart(StartListen));
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        /**
         *  Broadcast:cpname+ip
         *  BroadcastReply:cpname+ip
         *  ChatMessage:cpname+ip+data
         *  FileMessage:cpname+ip+path+size
         *  FileRequest:ip+path+size
         *  FileRequestReply:ip+path+size
         */
        private void StartListen()
        {
            UdpClient udpClient = new UdpClient(LISTEN_PORT);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
            IntPtr charRoom = Win32API.FindWindow(null, WINDOWS_NAME);

            while (true)
            {
                byte[] buff = udpClient.Receive(ref ipEndPoint);
                string tmpInfo = Encoding.Default.GetString(buff);

                string[] infos = tmpInfo.Split(SEPARATOR);
                MessageType messageType = (MessageType)Enum.Parse(typeof(MessageType), infos[0]);

                switch (messageType)
                {
                    case MessageType.Broadcast:
                        {
                            string computerName = infos[1];
                            string ipAddress = infos[2];
                            Win32API.My_lParam lp = new Win32API.My_lParam();
                            lp.s = string.Join(SEPARATOR + "", new string[] { computerName, ipAddress });
                            Win32API.SendMessage(charRoom, (int)MessageType.Broadcast, 0, ref lp);
                            string myInfo = string.Join(SEPARATOR + "", new string[] { MessageType.BroadcastReply + "", COMPUTER_NAME, IP_ADDRESS });
                            UdpSendMessage.SendToOne(ipAddress, myInfo);
                        }
                        break;

                    case MessageType.BroadcastReply:
                        {
                            string computerName = infos[1];
                            string ipAddress = infos[2];
                            Win32API.My_lParam lp = new Win32API.My_lParam();
                            lp.s = string.Join(SEPARATOR + "", new string[] { computerName, ipAddress });
                            Win32API.SendMessage(charRoom, (int)MessageType.BroadcastReply, 0, ref lp);
                        }
                        break;

                    case MessageType.ChatMessage:
                        {
                            string computerName = infos[1];
                            string ipAddress = infos[2];
                            string chatData = infos[3];
                            Win32API.My_lParam lp = new Win32API.My_lParam();
                            lp.s = string.Join(SEPARATOR + "", new string[] { computerName, ipAddress, chatData });
                            Win32API.SendMessage(charRoom, (int)MessageType.ChatMessage, 0, ref lp);
                        }
                        break;

                    case MessageType.FileMessage:
                        {
                            string computerName = infos[1];
                            string ipAddress = infos[2];
                            string filePath = infos[3];
                            string fileSize = infos[4];
                            Win32API.My_lParam lp = new Win32API.My_lParam();
                            lp.s = string.Join(SEPARATOR + "", new string[] { computerName, ipAddress, filePath, fileSize });
                            Win32API.SendMessage(charRoom, (int)MessageType.FileMessage, 0, ref lp);
                        }
                        break;

                    case MessageType.FileRequest:
                        {
                            string ipAddress = infos[1];
                            string filePath = infos[2];
                            string fileSize = infos[3];
                            if (File.Exists(filePath))
                            {
                                Thread sendFileThread = new Thread(new ParameterizedThreadStart(SendFile));
                                sendFileThread.Start(filePath);
                            }
                            else
                            {
                                fileSize = "0";
                            }

                            string myInfo = string.Join(SEPARATOR + "", new string[] { MessageType.FileRequestReply + "", IP_ADDRESS, filePath, fileSize });
                            UdpSendMessage.SendToOne(ipAddress, myInfo);
                        }
                        break;

                    case MessageType.FileRequestReply:
                        {
                            string ipAddress = infos[1];
                            string filePath = infos[2];
                            string fileSize = infos[3];
                            if (fileSize == "0")
                            {
                                MessageBox.Show("文件(" + filePath + ")已不存在，无法传输");
                            }
                            else
                            {
                                Win32API.My_lParam lp = new Win32API.My_lParam();
                                lp.s = string.Join(SEPARATOR + "", new string[] { ipAddress, filePath, fileSize });
                                Win32API.SendMessage(charRoom, (int)MessageType.FileRequestReply, 0, ref lp);
                            }
                        }
                        break;

                    default:

                        break;
                }
            }
        }

        private void SendFile(object filePath)
        {
            int len = 0;
            byte[] buff = new byte[TCP_DATA_MAX_SIZE];
            string sendFilePath = (string)filePath;
            Socket sendFileAccept = sendFileSocket.Accept();
            FileStream FS = new FileStream(sendFilePath, FileMode.Open, FileAccess.Read);

            while ((len = FS.Read(buff, 0, TCP_DATA_MAX_SIZE)) != 0)
            {
                sendFileAccept.Send(buff, 0, len, SocketFlags.None);
            }

            sendFileAccept.Close();
            FS.Close();
        }

        private void UpdateFriends()
        {
            Thread listenThread = new Thread(new ThreadStart(IntervalBroadcast));
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void IntervalBroadcast()
        {
            IntPtr charRoom = Win32API.FindWindow(null, "局域网群聊");
            string info = string.Join(SEPARATOR+"",new string[]{MessageType.Broadcast+"",COMPUTER_NAME,IP_ADDRESS});
            while (true)
            {
                Win32API.SendMessage(charRoom,(int)MessageType.ClearUsers,0,0);
                UdpSendMessage.SendToAll(info);
                Thread.Sleep(BROADCAST_INTERVAL);
            }
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
            //MessageBox.Show(list_file.SelectedItems[0].SubItems[1].Text);
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
