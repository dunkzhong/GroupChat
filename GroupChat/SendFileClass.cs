using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace GroupChat
{
    class SendFileClass
    {
        private SendFileClass() { }

        public static void SendFile(object filePath)
        {
            int len = 0;
            byte[] buff = new byte[ChatRoom.TCP_DATA_MAX_SIZE];
            string sendFilePath = (string)filePath;
            Socket sendFileAccept = ChatRoom.fileAcceptSocket();
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
