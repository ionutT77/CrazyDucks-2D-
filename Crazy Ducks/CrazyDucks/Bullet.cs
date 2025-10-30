using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

//for bullet direction implement all 4 directions
//implement as bullet travels , it shoul shrink and disappear
//implemet diagonal movement for bullet and player, so when i press 2keys together it should move diagonally
//take in consideration speed of bullet and player

//CHECKED


namespace CrazyDucks
{
    class Bullet
    {
        // caller sets this to something like "left", "up", "up+left", "right down", etc.
        public string directions;
        public int bulletleft;
        public int bullettop;
        // added public fields for right and down (bottom)
        public int bulletright;
        public int bulletdown;

        // tick interval (ms) and movement speed (pixels per tick).
        // kept private so callers that set bulletleft/bullettop don't need to change anything.
        private int tickInterval = 20;
        private int moveSpeed = 12;

        // shrink settings:
        // shrinkRate = pixels to shrink when a shrink happens
        // shrinkEvery = how many ticks between shrink steps (increase to slow shrink).
        // Previously set to 3 (3x lifetime). Increased to 6 to make the bullet last one more time (≈6x lifetime).
        private int shrinkRate = 1;
        private int shrinkEvery = 6; // increased from 3 -> 6 to further slow shrinking
        private int shrinkCounter = 0;
        
        // Weapon-specific properties
        private bool shouldShrink = true; // Sniper bullets don't shrink

        private PictureBox bullet = new PictureBox();
        private Timer bulletTimer = new Timer();
        private Form parentForm; // Store reference to parent form for boundary checking


        public void MakeBullet(Form form)
        {
            parentForm = form; // Store form reference
            
            // Get weapon-specific properties from WeaponManager
            moveSpeed = WeaponManager.GetBulletSpeed();
            shouldShrink = WeaponManager.DoesWeaponShrink();
            
            bullet.BackColor = Color.Yellow;
            bullet.Size = new Size(8, 8);
            bullet.Tag = "bullet";
            bullet.Left = bulletleft;
            bullet.Top = bullettop;
            bullet.BringToFront();

            // initialize right/down based on size
            bulletright = bullet.Left + bullet.Width;
            bulletdown = bullet.Top + bullet.Height;

            form.Controls.Add(bullet);

            bulletTimer.Interval = tickInterval;
            bulletTimer.Tick += new EventHandler(BulletTimerEvent);
            bulletTimer.Start();
        }

        private void BulletTimerEvent(object sender, EventArgs e)
        {
            if (bullet == null)
                return;

            // Interpret directions string (case-insensitive) and allow multiple directions
            // e.g. "up left", "up+left", "upleft" etc. We simply check for the keywords.
            string dir = directions ?? string.Empty;
            dir = dir.ToLowerInvariant();

            double vx = 0.0;
            double vy = 0.0;
            if (dir.Contains("left")) vx -= 1.0;
            if (dir.Contains("right")) vx += 1.0;
            if (dir.Contains("up")) vy -= 1.0;
            if (dir.Contains("down")) vy += 1.0;

            // If any direction present, normalize so diagonal speed ~= straight speed
            if (vx != 0.0 || vy != 0.0)
            {
                double len = Math.Sqrt(vx * vx + vy * vy);
                if (len == 0) len = 1;
                vx = vx / len * moveSpeed;
                vy = vy / len * moveSpeed;

                // apply movement (round to nearest pixel)
                int moveX = (int)Math.Round(vx);
                int moveY = (int)Math.Round(vy);

                bullet.Left += moveX;
                bullet.Top += moveY;
            }

            // update public positions after movement
            bulletleft = bullet.Left;
            bullettop = bullet.Top;
            bulletright = bullet.Left + bullet.Width;
            bulletdown = bullet.Top + bullet.Height;

            // shrink the bullet progressively but only every 'shrinkEvery' ticks
            // Sniper bullets don't shrink
            if (shouldShrink)
            {
                shrinkCounter++;
                if (shrinkCounter >= shrinkEvery)
                {
                    shrinkCounter = 0;

                    if (bullet.Width > 1 && bullet.Height > 1)
                    {
                        int newW = Math.Max(1, bullet.Width - shrinkRate);
                        int newH = Math.Max(1, bullet.Height - shrinkRate);

                        // adjust position so the bullet appears to shrink around its center
                        bullet.Left += (bullet.Width - newW) / 2;
                        bullet.Top += (bullet.Height - newH) / 2;

                        bullet.Size = new Size(newW, newH);

                        // update public positions after shrink
                        bulletleft = bullet.Left;
                        bullettop = bullet.Top;
                        bulletright = bullet.Left + bullet.Width;
                        bulletdown = bullet.Top + bullet.Height;
                    }
                    else
                    {
                        DisposeBullet();
                        return;
                    }
                }
            }

            // boundary check using left, right, top, down (bottom) - now dynamic based on form size
            if (parentForm != null)
            {
                int formWidth = parentForm.ClientSize.Width;
                int formHeight = parentForm.ClientSize.Height;
                
                if (bulletleft < 10 || bulletright > formWidth - 10 || 
                    bullettop < 10 || bulletdown > formHeight - 10)
                {
                    DisposeBullet();
                }
            }
            else
            {
                // Fallback to old behavior if form reference is lost
                if (bulletleft < 10 || bulletright > 860 || bullettop < 10 || bulletdown > 600)
                {
                    DisposeBullet();
                }
            }
        }

        private void DisposeBullet()
        {
            try
            {
                if (bulletTimer != null)
                {
                    bulletTimer.Stop();
                    bulletTimer.Tick -= BulletTimerEvent;
                    bulletTimer.Dispose();
                }
            }   
            catch { }

            try
            {
                if (bullet != null)
                {
                    bullet.Dispose();   
                }
            }
            catch { }

            bulletTimer = null;
            bullet = null;
        }
    }
}
