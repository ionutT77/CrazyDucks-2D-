using System;

namespace CrazyDucks
{
    // Enum to define weapon types
    public enum WeaponType
    {
        Pistol,
        Rifle,
        Sniper
    }

    // Base class for Weapon
    public abstract class Weapon
    {
        public abstract string Name { get; }
        public abstract int BulletSpeed { get; }
        public abstract int FireCooldown { get; }
        public abstract bool BulletsShrink { get; }
        public abstract bool IsAutomatic { get; }
        public abstract string GetDescription();
        public abstract bool CanFire(DateTime lastShotTime);
    }

    // Pistol class inheriting from Weapon
    public class Pistol : Weapon
    {
        public override string Name => "Pistol";
        public override int BulletSpeed => 12;
        public override int FireCooldown => 300;
        public override bool BulletsShrink => true;
        public override bool IsAutomatic => false;

        public override string GetDescription()
        {
            return "Pistol - Balanced weapon\n\n" +
                   "• Normal bullet speed\n" +
                   "• 300ms cooldown between shots\n" +
                   "• Semi-automatic fire\n" +
                   "• Balanced for all situations";
        }

        public override bool CanFire(DateTime lastShotTime)
        {
            TimeSpan timeSinceLastShot = DateTime.Now - lastShotTime;
            return timeSinceLastShot.TotalMilliseconds >= FireCooldown;
        }
    }

    // Rifle class inheriting from Weapon
    public class Rifle : Weapon
    {
        public override string Name => "Rifle";
        public override int BulletSpeed => 15;
        public override int FireCooldown => 100;
        public override bool BulletsShrink => true;
        public override bool IsAutomatic => true;

        public override string GetDescription()
        {
            return "Rifle - Automatic weapon\n\n" +
                   "• Fast bullet speed\n" +
                   "• 100ms cooldown (rapid fire)\n" +
                   "• Fully automatic fire\n" +
                   "• Hold space to fire continuously";
        }

        public override bool CanFire(DateTime lastShotTime)
        {
            TimeSpan timeSinceLastShot = DateTime.Now - lastShotTime;
            return timeSinceLastShot.TotalMilliseconds >= FireCooldown;
        }
    }

    // Sniper class inheriting from Weapon
    public class Sniper : Weapon
    {
        public override string Name => "Sniper";
        public override int BulletSpeed => 20;
        public override int FireCooldown => 1000;
        public override bool BulletsShrink => false;
        public override bool IsAutomatic => false;

        public override string GetDescription()
        {
            return "Sniper - High-power weapon\n\n" +
                   "• Very fast bullets (don't shrink)\n" +
                   "• 1000ms cooldown between shots\n" +
                   "• Semi-automatic fire\n" +
                   "• High difficulty, high reward";
        }

        public override bool CanFire(DateTime lastShotTime)
        {
            // Sniper has stricter cooldown enforcement
            TimeSpan timeSinceLastShot = DateTime.Now - lastShotTime;
            return timeSinceLastShot.TotalMilliseconds >= FireCooldown;
        }
    }

    // Static class to manage the currently selected weapon
    public static class WeaponManager
    {
        // Current weapon type (default is Pistol)
        public static WeaponType CurrentWeapon { get; set; } = WeaponType.Pistol;
        
        // Current weapon instance
        private static Weapon currentWeaponInstance = new Pistol();
        
        // Get the current weapon instance
        public static Weapon GetCurrentWeapon()
        {
            return currentWeaponInstance;
        }
        
        // Set the current weapon and create appropriate instance
        public static void SetWeapon(WeaponType weaponType)
        {
            CurrentWeapon = weaponType;
            
            // Create the appropriate weapon instance
            switch (weaponType)
            {
                case WeaponType.Pistol:
                    currentWeaponInstance = new Pistol();
                    break;
                case WeaponType.Rifle:
                    currentWeaponInstance = new Rifle();
                    break;
                case WeaponType.Sniper:
                    currentWeaponInstance = new Sniper();
                    break;
                default:
                    currentWeaponInstance = new Pistol();
                    break;
            }
        }

        // Get weapon display name
        public static string GetWeaponName()
        {
            return currentWeaponInstance.Name;
        }

        // Get weapon properties using the weapon instance
        public static int GetBulletSpeed()
        {
            return currentWeaponInstance.BulletSpeed;
        }

        public static int GetFireCooldown()
        {
            return currentWeaponInstance.FireCooldown;
        }

        public static bool DoesWeaponShrink()
        {
            return currentWeaponInstance.BulletsShrink;
        }

        public static bool IsAutomatic()
        {
            return currentWeaponInstance.IsAutomatic;
        }
        
        // Get weapon description
        public static string GetWeaponDescription()
        {
            return currentWeaponInstance.GetDescription();
        }
        
        // Check if weapon can fire
        public static bool CanFire(DateTime lastShotTime)
        {
            return currentWeaponInstance.CanFire(lastShotTime);
        }
    }
}
