using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace CrazyDucks
{
    class Bullet
    {
        public string directions;
        public int bulletleft;
        public int bullettop;

        private int speed = 20;
        private PictureBox bullet = new PictureBox();   
        private Timer bulletTimer = new Timer();


        public void MakeBullet(Form form)
        {
            bullet.BackColor=Color.Yellow;
            bullet.Size = new Size(5, 5);
            bullet.Tag = "bullet";
            bullet.Left = bulletleft;
            bullet.Top = bullettop;
            bullet.BringToFront();
            
            form.Controls.Add(bullet);


            bulletTimer.Interval = speed;
            bulletTimer.Tick += new EventHandler(BulletTimerEvent);
            bulletTimer.Start();
        }

        private void BulletTimerEvent(object sender, EventArgs e)
        {
           if(directions=="left")
            {
                bullet.Left -= speed;

            }
           if(directions=="right")
            {
                bullet.Left += speed;
            }
           if(directions == "up")
            {
                bullet.Top-= speed;
            }
           if(directions== "down")
            {
                bullet.Top += speed;
            }


           if(bullet.Left<10 || bullet.Left>860 || bullet.Top<10 || bullet.Top>600)
            {
                bulletTimer.Stop();
                bulletTimer.Dispose();
                bullet.Dispose();
                bulletTimer = null;
                bullet=null;



            }
        }


    }
}
