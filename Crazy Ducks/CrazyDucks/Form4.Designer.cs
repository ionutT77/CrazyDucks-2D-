namespace CrazyDucks
{
    partial class Form4
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
            this.sniper = new System.Windows.Forms.Button();
            this.pistol = new System.Windows.Forms.Button();
            this.riffle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sniper
            // 
            this.sniper.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sniper.Location = new System.Drawing.Point(555, 96);
            this.sniper.Name = "sniper";
            this.sniper.Size = new System.Drawing.Size(173, 81);
            this.sniper.TabIndex = 0;
            this.sniper.Text = "SNIPER";
            this.sniper.UseVisualStyleBackColor = true;
            // 
            // pistol
            // 
            this.pistol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pistol.Location = new System.Drawing.Point(322, 96);
            this.pistol.Name = "pistol";
            this.pistol.Size = new System.Drawing.Size(145, 81);
            this.pistol.TabIndex = 1;
            this.pistol.Text = "PISTOL";
            this.pistol.UseVisualStyleBackColor = true;
            // 
            // riffle
            // 
            this.riffle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.riffle.Location = new System.Drawing.Point(75, 96);
            this.riffle.Name = "riffle";
            this.riffle.Size = new System.Drawing.Size(161, 81);
            this.riffle.TabIndex = 2;
            this.riffle.Text = "RIFFLE";
            this.riffle.UseVisualStyleBackColor = true;
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.riffle);
            this.Controls.Add(this.pistol);
            this.Controls.Add(this.sniper);
            this.Name = "Form4";
            this.Text = "Form4";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button sniper;
        private System.Windows.Forms.Button pistol;
        private System.Windows.Forms.Button riffle;
    }
}