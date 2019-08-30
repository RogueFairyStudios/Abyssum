using UnityEngine;

namespace DEEP.Weapons
{

    // Base script for a simple weapons that fires common bullets..
    public class SimpleWeapon : WeaponBase
    {

        [Tooltip("Where the bullet should be spawned.")]
        [SerializeField] protected Transform bulletSpawn = null;
        [Tooltip("Where the bullet should be spawned.")]
        [SerializeField] protected Transform bulletPrefab = null;

        [Tooltip("Amount of time to wait between two consecutive shots.")]
        [SerializeField] protected float delayBetweenShots = 0.3f;
        private float delayTimer = 0; //Used to count the time between shots.

        protected virtual void Start()
        {

            // Allows the weapon to be fired at start.
            delayTimer = delayBetweenShots;

        }

        protected virtual void Update()
        {

            // Waits for the delay between shots.
            if(delayTimer < delayBetweenShots)
                delayTimer += Time.deltaTime;

        }


        // Fires the weapon.
        protected override void Fire() 
        {

            // Verifies if the weapon can be fired.
            if(delayTimer >= delayBetweenShots)
            {
                Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation); // Creates the bullet.
                delayTimer = 0; // Resets the delay.
            }

        }

         // Do something when not enough ammo is avaliable.
        protected override void NoAmmo() {}

    }

}