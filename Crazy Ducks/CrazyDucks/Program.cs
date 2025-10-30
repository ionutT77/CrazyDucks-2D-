using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


//add a debug menu: player speed for every level, bullet speed, ammo count, health duck and how much damage it takes-checked
//add a rifle automatic shooting continuous fire when mouse button is held down-checked

//pistol cooldown time between shots-checked
//add for cooldown a bodypart that shows the cooldown time left-checked
//add a sniper ,with bullets that doesn t shrink -checked
//add different bullet speed for every weapon -checked
//a class for every weapon that inheriten from a base class weapon -checked
//add a sound when shooting-checked
//add random collisions hard objects that you cannot pass through and generate them randomly-checked    
//add path finding for ducks so they can avoid obstacles-checked


namespace CrazyDucks
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }
}