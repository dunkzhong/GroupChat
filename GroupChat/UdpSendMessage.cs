using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace GroupChat
{
    class UdpSendMessage
    {
        private static UdpClient bcUdpClient = new UdpClient();
        private static IPEndPoint bcIPEndPoint = new IPEndPoint(IPAddress.Broadcast, ChatRoom.LISTEN_PORT);

        private UdpSendMessage() { }

        public static void SendToAll(string msg)
        {
            byte[] buff = Encoding.Default.GetBytes(msg);
            bcUdpClient.Send(buff, buff.Length, bcIPEndPoint);
        }

        public static void SendToOne(string ipReply, string msg)
        {
            IPEndPoint EPReply = new IPEndPoint(IPAddress.Parse(ipReply), ChatRoom.LISTEN_PORT);
            byte[] buff = Encoding.Default.GetBytes(msg);
            bcUdpClient.Send(buff, buff.Length, EPReply);
        }
    }
}
