using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace CrazyDucks
{
    public partial class Form1 : Form
    {

        bool GoUp, GoDown, GoLeft, GoRight;
        bool gameOver = false;

        string face = "up";

        int playerhealth = 100;
        int playerspeed = 10;
        int ammo = 10;
        int duckspeed = 2;

        Random rNum = new Random();

        List<PictureBox> ducksList = new List<PictureBox>();
        int score;
        int highscorE;


        public Form1()
        {
            InitializeComponent();
            Restartgame();

        }
        public void playsound()
        {
            SoundPlayer sound = new SoundPlayer("./Fortnite dead sound effect.wav");
            sound.Play();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if(playerhealth>1)
            {
                healthBar.Value = playerhealth;
            }
            else
            {
                gameOver = true;
                Player.Image = Properties.Resources.deadt;
                GameTimer.Stop();
                txtgmovr.Visible = true;
                txtgmovr.BringToFront();
                playsound();
            }

            highscore.Text = "Highscore:" + highscorE;
            txtAmmo.Text = "Ammo:" + ammo;
            txtScore.Text = "Kills:" + score;


            if(GoLeft==true && Player.Left>0)
            {
                Player.Left -= playerspeed;

            }
            if(GoRight== true && Player.Left + Player.Width < this.ClientSize.Width)
            {
                Player.Left += playerspeed;
            }

            if(GoUp== true && Player.Top>36)
            {
                Player.Top -= playerspeed;
            }
            if(GoDown==true && Player.Top + Player.Height < this.ClientSize.Height)
            {
                Player.Top += playerspeed;

            }
            foreach(Control x in this.Controls)
            {
                if(x is PictureBox && (string)x.Tag == "ammo")
                {
                    if(Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if(x is PictureBox && (string)x.Tag == "duck")
                {
                    if(Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerhealth -= 1;
                    }




                    if(x.Left > Player.Left)
                    {
                        x.Left -= duckspeed;
                        ((PictureBox)x).Image = Properties.Resources.dleft2;
                    }
                    if (x.Left < Player.Left)
                    {
                        x.Left += duckspeed;
                        ((PictureBox)x).Image = Properties.Resources.dright2;
                    }
                    if (x.Top > Player.Top)
                    {
                        x.Top -= duckspeed;
                        ((PictureBox)x).Image = Properties.Resources.dup2;
                    }
                    if (x.Top < Player.Top)
                    {
                        x.Top += duckspeed;
                        ((PictureBox)x).Image = Properties.Resources.ddown2;
                            
                    }
                }
                


                foreach (Control j in this.Controls)
                {
                    if(j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "duck")

                    {
                        if(x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            ducksList.Remove(((PictureBox)x));
                            MakeDucks();
                        }
                    
                    
                    }
                }
                if (score == 12)
                    duckspeed = 3;
                if (score == 20)
                {
                    duckspeed = 3;
                    playerhealth = 100;
                    playerspeed = 15;
                    ammo = 100;
                }
                if (score == 50)
                {
                    playerspeed = 18;
                    playerhealth = 100;
                    duckspeed = 3;
                    ammo = 40;
                }
                if(score == 100)
                {
                    playerhealth= 100;
                    playerspeed = 10;
                    duckspeed = 2;
                    ammo = 10;
                    txtlevel.Text = "Level: 2";
                }
                if (score == 120)
                    duckspeed = 3;
                if (score == 140)
                {
                    duckspeed = 4;
                    playerspeed = 15;
                }
                if (score == 180)
                {
                    playerspeed = 18;
                    playerhealth = 100;
                    duckspeed = 6;
                    ammo = 80;
                }

            }

        }
        
            

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(gameOver == true)
            {
                return;
            }
            if(e.KeyCode == Keys.Left)
            {
                GoLeft = true;
                face = "left";
                Player.Image = Properties.Resources.left;
            }
            if(e.KeyCode == Keys.Right)
            {
                GoRight = true;
                face="right";
                Player.Image = Properties.Resources.right;
            }
            if(e.KeyCode== Keys.Up)
            {
                GoUp = true;
                    face = "up";
                Player.Image = Properties.Resources.up;
            }
            if(e.KeyCode== Keys.Down)
            {
                GoDown = true;
                face = "down";
                Player.Image = Properties.Resources.down;
            }


        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                GoLeft = false;
                
            }
            if (e.KeyCode == Keys.Right)
            {
                GoRight = false;
                
            }
            if (e.KeyCode == Keys.Up)
            {
                GoUp = false;
             
            }
            if (e.KeyCode == Keys.Down)
            {
                GoDown = false;
            
            }
            if(e.KeyCode== Keys.Space && ammo>0 && gameOver == false)
            {
                ammo--;
                ShootBullet(face);
                  
                if (ammo < 1)
                    DropAmmo();
            }
            if(e.KeyCode== Keys.Enter && gameOver == true)
            {
                Restartgame();
               
            }
        }
        private void ShootBullet(string direction)
        {
            Bullet shotBullet = new Bullet();
            shotBullet.directions = direction;
            shotBullet.bulletleft = Player.Left + (Player.Width/2);
            shotBullet.bullettop = Player.Top + (Player.Height/2);
            shotBullet.MakeBullet(this);
        }

      

        private void MakeDucks()
        {
            PictureBox duck = new PictureBox();
            duck.Tag = "duck";
            duck.Image = Properties.Resources.ddown2;
            duck.Left = rNum.Next(0, 900);
            duck.Top = rNum.Next(0, 800);
            duck.SizeMode = PictureBoxSizeMode.Zoom;
            ducksList.Add(duck);
            this.Controls.Add(duck);
            Player.BringToFront();

        }
        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo; 
            ammo.SizeMode = PictureBoxSizeMode.Zoom; 
            ammo.Left = rNum.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = rNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            this.Controls.Add(ammo); 
            ammo.BringToFront();
            Player.BringToFront();
        }
       
        private void Restartgame()
        {
            Player.Image = Properties.Resources.up;
            foreach (PictureBox i in ducksList)
            {
                this.Controls.Remove(i);
            }
            ducksList.Clear();
            for(int i=0;i<3;i++)
            {
                MakeDucks();
            }
            GoUp=false;
            GoDown=false;
            GoLeft=false;
            GoRight=false;
            gameOver= false;
            if(highscorE<score)
                highscorE=score;

            playerhealth = 100;
            score = 0;
            ammo = 10;
            playerspeed = 10;
            duckspeed = 2;
            txtlevel.Text = "Level: 1";
            txtgmovr.Visible = false;

            GameTimer.Start();

        }
    }
}
