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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
            // Display current weapon when form loads
            UpdateWeaponDisplay();
            
            // Hook up SelectWeapon button click event
            SelectWeapon.Click += SelectWeapon_Click;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 startgame = new Form1();
            startgame.ShowDialog(); 
            this.Close();
        }

        private void DebugButton_Click(object sender, EventArgs e)
        {
            Form3 debugMenu = new Form3();
            debugMenu.ShowDialog(); // Opens debug menu as modal dialog
        }

        private void SelectWeapon_Click(object sender, EventArgs e)
        {
            // Open weapon selection form
            Form4 weaponSelection = new Form4();
            weaponSelection.ShowDialog();
            
            // Update weapon display after returning from Form4
            UpdateWeaponDisplay();
        }

        private void UpdateWeaponDisplay()
        {
            // Display the currently selected weapon in the Label
            CurrentWeapon.Text = WeaponManager.GetWeaponName();
        }
    }
}
