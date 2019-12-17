using UnityEngine;

namespace DEEP.Weapons
{

    // Base script for a weapon.
    public abstract class WeaponBase : MonoBehaviour
    {

        [Tooltip("How much ammo is used for each shot.")]
        [SerializeField] private int ammoUsage = 1;

        // Reference to the ammo source to use.
        [HideInInspector] public AmmoSource ammoSource;

        // Attempts to fire the weapon.
        public virtual bool Shot()
        {

            if(ammoSource != null && ammoUsage > 0) // Verifies if an ammoSource is assigned and the weapons needs ammo.
            {
                if(ammoSource.HasAmmo(ammoUsage)) // If the weapon has an ammoSource, checks for enough ammo.
                {

                    // Uses the ammo and performs the shot.
                    ammoSource.UseAmmo(ammoUsage);
                    Fire();
                    return true;

                }
                else {
                    NoAmmo(); // Do something when not enough ammo is avaliable.
                    return false;
                }
            }

            Fire(); // If no ammoSource is assigned or the weapon doesn't use ammo simply fire.
            return true;

        }

        // Fires the weapon.
        protected abstract void Fire();

         // Do something when not enough ammo is avaliable.
        protected abstract void NoAmmo();

    }

}