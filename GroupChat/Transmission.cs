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
using System.Diagnostics;

namespace GroupChat
{
    public partial class Transmission : Form
    {
        private string ipEnd;
        private string filePath;
        private string fileSize;


        public Transmission(string ipEnd, string filePath, string fileSize)
        {
            InitializeComponent();
            this.ipEnd = ipEnd;
            this.filePath = filePath;
            this.fileSize = fileSize;
            this.Text = "接收文件：" + this.filePath;
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == (int)MessageType.UpdateProgressBar)
            {
                if (receive_progressBar.Value < receive_progressBar.Maximum - 1)
                {
                    receive_progressBar.Value++;
                }
            }
            else if (m.Msg == (int)MessageType.FileReceiveSuccess)
            {
                receive_progressBar.Value = receive_progressBar.Maximum;
                MessageBox.Show("文件接收完毕");
                string savePath = Path.Combine(new string[] { ChatRoom.DOWNLOAD_DIR, Path.GetFileName(filePath) });
                Process.Start("explorer.exe", "/select, " + savePath);
                this.Close();
            }
            else if (m.Msg == (int)MessageType.FileReceiveError)
            {
                Win32API.My_lParam ml = new Win32API.My_lParam();
                Type t = ml.GetType();
                ml = (Win32API.My_lParam)m.GetLParam(t);
                MessageBox.Show(ml.s);
                this.Close();
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private void Transmission_Load(object sender, EventArgs e)
        {
            receive_progressBar.Minimum = 0;
            receive_progressBar.Maximum = (int)Math.Ceiling(Double.Parse(fileSize) / ChatRoom.TCP_DATA_MAX_SIZE);
            receive_progressBar.Value = 0;

            ReceiveFileClass receiveFileThread = new ReceiveFileClass(Win32API.FindWindow(null, this.Text), ipEnd, filePath);
            receiveFileThread.Start();
        }

    }
}
