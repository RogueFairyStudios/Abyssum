using UnityEngine;

using DEEP.Pooling;
using DEEP.Weapons.Bullets;

namespace DEEP.Weapons {

    // Base script for a simple weapons that fires common bullets.
    public class SimpleWeapon : WeaponBase {

        [Tooltip("If the weapon is owned by a player")]
        [SerializeField] protected bool isPlayerWeapon = false;
        [SerializeField] protected LayerMask raycastMask = new LayerMask();

        [Tooltip("Where the bullet should be spawned.")]
        [SerializeField] protected Transform bulletSpawn = null;

        [Tooltip("Where the bullet should be spawned.")]
        [SerializeField] protected GameObject bulletPrefab = null;

        [Tooltip("Max amount of this type of bullet that can exist in the game at the same time.")]
        [SerializeField] protected int maxBulletInstances = 128;

        [Tooltip("Amount of time to wait between two consecutive shots.")]
        [SerializeField] protected float delayBetweenShots = 0.3f;
        protected float delayTimer = 0; //Used to count the time between shots.

        protected Animator wAnimator; // Stores the weapon's Animator.
        protected AudioSource wAudio; // Stores the weapon's AudioSource.

        [Tooltip("AudioClip to be played when shooting.")]
        [SerializeField] protected AudioClip shotClip = null;

        [Tooltip("AudioClip to be played when trying to shoot whitout ammo.")]
        [SerializeField] protected AudioClip noAmmoCLip = null;

        protected virtual void Start()
        {

            // Allows the weapon to be fired at start.
            delayTimer = delayBetweenShots;

            // Gets the weapon's animator.
            wAnimator = GetComponentInChildren<Animator>();

            // Gets the weapon's AudioSource.
            wAudio = GetComponentInChildren<AudioSource>();            

        }

        protected virtual void FixedUpdate()
        {

            // Waits for the delay between shots.
            if(delayTimer < delayBetweenShots)
                delayTimer += Time.fixedDeltaTime;

        }

        // Attempts to fire the weapon.
        public override bool Shot()
        {

            // Verifies if the weapon can be fired.
            if (delayTimer >= delayBetweenShots)
            {
                // Ensures bullet spawn is facing the exact center of the screen if marked.
                if (isPlayerWeapon)
                {

                    // Gets a point far away into the horizon.
                    RaycastHit rayHit;
                    if(Physics.Linecast(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 5000.0f, out rayHit, raycastMask, QueryTriggerInteraction.Ignore)) {
                        bulletSpawn.LookAt(rayHit.point); // Looks to the collision point.
                    } else { // Looks to a point far away.
                        bulletSpawn.LookAt(Camera.main.transform.position + Camera.main.transform.forward * 5000.0f);
                    }

                }

                return base.Shot();

            }

            return false;

        }

        /// <summary>
        ///     Tries to shoot the weapon, with a target in mind. Meant for enemies, mainly.
        ///     Given the current code structure, I didn't want to change it too much so this feels really garbage. I'll probably clean it up later,
        /// but that will require changing the structure and base weapon class a little bit.
        /// </summary>
        /// <param name="target"> The bullet target </param>
        /// <returns> If the weapon was able to shoot </returns>
        public bool Shot(Vector3 target)
        {
             // Verifies if the weapon can be fired.
            if (delayTimer >= delayBetweenShots)
            {
                if(ammoSource != null && ammoUsage > 0) // Verifies if an ammoSource is assigned and the weapons needs ammo.
                {
                    if(ammoSource.HasAmmo(ammoUsage)) // If the weapon has an ammoSource, checks for enough ammo.
                    {

                        // Uses the ammo and performs the shot.
                        ammoSource.UseAmmo(ammoUsage);
                        Fire(target);
                        return true;

                    }
                    else {
                        NoAmmo(); // Do something when not enough ammo is avaliable.
                        return false;
                    }
                }

                Fire(target); // If no ammoSource is assigned or the weapon doesn't use ammo simply fire.
                return true;
            }
            else
                return false;
        }

        // Fires the weapon.
        protected override void Fire() 
        {              

            PoolingSystem.Instance.PoolObject(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            delayTimer = 0; // Resets the delay.

            // Plays the animation.
            if(wAnimator != null) {
                wAnimator.SetBool("Fire", true);
                wAnimator.SetBool("NoAmmo", false);
            }

            // Plays the audio.
            if(wAudio != null) {
                wAudio.clip = shotClip;
                wAudio.Play();
            }

        }

        /// <summary>
        ///     Again, this is just copy paste the other fire function because I didn't want to change the code structure.
        ///     Will probably clean up later.
        /// </summary>
        /// <param name="target"></param>
        protected virtual void Fire(Vector3 target)
        {
            PoolingSystem.Instance.PoolObject(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation).GetComponent<Bullets.BulletBase>().SetTarget(target); // Creates the bullet and sets its target.
            delayTimer = 0; // Resets the delay.

            // Plays the animation.
            if(wAnimator != null) {
                wAnimator.SetBool("Fire", true);
                wAnimator.SetBool("NoAmmo", false);
            }

            // Plays the audio.
            if(wAudio != null) {
                wAudio.clip = shotClip;
                wAudio.Play();
            }
        }

        // Do something when not enough ammo is avaliable.
        protected override void NoAmmo() 
        {

            delayTimer = 0; // Resets the delay.

            // Plays the audio.
            if(wAudio != null) {
                wAudio.clip = noAmmoCLip;
                wAudio.Play();
            }

        }

    }

}