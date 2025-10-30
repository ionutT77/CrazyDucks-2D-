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
using System.IO;

//save highscore using json file and when i start the game it should load the highscore from json file - CHECKED
//add health packs that spawn randomly on the map checked
//generate randomly ammo boxes on the map with a random amount between 1-10 checked
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
        
        // Obstacle system
        List<PictureBox> obstaclesList = new List<PictureBox>();
        private const int ObstacleCount = 5; // Number of obstacles to spawn
        private const int ObstacleSize = 50; // Size of each obstacle (smaller)
        
        // Stuck detection for bounce mechanic
        private Dictionary<PictureBox, int> stuckCounter = new Dictionary<PictureBox, int>();
        private const int StuckThreshold = 3; // Frames to consider entity stuck

        // New fields for dynamic difficulty scaling
        int baseSpeed = 2;
        int baseDuckCount = 3;
        int currentLevel = 1;
        int lastBonusScore = 0; // Track last bonus milestone
        
        // Fullscreen state tracking
        private bool isFullscreen = false;
        private FormWindowState previousWindowState;
        private FormBorderStyle previousBorderStyle;
        private Size previousSize;
        private Point previousLocation;
        
        // Window resize tracking for position scaling
        private Size lastFormSize;
        
        // Weapon firing system
        private bool isSpacePressed = false;
        private DateTime lastShotTime = DateTime.MinValue;
        private bool hasFiredThisPress = false; // NEW: Prevent spam clicking


        public Form1()
        {
            InitializeComponent();
            
            // Enable key preview so form receives key events before controls
            this.KeyPreview = true;
            
            // Add mouse click event for shooting
            this.MouseClick += Form1_MouseClick;
            
            // Load highscore from JSON file when game starts -checked
            highscorE = HighscoreManager.LoadHighscore();
            
            // Store initial window state for fullscreen toggle
            previousWindowState = this.WindowState;
            previousBorderStyle = this.FormBorderStyle;
            previousSize = this.Size;
            previousLocation = this.Location;
            
            // Initialize cooldown bar
            BulletCooldownBar.Maximum = 100;
            BulletCooldownBar.Value = 100; // Start ready to fire
            
            // Initialize window size tracking for scaling
            lastFormSize = this.ClientSize;
            
            // Position UI elements responsively
            PositionUIElements();
            
            // Generate random obstacles on the map
            GenerateObstacles();
            
            Restartgame();
        }
        public void playsound()
        {
            try
            {
                string soundPath = Path.Combine(Application.StartupPath, "Fortnite_dead_sound.wav");
                if (File.Exists(soundPath))
                {
                    SoundPlayer sound = new SoundPlayer(soundPath);
                    sound.Play();
                }
            }
            catch (Exception ex)
            {
                // Prevents crashes if sound file is missing
                Console.WriteLine($"Sound playback error: {ex.Message}");
            }
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
                CleanupPickups(); // Remove all pickups from the map
                txtgmovr.Visible = true;
                CenterGameOverText(); // Center game over text when displayed
                txtgmovr.BringToFront();
                playsound();
            }

            highscore.Text = "Highscore:" + highscorE;
            txtAmmo.Text = "Ammo:" + ammo;
            txtScore.Text = "Kills:" + score;
            
            // Update bullet cooldown bar
            UpdateCooldownBar();

            // Store previous player position for collision rollback
            int previousLeft = Player.Left;
            int previousTop = Player.Top;

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
            
            // Check if player collides with any obstacle and rollback if needed
            if (CheckObstacleCollision(Player))
            {
                // Apply bounce away from obstacle instead of simple rollback
                BounceAwayFromObstacle(Player, previousLeft, previousTop);
            }

            // Update player image based on current movement direction
            UpdatePlayerImage();
            
            // Handle automatic fire for rifle
            if (WeaponManager.IsAutomatic() && isSpacePressed && ammo > 0 && !gameOver)
            {
                if (WeaponManager.CanFire(lastShotTime))
                {
                    ammo--;
                    ShootBullet(face);
                    lastShotTime = DateTime.Now;
                    
                    if (ammo < 1)
                        DropAmmo();
                }
            }

            // Randomly spawn ammo boxes and health packs on the map
            // Adjust spawn probabilities to taste
            if (!gameOver)
            {
                // spawn ammo box occasionally - reduced spawn rate
                if (rNum.Next(0, 2000) < 6) // ~0.3% chance per tick (reduced from 0.6%)
                    DropRandomAmmoBox();

                // spawn health pack - rarely
                if (rNum.Next(0, 3000) < 6) // ~0.2% chance per tick (reduced from previous)
                    DropRandomHealthPack();
            }

            foreach(Control x in this.Controls)
            {
                // handle pickups (ammo boxes and health packs)
                if (x is PictureBox && x.Tag != null)
                {
                    string t = x.Tag.ToString();

                    if (t.StartsWith("ammo", StringComparison.OrdinalIgnoreCase))
                    {
                        // Tag format: "ammo" or "ammo:NN"
                        int amount = 5; // default fallback
                        var parts = t.Split(':');
                        if (parts.Length == 2)
                            int.TryParse(parts[1], out amount);

                        if (Player.Bounds.IntersectsWith(x.Bounds))
                        {
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            ammo += amount;
                        }
                    }
                    else if (t.StartsWith("health", StringComparison.OrdinalIgnoreCase))
                    {
                        // Tag format: "health:NN"
                        int heal = 20; // default fallback
                        var parts = t.Split(':');
                        if (parts.Length == 2)
                            int.TryParse(parts[1], out heal);

                        if (Player.Bounds.IntersectsWith(x.Bounds))
                        {
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            playerhealth = Math.Min(100, playerhealth + heal);
                        }
                    }
                }

                if(x is PictureBox && (string)x.Tag == "duck")
                {
                    if(Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerhealth -= 1;
                    }

                    // Store previous duck position for collision rollback
                    int prevDuckLeft = x.Left;
                    int prevDuckTop = x.Top;

                    // Use A* pathfinding to calculate target position (avoiding obstacles)
                    Point playerCenter = new Point(Player.Left + Player.Width / 2, Player.Top + Player.Height / 2);
                    Point nextPosition = GetNextPathPosition((PictureBox)x, playerCenter);
                    
                    // Calculate direction to next position
                    int deltaX = nextPosition.X - x.Left;
                    int deltaY = nextPosition.Y - x.Top;
                    
                    // Normalize movement to duckspeed
                    double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    if (distance > 0)
                    {
                        double moveX = (deltaX / distance) * duckspeed;
                        double moveY = (deltaY / distance) * duckspeed;
                        
                        x.Left += (int)moveX;
                        x.Top += (int)moveY;
                    }
                    
                    // Check collision with obstacles and rollback if needed
                    if (CheckObstacleCollision(x))
                    {
                        // Apply bounce away from obstacle for ducks
                        BounceAwayFromObstacle(x, prevDuckLeft, prevDuckTop);
                    }
                    
                    // Update duck image based on movement direction
                    bool isDiagonal = Math.Abs(deltaX) > 10 && Math.Abs(deltaY) > 10;

                    if (isDiagonal)
                    {
                        if (deltaX > 0 && deltaY > 0)
                        {
                            ((PictureBox)x).Image = TryGetResourceImage("right_down_diag_duck") ?? 
                                                    TryGetResourceImage("dright2") ?? 
                                                    Properties.Resources.dright;
                        }
                        else if (deltaX > 0 && deltaY < 0)
                        {
                            ((PictureBox)x).Image = TryGetResourceImage("right_up_diag_duck") ?? 
                                                    TryGetResourceImage("dright2") ?? 
                                                    Properties.Resources.dright;
                        }
                        else if (deltaX < 0 && deltaY > 0)
                        {
                            ((PictureBox)x).Image = TryGetResourceImage("left_down_diag_duck") ?? 
                                                    TryGetResourceImage("dleft2") ?? 
                                                    Properties.Resources.dleft;
                        }
                        else
                        {
                            ((PictureBox)x).Image = TryGetResourceImage("left_up_diag_duck") ?? 
                                                    TryGetResourceImage("dleft2") ?? 
                                                    Properties.Resources.dleft;
                        }
                    }
                    else
                    {
                        if (Math.Abs(deltaX) > Math.Abs(deltaY))
                        {
                            if (deltaX > 0)
                            {
                                ((PictureBox)x).Image = TryGetResourceImage("dright2") ?? Properties.Resources.dright;
                            }
                            else
                            {
                                ((PictureBox)x).Image = TryGetResourceImage("dleft2") ?? Properties.Resources.dleft;
                            }
                        }
                        else
                        {
                            if (deltaY > 0)
                            {
                                ((PictureBox)x).Image = TryGetResourceImage("ddown2") ?? Properties.Resources.down;
                            }
                            else
                            {
                                ((PictureBox)x).Image = TryGetResourceImage("dup2") ?? Properties.Resources.dup;
                            }
                        }
                    }
                }

                //make the duck move diagonally when the player is in diagonal position to the duck

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
            }

            // Apply dynamic difficulty scaling based on score
            UpdateDifficultyByScore();
            
            // NEW: Prevent ducks from overlapping each other
            PreventDuckOverlap();
            
            // Reset stuck counters for entities that are not colliding
            ResetStuckCounters();

        }
        
            

        // Dynamically adjusts game difficulty based on player score using logarithmic scaling
        private void UpdateDifficultyByScore()
        {
            // Calculate level based on logarithmic scale
            // Formula: level increases every ~30-40 kills (slower progression)
            int newLevel = (int)Math.Floor(Math.Log10(score + 1) * 5) + 1;
            
            if (newLevel != currentLevel)
            {
                currentLevel = newLevel;
                txtlevel.Text = "Level: " + currentLevel;
            }

            // Logarithmic speed increase for ducks (SLOWER)
            // Starts at 2, gradually increases more slowly
            // Formula: baseSpeed + log10(score + 1) * smaller multiplier
            duckspeed = (int)Math.Ceiling(baseSpeed + Math.Log10(score + 1) * 0.8);
            duckspeed = Math.Min(duckspeed, 6); // Cap at 6 instead of 10

            // Calculate target number of ducks on screen (FEWER DUCKS)
            // Starts at 3, increases much more slowly: 3, 4, 5...
            int targetDuckCount = baseDuckCount + (int)Math.Floor(Math.Log10(score + 1) * 0.8);
            targetDuckCount = Math.Min(targetDuckCount, 8); // Cap at 8 instead of 12

            // Spawn additional ducks if needed
            while (ducksList.Count < targetDuckCount && !gameOver)
            {
                MakeDucks();
            }

            // Increase player speed slightly to compensate for difficulty
            // Player speed: 10, 11, 12... up to 14 (slower increase)
            playerspeed = 10 + (int)Math.Floor(Math.Log10(score + 1) * 0.5);
            playerspeed = Math.Min(playerspeed, 14);

            // Milestone bonuses every 30 kills (less frequent)
            if (score > 0 && score % 30 == 0 && score != lastBonusScore)
            {
                lastBonusScore = score; // Mark this milestone as awarded
                
                // Award health and ammo bonuses
                playerhealth = Math.Min(100, playerhealth + 20);
                ammo += 15;
            }
        }

        // Removes all pickups (ammo boxes and health packs) from the map
        private void CleanupPickups()
        {
            // Create a list to store pickups to remove (avoid modifying collection during iteration)
            List<Control> pickupsToRemove = new List<Control>();

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag != null)
                {
                    string tag = x.Tag.ToString();
                    
                    // Check if it's an ammo box or health pack
                    if (tag.StartsWith("ammo", StringComparison.OrdinalIgnoreCase) ||
                        tag.StartsWith("health", StringComparison.OrdinalIgnoreCase))
                    {
                        pickupsToRemove.Add(x);
                    }
                }
            }

            // Remove all pickups from the form
            foreach (Control pickup in pickupsToRemove)
            {
                this.Controls.Remove(pickup);
                ((PictureBox)pickup).Dispose();
            }
        }


        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(gameOver == true)
            {
                return;
            }
            
            // ESC key to exit fullscreen
            if(e.KeyCode == Keys.Escape && isFullscreen)
            {
                ToggleFullscreen();
                return;
            }
            
            // F key to toggle fullscreen
            if(e.KeyCode == Keys.F)
            {
                ToggleFullscreen();
                return;
            }
            
            // Space key for shooting
            if(e.KeyCode == Keys.Space && !isSpacePressed)
            {
                isSpacePressed = true;
                hasFiredThisPress = false; // Reset for this key press
                
                // Immediate shot on press (for all weapons)
                if (ammo > 0 && !hasFiredThisPress)
                {
                    if (WeaponManager.CanFire(lastShotTime))
                    {
                        ammo--;
                        ShootBullet(face);
                        lastShotTime = DateTime.Now;
                        hasFiredThisPress = true; // NEW: Mark as fired this press
                        
                        if (ammo < 1)
                            DropAmmo();
                    }
                }
            }
            
            // WASD movement controls
            if(e.KeyCode == Keys.A)
            {
                GoLeft = true;
            }
            if(e.KeyCode == Keys.D)
            {
                GoRight = true;
            }
            if(e.KeyCode == Keys.W)
            {
                GoUp = true;
            }
            if(e.KeyCode == Keys.S)
            {
                GoDown = true;
            }
            
            // Arrow key movement controls
            if(e.KeyCode == Keys.Left)
            {
                GoLeft = true;
            }
            if(e.KeyCode == Keys.Right)
            {
                GoRight = true;
            }
            if(e.KeyCode== Keys.Up)
            {
                GoUp = true;
            }
            if(e.KeyCode== Keys.Down)
            {
                GoDown = true;
            }
        }

        // Updates player image based on current movement direction (supports diagonals) - checked
        private void UpdatePlayerImage()
        {
            if (gameOver) return;

            // Check for diagonal movements first
            if (GoUp && GoLeft)
            {
                face = "left_up_diag";
                Player.Image = TryGetResourceImage("left_up_diag_player") ?? Properties.Resources.left;
            }
            else if (GoUp && GoRight)
            {
                face = "right_up_diag";
                Player.Image = TryGetResourceImage("right_up_diag_player") ?? Properties.Resources.right;
            }
            else if (GoDown && GoLeft)
            {
                face = "left_down_diag";
                Player.Image = TryGetResourceImage("left_down_diag_player") ?? Properties.Resources.left;
            }
            else if (GoDown && GoRight)
            {
                face = "right_down";
                Player.Image = TryGetResourceImage("right_down_diag_player") ?? Properties.Resources.right;
            }
            // Cardinal directions
            else if (GoLeft)
            {
                face = "left";
                Player.Image = Properties.Resources.left;
            }
            else if (GoRight)
            {
                face = "right";
                Player.Image = Properties.Resources.right;
            }
            else if (GoUp)
            {
                face = "up";
                Player.Image = Properties.Resources.up;
            }
            else if (GoDown)
            {
                face = "down";
                Player.Image = Properties.Resources.down;
            }
        }
        
        // Toggles between windowed and fullscreen mode (F key)
        private void ToggleFullscreen()
        {
            if (!isFullscreen)
            {
                // Enter fullscreen mode (borderless windowed)
                previousWindowState = this.WindowState;
                previousBorderStyle = this.FormBorderStyle;
                previousSize = this.Size;
                previousLocation = this.Location;

                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Normal;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this.TopMost = true;

                isFullscreen = true;
            }
            else
            {
                // Exit fullscreen mode (restore windowed)
                this.TopMost = false;
                this.FormBorderStyle = previousBorderStyle;
                this.WindowState = previousWindowState;
                
                if (previousWindowState == FormWindowState.Normal)
                {
                    this.Size = previousSize;
                    this.Location = previousLocation;
                }

                isFullscreen = false;
            }
            
            // Regenerate obstacles for new screen size
            GenerateObstacles();
            
            // Reposition UI elements after toggle
            PositionUIElements();
        }
        
        // Positions UI elements responsively based on current window size
        private void PositionUIElements()
        {
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;
            
            // Left side elements - stay anchored to left
            txtAmmo.Location = new Point(12, 9);
            highscore.Location = new Point(txtAmmo.Right + 20, 9);
            txtScore.Location = new Point(highscore.Right + 20, 8);
            txtlevel.Location = new Point(txtScore.Right + 20, 8);
            
            // Cooldown bar - after level
            BulletCooldown.Location = new Point(txtlevel.Right + 20, 9);
            BulletCooldownBar.Location = new Point(BulletCooldown.Right + 10, 12);
            
            // Right side elements - anchor to right edge
            healthBar.Location = new Point(formWidth - healthBar.Width - 12, 12);
            txtHealth.Location = new Point(healthBar.Left - txtHealth.Width - 5, 9);
            
            // Center game over text
            CenterGameOverText();
        }
        
        // Updates the cooldown progress bar to show time until next shot
        private void UpdateCooldownBar()
        {
            if (gameOver)
            {
                BulletCooldownBar.Value = 0;
                return;
            }
            
            // Get current weapon's cooldown
            int cooldownTime = WeaponManager.GetFireCooldown();
            
            // Calculate time since last shot
            TimeSpan timeSinceLastShot = DateTime.Now - lastShotTime;
            double millisecondsSinceShot = timeSinceLastShot.TotalMilliseconds;
            
            // Calculate cooldown percentage (0-100)
            if (millisecondsSinceShot >= cooldownTime)
            {
                // Ready to fire - bar is full (green)
                BulletCooldownBar.Value = 100;
                BulletCooldownBar.ForeColor = Color.Green;
            }
            else
            {
                // Still cooling down - show progress
                double cooldownPercent = (millisecondsSinceShot / cooldownTime) * 100;
                BulletCooldownBar.Value = (int)Math.Min(100, Math.Max(0, cooldownPercent));
                
                // Color changes based on readiness
                if (cooldownPercent < 30)
                {
                    BulletCooldownBar.ForeColor = Color.Red; // Just shot, not ready
                }
                else if (cooldownPercent < 70)
                {
                    BulletCooldownBar.ForeColor = Color.Yellow; // Getting close
                }
                else
                {
                    BulletCooldownBar.ForeColor = Color.LimeGreen; // Almost ready
                }
            }
        }
        
        // Centers the game over text on the screen
        private void CenterGameOverText()
        {
            if (txtgmovr != null)
            {
                int centerX = (this.ClientSize.Width - txtgmovr.Width) / 2;
                int centerY = (this.ClientSize.Height - txtgmovr.Height) / 3; // Place in upper-middle area
                txtgmovr.Location = new Point(centerX, centerY);
            }
        }
        
        // Handle form resize event to reposition UI elements
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Skip if form is minimized or not yet initialized
            if (this.WindowState == FormWindowState.Minimized || lastFormSize.IsEmpty)
                return;
            
            // Check if this is a significant resize (not just a small adjustment)
            int widthDiff = Math.Abs(this.ClientSize.Width - lastFormSize.Width);
            int heightDiff = Math.Abs(this.ClientSize.Height - lastFormSize.Height);
            
            // If significant resize (more than 100px in either dimension), regenerate obstacles
            if (widthDiff > 100 || heightDiff > 100)
            {
                GenerateObstacles();
            }
            
            // Scale player and duck positions proportionally
            ScalePositions();
            
            // Reposition UI elements
            PositionUIElements();
            
            // Update last size
            lastFormSize = this.ClientSize;
        }
        
        // Scales player and duck positions based on the current form size
        private void ScalePositions()
        {
            if (Player == null || ducksList.Count == 0)
                return;
            
            // Calculate scale factors
            float scaleX = (float)this.ClientSize.Width / previousSize.Width;
            float scaleY = (float)this.ClientSize.Height / previousSize.Height;
            
            // Scale player position
            Player.Left = (int)(Player.Left * scaleX);
            Player.Top = (int)(Player.Top * scaleY);
            
            // Scale all duck positions
            foreach (PictureBox duck in ducksList)
            {
                if (duck == null) continue;
                
                duck.Left = (int)(duck.Left * scaleX);
                duck.Top = (int)(duck.Top * scaleY);
            }
        }
        
        // Clamps player position to stay within visible screen bounds
        private void ClampPlayerPosition()
        {
            if (Player == null) return;
            
            int maxX = this.ClientSize.Width - Player.Width;
            int maxY = this.ClientSize.Height - Player.Height;
            
            // Keep player within bounds
            if (Player.Left < 0) Player.Left = 0;
            if (Player.Left > maxX) Player.Left = maxX;
            if (Player.Top < 40) Player.Top = 40; // Leave space for UI
            if (Player.Top > maxY) Player.Top = maxY;
        }
        
        // Clamps all ducks to stay within visible screen bounds
        private void ClampDuckPositions()
        {
            foreach (PictureBox duck in ducksList)
            {
                if (duck == null) continue;
                
                int maxX = this.ClientSize.Width - duck.Width;
                int maxY = this.ClientSize.Height - duck.Height;
                
                // Keep ducks within bounds
                if (duck.Left < 0) duck.Left = 0;
                if (duck.Left > maxX) duck.Left = maxX;
                if (duck.Top < 40) duck.Top = 40; // Leave space for UI
                if (duck.Top > maxY) duck.Top = maxY;
            }
        }
        
        // Prevents ducks from overlapping each other by gently pushing them apart
        private void PreventDuckOverlap()
        {
            const int minDistance = 70; // Increased minimum pixel distance between duck centers
            
            for (int i = 0; i < ducksList.Count; i++)
            {
                PictureBox duck1 = ducksList[i];
                if (duck1 == null) continue;
                
                for (int j = i + 1; j < ducksList.Count; j++)
                {
                    PictureBox duck2 = ducksList[j];
                    if (duck2 == null) continue;
                    
                    // Calculate distance between duck centers
                    int centerX1 = duck1.Left + duck1.Width / 2;
                    int centerY1 = duck1.Top + duck1.Height / 2;
                    int centerX2 = duck2.Left + duck2.Width / 2;
                    int centerY2 = duck2.Top + duck2.Height / 2;
                    
                    int deltaX = centerX2 - centerX1;
                    int deltaY = centerY2 - centerY1;
                    double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    
                    // If ducks are too close, push them apart strongly
                    if (distance < minDistance && distance > 0)
                    {
                        // Calculate push direction with stronger force
                        double overlap = minDistance - distance;
                        double pushX = (deltaX / distance) * overlap; // Full overlap distance, not divided by 2
                        double pushY = (deltaY / distance) * overlap;
                        
                        // Push both ducks in opposite directions with full force
                        duck1.Left -= (int)pushX;
                        duck1.Top -= (int)pushY;
                        duck2.Left += (int)pushX;
                        duck2.Top += (int)pushY;
                        
                        // Ensure ducks stay within bounds
                        ClampDuckPosition(duck1);
                        ClampDuckPosition(duck2);
                    }
                }
            }
        }
        
        // Helper method to clamp a single duck's position
        private void ClampDuckPosition(PictureBox duck)
        {
            if (duck == null) return;
            
            int maxX = this.ClientSize.Width - duck.Width;
            int maxY = this.ClientSize.Height - duck.Height;
            
            if (duck.Left < 0) duck.Left = 0;
            if (duck.Left > maxX) duck.Left = maxX;
            if (duck.Top < 40) duck.Top = 40;
            if (duck.Top > maxY) duck.Top = maxY;
        }
        
        // ======================
        // OBSTACLE SYSTEM
        // ======================
        
        // Generates random obstacles on the map
        private void GenerateObstacles()
        {
            // Clear existing obstacles
            foreach (PictureBox obstacle in obstaclesList)
            {
                this.Controls.Remove(obstacle);
                obstacle.Dispose();
            }
            obstaclesList.Clear();
            
            int attempts = 0;
            int maxAttempts = 100;
            
            while (obstaclesList.Count < ObstacleCount && attempts < maxAttempts)
            {
                attempts++;
                
                // Random position for obstacle
                int obstacleX = rNum.Next(100, this.ClientSize.Width - ObstacleSize - 100);
                int obstacleY = rNum.Next(100, this.ClientSize.Height - ObstacleSize - 100);
                
                // Create temporary rectangle to test placement
                Rectangle testRect = new Rectangle(obstacleX, obstacleY, ObstacleSize, ObstacleSize);
                
                // Check if obstacle would overlap with player spawn area (center)
                int centerX = this.ClientSize.Width / 2;
                int centerY = this.ClientSize.Height / 2;
                Rectangle playerSpawnArea = new Rectangle(centerX - 150, centerY - 150, 300, 300);
                
                if (testRect.IntersectsWith(playerSpawnArea))
                    continue;
                
                // Check if obstacle overlaps with existing obstacles
                bool overlaps = false;
                foreach (PictureBox existingObstacle in obstaclesList)
                {
                    if (testRect.IntersectsWith(existingObstacle.Bounds))
                    {
                        overlaps = true;
                        break;
                    }
                }
                
                if (overlaps)
                    continue;
                
                // Create the obstacle
                PictureBox obstacle = new PictureBox();
                obstacle.Tag = "obstacle";
                obstacle.Left = obstacleX;
                obstacle.Top = obstacleY;
                obstacle.Size = new Size(ObstacleSize, ObstacleSize);
                obstacle.SizeMode = PictureBoxSizeMode.StretchImage;
                obstacle.BackColor = Color.Transparent;
                
                // Load rock image from resources
                var img = TryGetResourceImage("rock");
                if (img != null)
                {
                    obstacle.Image = img;
                }
                else
                {
                    // Fallback: Gray box with border if rock image not found
                    obstacle.BackColor = Color.DarkGray;
                    obstacle.BorderStyle = BorderStyle.FixedSingle;
                }
                
                obstaclesList.Add(obstacle);
                this.Controls.Add(obstacle);
                obstacle.BringToFront();
            }
            
            // Ensure player is always on top
            Player.BringToFront();
        }
        
        // Checks if a control collides with any obstacle
        private bool CheckObstacleCollision(Control control)
        {
            foreach (PictureBox obstacle in obstaclesList)
            {
                if (control.Bounds.IntersectsWith(obstacle.Bounds))
                {
                    return true;
                }
            }
            return false;
        }
        
        // Gets the obstacle that the control is colliding with (if any)
        private PictureBox GetCollidingObstacle(Control control)
        {
            foreach (PictureBox obstacle in obstaclesList)
            {
                if (control.Bounds.IntersectsWith(obstacle.Bounds))
                {
                    return obstacle;
                }
            }
            return null;
        }
        
        // Bounces entity away from obstacle when stuck
        private void BounceAwayFromObstacle(Control entity, int previousLeft, int previousTop)
        {
            PictureBox collidingObstacle = GetCollidingObstacle(entity);
            
            if (collidingObstacle == null)
            {
                // No collision, just return
                return;
            }
            
            // Track stuck counter
            if (!stuckCounter.ContainsKey((PictureBox)entity))
            {
                stuckCounter[(PictureBox)entity] = 0;
            }
            
            stuckCounter[(PictureBox)entity]++;
            
            // Calculate center positions
            int entityCenterX = entity.Left + entity.Width / 2;
            int entityCenterY = entity.Top + entity.Height / 2;
            int obstacleCenterX = collidingObstacle.Left + collidingObstacle.Width / 2;
            int obstacleCenterY = collidingObstacle.Top + collidingObstacle.Height / 2;
            
            // Calculate direction away from obstacle
            int deltaX = entityCenterX - obstacleCenterX;
            int deltaY = entityCenterY - obstacleCenterY;
            
            // Normalize and apply bounce force
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            
            if (distance > 0)
            {
                // Determine bounce strength based on stuck counter
                int bounceStrength = 15; // Base bounce strength
                
                if (stuckCounter[(PictureBox)entity] > StuckThreshold)
                {
                    // Apply stronger bounce if stuck for multiple frames
                    bounceStrength = 30;
                }
                
                // Calculate bounce direction (away from obstacle)
                int bounceX = (int)((deltaX / distance) * bounceStrength);
                int bounceY = (int)((deltaY / distance) * bounceStrength);
                
                // Apply bounce
                entity.Left = previousLeft + bounceX;
                entity.Top = previousTop + bounceY;
                
                // If still colliding after bounce, try moving to side
                if (CheckObstacleCollision(entity))
                {
                    // Try moving perpendicular to collision direction
                    entity.Left = previousLeft - deltaY / 5;
                    entity.Top = previousTop + deltaX / 5;
                    
                    // If still stuck, teleport to safe distance
                    if (CheckObstacleCollision(entity))
                    {
                        entity.Left = previousLeft + (int)((deltaX / distance) * 60);
                        entity.Top = previousTop + (int)((deltaY / distance) * 60);
                    }
                }
            }
            else
            {
                // Fallback: just move back to previous position
                entity.Left = previousLeft;
                entity.Top = previousTop;
            }
        }
        
        // Resets stuck counters for entities that are no longer colliding
        private void ResetStuckCounters()
        {
            List<PictureBox> toRemove = new List<PictureBox>();
            
            foreach (var kvp in stuckCounter)
            {
                PictureBox entity = kvp.Key;
                
                // Check if entity still exists and is not colliding
                if (!this.Controls.Contains(entity) || !CheckObstacleCollision(entity))
                {
                    toRemove.Add(entity);
                }
            }
            
            // Remove cleared entities from stuck counter
            foreach (PictureBox entity in toRemove)
            {
                stuckCounter.Remove(entity);
            }
        }
        
        // ======================
        // A* PATHFINDING FOR DUCKS
        // ======================
        
        // Pathfinding node for A* algorithm
        private class PathNode
        {
            public Point Position { get; set; }
            public PathNode Parent { get; set; }
            public float GCost { get; set; } // Distance from start
            public float HCost { get; set; } // Heuristic distance to target
            public float FCost { get { return GCost + HCost; } }
            
            public PathNode(Point position)
            {
                Position = position;
            }
        }
        
        // Calculates next move for duck using A* pathfinding
        private Point GetNextPathPosition(PictureBox duck, Point target)
        {
            // Grid-based pathfinding (20px cells)
            const int gridSize = 20;
            
            Point duckGridPos = new Point(duck.Left / gridSize, duck.Top / gridSize);
            Point targetGridPos = new Point(target.X / gridSize, target.Y / gridSize);
            
            // If target is close or no obstacles nearby, move directly
            int dist = Math.Abs(duckGridPos.X - targetGridPos.X) + Math.Abs(duckGridPos.Y - targetGridPos.Y);
            if (dist < 3 || !HasObstacleNearby(duck, target))
            {
                return target; // Direct movement
            }
            
            // A* algorithm
            List<PathNode> openList = new List<PathNode>();
            List<Point> closedList = new List<Point>();
            
            PathNode startNode = new PathNode(duckGridPos);
            startNode.GCost = 0;
            startNode.HCost = GetHeuristic(duckGridPos, targetGridPos);
            openList.Add(startNode);
            
            int maxIterations = 50; // Limit iterations for performance
            int iterations = 0;
            
            while (openList.Count > 0 && iterations < maxIterations)
            {
                iterations++;
                
                // Get node with lowest FCost
                PathNode current = openList[0];
                foreach (PathNode node in openList)
                {
                    if (node.FCost < current.FCost)
                        current = node;
                }
                
                // Reached target
                if (current.Position == targetGridPos)
                {
                    // Trace back path and return first step
                    PathNode step = current;
                    while (step.Parent != null && step.Parent.Parent != null)
                    {
                        step = step.Parent;
                    }
                    return new Point(step.Position.X * gridSize, step.Position.Y * gridSize);
                }
                
                openList.Remove(current);
                closedList.Add(current.Position);
                
                // Check neighbors (8 directions)
                int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
                int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
                
                for (int i = 0; i < 8; i++)
                {
                    Point neighborPos = new Point(current.Position.X + dx[i], current.Position.Y + dy[i]);
                    
                    // Check bounds
                    if (neighborPos.X < 0 || neighborPos.Y < 2 || 
                        neighborPos.X >= this.ClientSize.Width / gridSize || 
                        neighborPos.Y >= this.ClientSize.Height / gridSize)
                        continue;
                    
                    // Skip if in closed list
                    if (closedList.Contains(neighborPos))
                        continue;
                    
                    // Check if position is blocked by obstacle
                    Rectangle testRect = new Rectangle(neighborPos.X * gridSize, neighborPos.Y * gridSize, 
                                                       duck.Width, duck.Height);
                    bool blocked = false;
                    foreach (PictureBox obstacle in obstaclesList)
                    {
                        if (testRect.IntersectsWith(obstacle.Bounds))
                        {
                            blocked = true;
                            break;
                        }
                    }
                    
                    if (blocked)
                        continue;
                    
                    // Calculate costs
                    float moveCost = (i % 2 == 0) ? 1.4f : 1.0f; // Diagonal vs straight
                    float newGCost = current.GCost + moveCost;
                    
                    PathNode existingNode = openList.Find(n => n.Position == neighborPos);
                    if (existingNode != null)
                    {
                        if (newGCost < existingNode.GCost)
                        {
                            existingNode.GCost = newGCost;
                            existingNode.Parent = current;
                        }
                    }
                    else
                    {
                        PathNode neighbor = new PathNode(neighborPos);
                        neighbor.GCost = newGCost;
                        neighbor.HCost = GetHeuristic(neighborPos, targetGridPos);
                        neighbor.Parent = current;
                        openList.Add(neighbor);
                    }
                }
            }
            
            // No path found, move directly (will be blocked by collision)
            return target;
        }
        
        // Heuristic function for A* (Manhattan distance)
        private float GetHeuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
        
        // Checks if there's an obstacle between duck and target
        private bool HasObstacleNearby(PictureBox duck, Point target)
        {
            foreach (PictureBox obstacle in obstaclesList)
            {
                // Check if obstacle is between duck and player
                Rectangle expandedBounds = new Rectangle(
                    obstacle.Left - 50, obstacle.Top - 50,
                    obstacle.Width + 100, obstacle.Height + 100);
                
                if (expandedBounds.Contains(duck.Left, duck.Top) || 
                    expandedBounds.Contains(target))
                {
                    return true;
                }
            }
            return false;
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            // Arrow keys
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
            
            // WASD keys
            if (e.KeyCode == Keys.A)
            {
                GoLeft = false;
            }
            if (e.KeyCode == Keys.D)
            {
                GoRight = false;
            }
            if (e.KeyCode == Keys.W)
            {
                GoUp = false;
            }
            if (e.KeyCode == Keys.S)
            {
                GoDown = false;
            }
            
            if(e.KeyCode == Keys.Space)
            {
                isSpacePressed = false;
                hasFiredThisPress = false; // NEW: Reset when key released
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
            
            // Play shooting sound based on weapon type
            PlayShootingSound();
        }
        
        // Play weapon-specific shooting sound
        private void PlayShootingSound()
        {
            try
            {
                string soundFile = "";
                
                // Determine which sound file to play based on weapon
                if (WeaponManager.CurrentWeapon == WeaponType.Sniper)
                {
                    soundFile = "mixkit-laser-weapon-shot-1681.wav";
                }
                else // Pistol or Rifle
                {
                    soundFile = "mixkit-game-gun-shot-1662.wav";
                }
                
                string soundPath = Path.Combine(Application.StartupPath, soundFile);
                
                if (File.Exists(soundPath))
                {
                    // Create new SoundPlayer for each shot to allow overlapping sounds
                    SoundPlayer shootSound = new SoundPlayer(soundPath);
                    shootSound.Play(); // Play asynchronously
                }
            }
            catch (Exception ex)
            {
                // Prevents crashes if sound file is missing
                Console.WriteLine($"Shooting sound error: {ex.Message}");
            }
        }
        
        // Handle mouse click for shooting
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            // Only left-click to shoot
            if (e.Button != MouseButtons.Left || gameOver || ammo <= 0)
                return;
            
            // Check cooldown
            if (!WeaponManager.CanFire(lastShotTime))
                return;
            
            // Calculate direction from player to mouse click position
            int deltaX = e.X - (Player.Left + Player.Width / 2);
            int deltaY = e.Y - (Player.Top + Player.Height / 2);
            
            // Determine the direction to shoot based on mouse position
            string shootDirection = GetDirectionFromDeltas(deltaX, deltaY);
            
            // Update player facing direction
            face = shootDirection;
            
            // Shoot bullet
            ammo--;
            ShootBullet(shootDirection);
            lastShotTime = DateTime.Now;
            
            if (ammo < 1)
                DropAmmo();
        }
        
        // Helper method to determine direction based on delta X and Y
        private string GetDirectionFromDeltas(int deltaX, int deltaY)
        {
            // Calculate angle in degrees
            double angle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI);
            
            // Normalize angle to 0-360
            if (angle < 0) angle += 360;
            
            // Determine direction based on angle (8 directions)
            if (angle >= 337.5 || angle < 22.5)
                return "right";
            else if (angle >= 22.5 && angle < 67.5)
                return "right_down_diag";
            else if (angle >= 67.5 && angle < 112.5)
                return "down";
            else if (angle >= 112.5 && angle < 157.5)
                return "left_down_diag";
            else if (angle >= 157.5 && angle < 202.5)
                return "left";
            else if (angle >= 202.5 && angle < 247.5)
                return "left_up_diag";
            else if (angle >= 247.5 && angle < 292.5)
                return "up";
            else // 292.5 to 337.5
                return "right_up_diag";
        }

        private void MakeDucks()
        {
            PictureBox duck = new PictureBox();
            duck.Tag = "duck";
            duck.Image = TryGetResourceImage("ddown2") ?? 
                         TryGetResourceImage("ddown") ?? 
                         Properties.Resources.down;
            duck.Left = rNum.Next(0, this.ClientSize.Width - 100);
            duck.Top = rNum.Next(40, this.ClientSize.Height - 100);
            duck.SizeMode = PictureBoxSizeMode.Zoom;
            ducksList.Add(duck);
            this.Controls.Add(duck);
            Player.BringToFront();
        }

        // helper: try to fetch an image from strongly-typed resources by key.
        // returns null if the key/property does not exist.
        private Image TryGetResourceImage(string key)
        {
            try
            {
                var obj = Properties.Resources.ResourceManager.GetObject(key);
                return obj as Image;
            }
            catch
            {
                return null;
            }
        }

        // called when player runs out of ammo
        private void DropAmmo()
        {
            PictureBox ammoBox = new PictureBox();
            int amount = rNum.Next(1, 11); // 1..10

            var img = TryGetResourceImage("ammobox") ?? TryGetResourceImage("ammo");

            if (img != null)
                ammoBox.Image = img;
            else
                ammoBox.BackColor = Color.Orange;

            ammoBox.SizeMode = PictureBoxSizeMode.Zoom;
            ammoBox.Size = new Size(32, 32);
            ammoBox.BackColor = this.BackColor;
            ammoBox.Left = rNum.Next(10, this.ClientSize.Width - ammoBox.Width);
            ammoBox.Top = rNum.Next(60, this.ClientSize.Height - ammoBox.Height);
            ammoBox.Tag = "ammo:" + amount;
            this.Controls.Add(ammoBox);
            ammoBox.BringToFront();
            Player.BringToFront();
        }

    

        // Spawn an ammo box (random amount 1..10) at a random map location-checked
        private void DropRandomAmmoBox()
        {
            PictureBox ammoBox = new PictureBox();
            int amount = rNum.Next(1, 11); // 1..10

            var img = TryGetResourceImage("ammobox") ?? TryGetResourceImage("ammo");

            if (img != null)
                ammoBox.Image = img;
            else
                ammoBox.BackColor = Color.Orange;

            ammoBox.SizeMode = PictureBoxSizeMode.Zoom;
            ammoBox.Size = new Size(32, 32);
            ammoBox.BackColor = this.BackColor;
            ammoBox.Left = rNum.Next(10, this.ClientSize.Width - ammoBox.Width);
            ammoBox.Top = rNum.Next(60, this.ClientSize.Height - ammoBox.Height);
            ammoBox.Tag = "ammo:" + amount;
            this.Controls.Add(ammoBox);
            ammoBox.BringToFront();
            Player.BringToFront();
        }

        // Spawn a health pack (random heal amount, e.g. 10..30) at a random map location-checked
        private void DropRandomHealthPack()
        {
            PictureBox medkitBox = new PictureBox();
            int heal = rNum.Next(10, 31); // heal 10..30

            var img = TryGetResourceImage("medkit");

            if (img != null)
            {
                medkitBox.Image = img;
            }
            else
            {
                // Fallback: Bright red box if medkit image not found (more visible!)
                medkitBox.BackColor = Color.Red;
            }

            medkitBox.SizeMode = PictureBoxSizeMode.Zoom;
            medkitBox.Size = new Size(40, 40); // Increased from 32 to 40 for better visibility
            medkitBox.Left = rNum.Next(10, this.ClientSize.Width - medkitBox.Width);
            medkitBox.Top = rNum.Next(60, this.ClientSize.Height - medkitBox.Height);
            medkitBox.Tag = "health:" + heal;
            this.Controls.Add(medkitBox);
            medkitBox.BringToFront();
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
            
            // Clear stuck counters
            stuckCounter.Clear();
            
            // Regenerate obstacles for new game
            GenerateObstacles();
            
            // Reset to starting duck count
            for(int i=0; i < baseDuckCount; i++)
            {
                MakeDucks();
            }
            
            GoUp=false;
            GoDown=false;
            GoLeft=false;
            GoRight=false;
            gameOver= false;
            
            // Update and save highscore if current score is higher
            if(highscorE<score)
            {
                highscorE=score;
                HighscoreManager.SaveHighscore(highscorE);
            }

            playerhealth = 100;
            score = 0;
            ammo = 10;
            playerspeed = 10;
            duckspeed = 2;
            currentLevel = 1; // Reset level
            lastBonusScore = 0; // Reset bonus tracker
            txtlevel.Text = "Level: 1";
            txtgmovr.Visible = false;

            GameTimer.Start();
        }
    }
}
