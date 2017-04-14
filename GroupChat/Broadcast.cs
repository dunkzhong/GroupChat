using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GroupChat
{
    class Broadcast
    {
        public static readonly int BROADCAST_INTERVAL = 10000;

        private Broadcast() { }

        public static void IntervalBroadcast()
        {
            IntPtr charRoom = Win32API.FindWindow(null, "局域网群聊");
            string info = string.Join(ChatRoom.SEPARATOR + "", new string[] { MessageType.Broadcast + "", ChatRoom.COMPUTER_NAME, ChatRoom.IP_ADDRESS });
            while (true)
            {
                Win32API.SendMessage(charRoom, (int)MessageType.ClearUsers, 0, 0);
                UdpSendMessage.SendToAll(info);
                Thread.Sleep(BROADCAST_INTERVAL);
            }
        }
    }
}
