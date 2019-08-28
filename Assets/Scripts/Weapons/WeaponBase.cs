using UnityEngine;

namespace DEEP.Weapons
{

    // Base script for a weapon.
    public abstract class WeaponBase : MonoBehaviour
    {

        [Header("Ammo")]
        [Tooltip("Ammo source for this weapon.")]
        public AmmoSource ammoSource;

        [Tooltip("How much ammo is used for each shot.")]
        [SerializeField] private int ammoUsage = 1;

        // Attempts to fire the weapon.
        public virtual void Shot()
        {

            if(ammoSource != null) // Verifies if an ammoSource is assigned.
            {
                if(ammoSource.HasAmmo(ammoUsage)) // If the weapon has an ammoSource, checks for enough ammo.
                {

                    // Uses the ammo and performs the shot.
                    ammoSource.UseAmmo(ammoUsage);
                    Fire();

                }
                else NoAmmo(); // Do something when not enough ammo is avaliable.
            }
            else Fire(); // If no ammoSource is assigned simple fire.

        }

        // Fires the weapon.
        protected abstract void Fire();

         // Do something when not enough ammo is avaliable.
        protected abstract void NoAmmo();

    }

}