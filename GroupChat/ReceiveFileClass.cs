using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

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
            socketReceiveFile.Connect(ipSend);
            byte[] Buff = new byte[ChatRoom.TCP_DATA_MAX_SIZE];
            int len;
            FileStream FS = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);

            while ((len = socketReceiveFile.Receive(Buff)) != 0)
            {
                Win32API.PostMessage(receiveIntPtr,(int)MessageType.UpdateProgressBar,0,0);
                FS.Write(Buff, 0, len);
            }
            FS.Flush();
            FS.Close();
            Win32API.PostMessage(receiveIntPtr, (int)MessageType.FileReceiveSuccess, 0, 0);
            socketReceiveFile.Close();
        }
    }
}
