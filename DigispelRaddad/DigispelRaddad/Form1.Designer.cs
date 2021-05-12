namespace Digipel
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Explosion = new AxWMPLib.AxWindowsMediaPlayer();
            this.Fire = new AxWMPLib.AxWindowsMediaPlayer();
            this.Song = new AxWMPLib.AxWindowsMediaPlayer();
            this.Crash = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.Explosion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Fire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Song)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Crash)).BeginInit();
            this.SuspendLayout();
            // 
            // Explosion
            // 
            this.Explosion.Enabled = true;
            this.Explosion.Location = new System.Drawing.Point(384, 134);
            this.Explosion.Name = "Explosion";
            this.Explosion.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Explosion.OcxState")));
            this.Explosion.Size = new System.Drawing.Size(229, 34);
            this.Explosion.TabIndex = 0;
            this.Explosion.Visible = false;
            // 
            // Fire
            // 
            this.Fire.Enabled = true;
            this.Fire.Location = new System.Drawing.Point(286, 208);
            this.Fire.Name = "Fire";
            this.Fire.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Fire.OcxState")));
            this.Fire.Size = new System.Drawing.Size(229, 34);
            this.Fire.TabIndex = 1;
            this.Fire.Visible = false;
            // 
            // Song
            // 
            this.Song.Enabled = true;
            this.Song.Location = new System.Drawing.Point(286, 248);
            this.Song.Name = "Song";
            this.Song.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Song.OcxState")));
            this.Song.Size = new System.Drawing.Size(229, 34);
            this.Song.TabIndex = 2;
            this.Song.Visible = false;
            // 
            // Crash
            // 
            this.Crash.Enabled = true;
            this.Crash.Location = new System.Drawing.Point(306, 301);
            this.Crash.Name = "Crash";
            this.Crash.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Crash.OcxState")));
            this.Crash.Size = new System.Drawing.Size(229, 34);
            this.Crash.TabIndex = 3;
            this.Crash.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Crash);
            this.Controls.Add(this.Song);
            this.Controls.Add(this.Fire);
            this.Controls.Add(this.Explosion);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Click += new System.EventHandler(this.Form1_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.Explosion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Fire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Song)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Crash)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer Explosion;
        private AxWMPLib.AxWindowsMediaPlayer Fire;
        private AxWMPLib.AxWindowsMediaPlayer Song;
        private AxWMPLib.AxWindowsMediaPlayer Crash;
    }
}

