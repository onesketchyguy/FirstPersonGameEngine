using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FirstPersonGameEngine
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.display = new PictureBox();
            ((ISupportInitialize)(this.display)).BeginInit();
            this.SuspendLayout();
            // 
            // display
            // 
            this.display.Dock = DockStyle.Fill;
            this.display.Location = new Point(0, 0);
            this.display.Name = "mainImage";
            this.display.Size = new Size(640, 640);
            this.display.TabIndex = 0;
            this.display.TabStop = false;
            this.display.BackColor = Color.Empty;
            // 
            // Main Forum
            // 
            this.ClientSize = new Size(640, 640);
            this.MaximumSize = new Size(1280, 1280);
            this.MinimumSize = new Size(320, 320);
            this.AutoSize = true;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.Controls.Add(this.display);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Game";
            this.ShowIcon = false;
            ((ISupportInitialize)(this.display)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private PictureBox display;
    }
}

