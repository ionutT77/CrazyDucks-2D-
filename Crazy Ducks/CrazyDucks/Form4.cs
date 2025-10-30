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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            
            // Hook up button click events
            pistol.Click += Pistol_Click;
            riffle.Click += Riffle_Click;
            sniper.Click += Sniper_Click;
            
            // Highlight the currently selected weapon
            HighlightCurrentWeapon();
        }

        private void Pistol_Click(object sender, EventArgs e)
        {
            WeaponManager.SetWeapon(WeaponType.Pistol);
            MessageBox.Show(WeaponManager.GetWeaponDescription(), 
                          "Weapon Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void Riffle_Click(object sender, EventArgs e)
        {
            WeaponManager.SetWeapon(WeaponType.Rifle);
            MessageBox.Show(WeaponManager.GetWeaponDescription(), 
                          "Weapon Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void Sniper_Click(object sender, EventArgs e)
        {
            WeaponManager.SetWeapon(WeaponType.Sniper);
            MessageBox.Show(WeaponManager.GetWeaponDescription(), 
                          "Weapon Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void HighlightCurrentWeapon()
        {
            // Reset all buttons to default color
            pistol.BackColor = SystemColors.Control;
            riffle.BackColor = SystemColors.Control;
            sniper.BackColor = SystemColors.Control;
            
            // Highlight the currently selected weapon
            switch (WeaponManager.CurrentWeapon)
            {
                case WeaponType.Pistol:
                    pistol.BackColor = Color.LightGreen;
                    break;
                case WeaponType.Rifle:
                    riffle.BackColor = Color.LightGreen;
                    break;
                case WeaponType.Sniper:
                    sniper.BackColor = Color.LightGreen;
                    break;
            }
        }
    }
}
