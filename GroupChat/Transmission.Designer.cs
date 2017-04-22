namespace GroupChat
{
    partial class Transmission
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.receive_progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // receive_progressBar
            // 
            this.receive_progressBar.Location = new System.Drawing.Point(12, 40);
            this.receive_progressBar.Name = "receive_progressBar";
            this.receive_progressBar.Size = new System.Drawing.Size(496, 23);
            this.receive_progressBar.TabIndex = 0;
            // 
            // Transmission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 115);
            this.Controls.Add(this.receive_progressBar);
            this.Name = "Transmission";
            this.Text = "传输窗口";
            this.Load += new System.EventHandler(this.Transmission_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar receive_progressBar;
    }
}