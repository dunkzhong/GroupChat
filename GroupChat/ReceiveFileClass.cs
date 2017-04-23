using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GroupChat
{
    class ReceiveFileClass
    {
        IntPtr receiveIntPtr;
        string ipEnd;
        string filePath;

        public ReceiveFileClass(IntPtr receiveIntPtr, string ipEnd, string filePath)
        {
            this.receiveIntPtr = receiveIntPtr;
            this.ipEnd = ipEnd;
            this.filePath = filePath;
        }

        public void Start()
        {
            Thread receiveFile = new Thread(new ThreadStart(run));
            receiveFile.IsBackground = true;
            receiveFile.Start();
        }

        void run()
        {
            Socket socketReceiveFile = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipSend = new IPEndPoint(IPAddress.Parse(this.ipEnd), ChatRoom.FILE_PORT);
            try
            {
                socketReceiveFile.Connect(ipSend);

                byte[] tmp1 = Encoding.Default.GetBytes(filePath);
                socketReceiveFile.Send(tmp1, 0, tmp1.Length, SocketFlags.None);

                byte[] tmp2 = new byte[ChatRoom.UDP_DATA_MAX_SIZE];
                socketReceiveFile.Receive(tmp2);

                string ackMessage = Encoding.Default.GetString(tmp2);

                if (ackMessage.CompareTo("有效文件") == 0)
                {
                    string fileSavePath = ChatRoom.DOWNLOAD_DIR;
                    if (!Directory.Exists(fileSavePath))
                    {
                        Directory.CreateDirectory(fileSavePath);
                    }

                    string fileName = Path.Combine(new string[] { fileSavePath, Path.GetFileName(filePath) });
                    FileStream FS = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);

                    byte[] Buff = new byte[ChatRoom.TCP_DATA_MAX_SIZE];
                    int len;
                    while ((len = socketReceiveFile.Receive(Buff)) != 0)
                    {
                        Win32API.PostMessage(receiveIntPtr, (int)MessageType.UpdateProgressBar, 0, 0);
                        FS.Write(Buff, 0, len);
                    }
                    FS.Flush();
                    FS.Close();
                    Win32API.PostMessage(receiveIntPtr, (int)MessageType.FileReceiveSuccess, 0, 0);


                }
                else
                {
                    Win32API.My_lParam lp = new Win32API.My_lParam();
                    lp.s = ackMessage;
                    Win32API.SendMessage(receiveIntPtr, (int)MessageType.FileReceiveError, 0, ref lp);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            finally
            {
                socketReceiveFile.Close();
            }
        }
    }
}
