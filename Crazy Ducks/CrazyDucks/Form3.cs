using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyDucks
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            
            // Hook up button click events
            CloseButton.Click += CloseButton_Click;
            RefreshButton.Click += RefreshButton_Click;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Load debug information when form loads
            LoadDebugInfo();
        }

        private void LoadDebugInfo()
        {
            // Clear existing text
            txtDebugInfo.Clear();
            
            // Header
            txtDebugInfo.AppendText("═══════════════════════════════════════\r\n");
            txtDebugInfo.AppendText("     CRAZY DUCKS - DEBUG MENU\r\n");
            txtDebugInfo.AppendText("═══════════════════════════════════════\r\n\r\n");
            
            // Player Stats by Level
            txtDebugInfo.AppendText("▼ PLAYER SPEED BY LEVEL:\r\n");
            txtDebugInfo.AppendText("  Level 1: 10 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 2: 11 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 3: 12 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 4: 13 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 5+: 14 pixels/frame (max)\r\n\r\n");
            
            // Bullet Stats
            txtDebugInfo.AppendText("▼ BULLET PROPERTIES:\r\n");
            txtDebugInfo.AppendText("  Speed: 12 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Size: 8x8 pixels (shrinks over time)\r\n");
            txtDebugInfo.AppendText("  Shrink Rate: 1 pixel every 6 frames\r\n");
            txtDebugInfo.AppendText("  Damage: Instant kill on ducks\r\n\r\n");
            
            // Ammo System
            txtDebugInfo.AppendText("▼ AMMO SYSTEM:\r\n");
            txtDebugInfo.AppendText("  Starting Ammo: 10\r\n");
            txtDebugInfo.AppendText("  Ammo Box Drop: 1-10 random\r\n");
            txtDebugInfo.AppendText("  Bonus at Milestone (÷30): +15 ammo\r\n");
            txtDebugInfo.AppendText("  Bonus Health at Milestone: +20 HP\r\n\r\n");
            
            // Duck Stats by Level
            txtDebugInfo.AppendText("▼ DUCK SPEED BY LEVEL:\r\n");
            txtDebugInfo.AppendText("  Level 1: 2 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 2: 3 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 3: 4 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 4: 5 pixels/frame\r\n");
            txtDebugInfo.AppendText("  Level 5+: 6 pixels/frame (max)\r\n\r\n");
            
            // Duck Health & Damage
            txtDebugInfo.AppendText("▼ DUCK PROPERTIES:\r\n");
            txtDebugInfo.AppendText("  Health: 1 HP (dies in 1 hit)\r\n");
            txtDebugInfo.AppendText("  Damage to Player: 1 HP/collision\r\n");
            txtDebugInfo.AppendText("  Starting Count: 3 ducks\r\n");
            txtDebugInfo.AppendText("  Max Count: 8 ducks\r\n");
            txtDebugInfo.AppendText("  AI: Chase player (diagonal capable)\r\n\r\n");
            
            // Player Health
            txtDebugInfo.AppendText("▼ PLAYER HEALTH:\r\n");
            txtDebugInfo.AppendText("  Max Health: 100 HP\r\n");
            txtDebugInfo.AppendText("  Starting Health: 100 HP\r\n");
            txtDebugInfo.AppendText("  Health Pack Heal: 10-30 HP (random)\r\n");
            txtDebugInfo.AppendText("  Bonus Health at Milestones: +20 HP\r\n\r\n");
            
            // Level Progression
            txtDebugInfo.AppendText("▼ LEVEL PROGRESSION:\r\n");
            txtDebugInfo.AppendText("  Formula: log10(score + 1) × 5\r\n");
            txtDebugInfo.AppendText("  Level increases every ~30-40 kills\r\n");
            txtDebugInfo.AppendText("  Difficulty scales logarithmically\r\n\r\n");
            
            // Pickup Spawn Rates
            txtDebugInfo.AppendText("▼ PICKUP SPAWN RATES:\r\n");
            txtDebugInfo.AppendText("  Ammo Box: 0.6% per frame (~6/1000)\r\n");
            txtDebugInfo.AppendText("  Health Pack: 0.15% per frame (~3/2000)\r\n\r\n");
            
            // Game Timer
            txtDebugInfo.AppendText("▼ GAME SETTINGS:\r\n");
            txtDebugInfo.AppendText("  Game Tick: 20ms (50 FPS)\r\n");
            txtDebugInfo.AppendText("  Bullet Tick: 20ms\r\n\r\n");
            
            // Controls Reference
            txtDebugInfo.AppendText("═══════════════════════════════════════\r\n");
            txtDebugInfo.AppendText("     CONTROLS REFERENCE\r\n");
            txtDebugInfo.AppendText("═══════════════════════════════════════\r\n");
            txtDebugInfo.AppendText("  Arrow Keys: Move player\r\n");
            txtDebugInfo.AppendText("  Space: Shoot\r\n");
            txtDebugInfo.AppendText("  F11: Toggle fullscreen\r\n");
            txtDebugInfo.AppendText("  Enter: Restart (when dead)\r\n\r\n");
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            // Close the debug menu
            this.Close();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            // Reload debug information
            LoadDebugInfo();
            MessageBox.Show("Debug info refreshed!", "Debug Menu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Label click event (if needed)
        }
    }
}
