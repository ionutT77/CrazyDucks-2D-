namespace CrazyDucks
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
            this.components = new System.ComponentModel.Container();
            this.txtAmmo = new System.Windows.Forms.Label();
            this.txtScore = new System.Windows.Forms.Label();
            this.txtHealth = new System.Windows.Forms.Label();
            this.healthBar = new System.Windows.Forms.ProgressBar();
            this.GameTimer = new System.Windows.Forms.Timer(this.components);
            this.Player = new System.Windows.Forms.PictureBox();
            this.highscore = new System.Windows.Forms.Label();
            this.txtlevel = new System.Windows.Forms.Label();
            this.txtgmovr = new System.Windows.Forms.Label();
            this.BulletCooldown = new System.Windows.Forms.Label();
            this.BulletCooldownBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.Player)).BeginInit();
            this.SuspendLayout();
            // 
            // txtAmmo
            // 
            this.txtAmmo.AutoSize = true;
            this.txtAmmo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAmmo.ForeColor = System.Drawing.Color.White;
            this.txtAmmo.Location = new System.Drawing.Point(16, 11);
            this.txtAmmo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtAmmo.Name = "txtAmmo";
            this.txtAmmo.Size = new System.Drawing.Size(107, 29);
            this.txtAmmo.TabIndex = 0;
            this.txtAmmo.Text = "Ammo: 0";
            // 
            // txtScore
            // 
            this.txtScore.AutoSize = true;
            this.txtScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScore.ForeColor = System.Drawing.Color.White;
            this.txtScore.Location = new System.Drawing.Point(277, 11);
            this.txtScore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtScore.Name = "txtScore";
            this.txtScore.Size = new System.Drawing.Size(84, 29);
            this.txtScore.TabIndex = 1;
            this.txtScore.Text = "Kills: 0";
            // 
            // txtHealth
            // 
            this.txtHealth.AutoSize = true;
            this.txtHealth.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHealth.ForeColor = System.Drawing.Color.White;
            this.txtHealth.Location = new System.Drawing.Point(995, 11);
            this.txtHealth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtHealth.Name = "txtHealth";
            this.txtHealth.Size = new System.Drawing.Size(88, 29);
            this.txtHealth.TabIndex = 2;
            this.txtHealth.Text = "Health:";
            // 
            // healthBar
            // 
            this.healthBar.Location = new System.Drawing.Point(1091, 15);
            this.healthBar.Margin = new System.Windows.Forms.Padding(4);
            this.healthBar.Name = "healthBar";
            this.healthBar.Size = new System.Drawing.Size(133, 25);
            this.healthBar.TabIndex = 3;
            this.healthBar.Value = 100;
            // 
            // GameTimer
            // 
            this.GameTimer.Enabled = true;
            this.GameTimer.Interval = 20;
            this.GameTimer.Tick += new System.EventHandler(this.MainTimerEvent);
            // 
            // Player
            // 
            this.Player.Image = global::CrazyDucks.Properties.Resources.up;
            this.Player.Location = new System.Drawing.Point(555, 506);
            this.Player.Margin = new System.Windows.Forms.Padding(4);
            this.Player.Name = "Player";
            this.Player.Size = new System.Drawing.Size(71, 100);
            this.Player.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.Player.TabIndex = 4;
            this.Player.TabStop = false;
            // 
            // highscore
            // 
            this.highscore.AutoSize = true;
            this.highscore.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.highscore.ForeColor = System.Drawing.Color.White;
            this.highscore.Location = new System.Drawing.Point(121, 11);
            this.highscore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.highscore.Name = "highscore";
            this.highscore.Size = new System.Drawing.Size(148, 29);
            this.highscore.TabIndex = 5;
            this.highscore.Text = "Highscore: 0";
            // 
            // txtlevel
            // 
            this.txtlevel.AutoSize = true;
            this.txtlevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtlevel.ForeColor = System.Drawing.Color.White;
            this.txtlevel.Location = new System.Drawing.Point(786, 11);
            this.txtlevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtlevel.Name = "txtlevel";
            this.txtlevel.Size = new System.Drawing.Size(96, 29);
            this.txtlevel.TabIndex = 6;
            this.txtlevel.Text = "Level: 1";
            // 
            // txtgmovr
            // 
            this.txtgmovr.AutoSize = true;
            this.txtgmovr.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtgmovr.ForeColor = System.Drawing.Color.Red;
            this.txtgmovr.Location = new System.Drawing.Point(464, 39);
            this.txtgmovr.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.txtgmovr.Name = "txtgmovr";
            this.txtgmovr.Size = new System.Drawing.Size(248, 52);
            this.txtgmovr.TabIndex = 7;
            this.txtgmovr.Text = "Game Over";
            this.txtgmovr.Visible = false;
            // 
            // BulletCooldown
            // 
            this.BulletCooldown.AutoSize = true;
            this.BulletCooldown.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BulletCooldown.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.BulletCooldown.Location = new System.Drawing.Point(368, 11);
            this.BulletCooldown.Name = "BulletCooldown";
            this.BulletCooldown.Size = new System.Drawing.Size(196, 29);
            this.BulletCooldown.TabIndex = 8;
            this.BulletCooldown.Text = "Bullet Cooldown:";
            // 
            // BulletCooldownBar
            // 
            this.BulletCooldownBar.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.BulletCooldownBar.Location = new System.Drawing.Point(570, 15);
            this.BulletCooldownBar.Name = "BulletCooldownBar";
            this.BulletCooldownBar.Size = new System.Drawing.Size(105, 23);
            this.BulletCooldownBar.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(1227, 706);
            this.Controls.Add(this.BulletCooldownBar);
            this.Controls.Add(this.BulletCooldown);
            this.Controls.Add(this.txtgmovr);
            this.Controls.Add(this.txtlevel);
            this.Controls.Add(this.highscore);
            this.Controls.Add(this.Player);
            this.Controls.Add(this.healthBar);
            this.Controls.Add(this.txtHealth);
            this.Controls.Add(this.txtScore);
            this.Controls.Add(this.txtAmmo);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "CrazyDucks";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyIsDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyIsUp);
            ((System.ComponentModel.ISupportInitialize)(this.Player)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtAmmo;
        private System.Windows.Forms.Label txtScore;
        private System.Windows.Forms.Label txtHealth;
        private System.Windows.Forms.ProgressBar healthBar;
        private System.Windows.Forms.PictureBox Player;
        private System.Windows.Forms.Timer GameTimer;
        private System.Windows.Forms.Label highscore;
        private System.Windows.Forms.Label txtlevel;
        private System.Windows.Forms.Label txtgmovr;
        private System.Windows.Forms.Label BulletCooldown;
        private System.Windows.Forms.ProgressBar BulletCooldownBar;
    }
}

