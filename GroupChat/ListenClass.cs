using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace GroupChat
{
    class ListenClass
    {
        private ListenClass() { }

        public static void StartListen()
        {
            UdpClient udpClient = new UdpClient(ChatRoom.LISTEN_PORT);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
            IntPtr charRoom = Win32API.FindWindow(null, ChatRoom.WINDOWS_NAME);

            while (true)
            {
                byte[] buff = udpClient.Receive(ref ipEndPoint);
                string tmpInfo = Encoding.Default.GetString(buff);

                string[] infos = tmpInfo.Split(ChatRoom.SEPARATOR);
                MessageType messageType = (MessageType)Enum.Parse(typeof(MessageType), infos[0]);

                switch (messageType)
                {
                    case MessageType.Broadcast:
                        {
                            string computerName = infos[1];
                            string ipAddress = infos[2];
                            Win32API.My_lParam lp = new Win32API.My_lParam();
                            lp.s = string.Join(ChatRoom.SEPARATOR + "", new string[] { computerName, ipAddress });
                            Win32API.SendMessage(charRoom, (int)MessageType.Broadcast, 0, ref lp);
                            string myInfo = string.Join(ChatRoom.SEPARATOR + "", new string[] { MessageType.BroadcastReply + "", ChatRoom.COMPUTER_NAME, ChatRoom.IP_ADDRESS });
                            UdpSendMessage.SendToOne(ipAddress, myInfo);
                        }
                        break;

                    case MessageType.BroadcastReply:
                        {
                            string computerName = infos[1];
                            string ipAddress = infos[2];
                            Win32API.My_lParam lp = new Win32API.My_lParam();
                            lp.s = string.Join(ChatRoom.SEPARATOR + "", new string[] { computerName, ipAddress });
                            Win32API.SendMessage(charRoom, (int)MessageType.BroadcastReply, 0, ref lp);
                        }
                        break;

                    case MessageType.ChatMessage:
                        {
                            string computerName = infos[1];
                            string ipAddress = infos[2];
                            string chatData = infos[3];
                            Win32API.My_lParam lp = new Win32API.My_lParam();
                            lp.s = string.Join(ChatRoom.SEPARATOR + "", new string[] { computerName, ipAddress, chatData });
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
                            lp.s = string.Join(ChatRoom.SEPARATOR + "", new string[] { computerName, ipAddress, filePath, fileSize });
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
                                Thread sendFileThread = new Thread(new ParameterizedThreadStart(SendFileClass.SendFile));
                                sendFileThread.Start(filePath);
                            }
                            else
                            {
                                fileSize = "0";
                            }

                            string myInfo = string.Join(ChatRoom.SEPARATOR + "", new string[] { MessageType.FileRequestReply + "", ChatRoom.IP_ADDRESS, filePath, fileSize });
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
                                lp.s = string.Join(ChatRoom.SEPARATOR + "", new string[] { ipAddress, filePath, fileSize });
                                Win32API.SendMessage(charRoom, (int)MessageType.FileRequestReply, 0, ref lp);
                            }
                        }
                        break;

                    default:

                        break;
                }
            }
        }

        

    }
}
