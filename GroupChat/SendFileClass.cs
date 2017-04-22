using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace GroupChat
{
    class SendFileClass
    {
       
        private SendFileClass() { }

        public static void Initialize(Socket sendFileSocket,IPEndPoint tcpPoint)
        {
            sendFileSocket.Bind(tcpPoint);
            sendFileSocket.Listen(ChatRoom.TCP_LISTEN_MAX_SIZE);
            Thread acceptThread = new Thread(new ParameterizedThreadStart(FileAccept));
            acceptThread.IsBackground = true;
            acceptThread.Start(sendFileSocket);
        }

        private static void FileAccept(object obj)
        {
            while (true)
            {
                Socket sendFileAccept = ((Socket)obj).Accept();
                byte[] tmp1 = new byte[ChatRoom.TCP_DATA_MAX_SIZE];
                sendFileAccept.Receive(tmp1);
                string filePath = Encoding.Default.GetString(tmp1);
                try
                {
                    Win32API.My_lParam lp = new Win32API.My_lParam();
                    lp.t = sendFileAccept;
                    lp.s = filePath;
                    Win32API.SendMessage(Win32API.FindWindow(null, ChatRoom.WINDOWS_NAME), (int)MessageType.FileRequest, 0, ref lp);
                }
                catch (Exception e)
                {
                    sendFileAccept.Send(Encoding.Default.GetBytes(e.Message), SocketFlags.None);
                    sendFileAccept.Close();
                }
                
            }
        }

        public static void SendFile(object socketAndPath)
        {
            int len = 0;
            byte[] buff = new byte[ChatRoom.TCP_DATA_MAX_SIZE];

            Socket sendFileAccept = ((Win32API.My_lParam)socketAndPath).t;
            string sendFilePath = ((Win32API.My_lParam)socketAndPath).s;

            FileStream FS = new FileStream(sendFilePath, FileMode.Open, FileAccess.Read);

            while ((len = FS.Read(buff, 0, ChatRoom.TCP_DATA_MAX_SIZE)) != 0)
            {
                sendFileAccept.Send(buff, 0, len, SocketFlags.None);
            }

            sendFileAccept.Close();
            FS.Close();
        }
    }
}
