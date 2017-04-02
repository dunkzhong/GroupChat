using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace GroupChat
{
    public partial class Transmission : Form
    {
        private string ipEnd;
        private string filePath;
        private string fileSize;


        public Transmission(string msgs)
        {
            InitializeComponent();
            string[] infos = msgs.Split(ChatRoom.SEPARATOR);
            this.ipEnd = infos[0];
            this.filePath = infos[1];
            this.fileSize = infos[2];
            this.Text = "接收文件：" + this.filePath;
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == (int)MessageType.UpdateProgressBar)
            {
                if (receive_progressBar.Value < receive_progressBar.Maximum-1)
                {
                    receive_progressBar.Value++;
                }
            }
            else if (m.Msg == (int)MessageType.FileReceiveSuccess)
            {
                receive_progressBar.Value = receive_progressBar.Maximum;
                MessageBox.Show("文件接收完毕");
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }


        private void Transmission_Load(object sender, EventArgs e)
        {
            string type = Path.GetExtension(this.filePath);

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.OverwritePrompt = true;
            SFD.RestoreDirectory = true;
            SFD.Filter = type + " files(*." + type + ")|*." + type + "|All files(*.*)|*.*";
            SFD.FileName = Path.GetFileName(this.filePath);



            if (SFD.ShowDialog() == DialogResult.OK)
            {
                receive_progressBar.Minimum = 0;
                receive_progressBar.Maximum = (int)Math.Ceiling(Double.Parse(fileSize) / ChatRoom.TCP_DATA_MAX_SIZE);
                receive_progressBar.Value = 0;
                msg_receive.Text = "文件存于：" + SFD.FileName;
                ReceiveFileClass receiveFileThread = new ReceiveFileClass(Win32API.FindWindow(null,this.Text), ipEnd, SFD.FileName);
                receiveFileThread.Start();
            }
            else
            {
                MessageBox.Show("取消接收文件：" + this.filePath);
                this.Close();
            }


        }

    }
}
