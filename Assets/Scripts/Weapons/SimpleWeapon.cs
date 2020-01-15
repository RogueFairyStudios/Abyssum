using UnityEngine;

using DEEP.Entities;

namespace DEEP.Weapons {

    // Base script for a simple weapons that fires common bullets.
    public class SimpleWeapon : WeaponBase {

        [Tooltip("If the weapon is owned by a player")]
        [SerializeField] protected bool isPlayerWeapon = false;
        [SerializeField] protected LayerMask raycastMask = new LayerMask();

        [Tooltip("Where the bullet should be spawned.")]
        [SerializeField] protected Transform bulletSpawn = null;

        [Tooltip("Where the bullet should be spawned.")]
        [SerializeField] protected Transform bulletPrefab = null;

        [Tooltip("Amount of time to wait between two consecutive shots.")]
        [SerializeField] protected float delayBetweenShots = 0.3f;
        protected float delayTimer = 0; //Used to count the time between shots.

        protected Animator _animator; // Stores the weapon's Animator.
        protected AudioSource _audio; // Stores the weapon's AudioSource.

        [Tooltip("AudioClip to be played when shooting.")]
        [SerializeField] protected AudioClip shotClip = null;

        [Tooltip("AudioClip to be played when trying to shoot whitout ammo.")]
        [SerializeField] protected AudioClip noAmmoCLip = null;

        protected virtual void Start()
        {

            // Allows the weapon to be fired at start.
            delayTimer = delayBetweenShots;

            // Gets the weapon's animator.
            _animator = GetComponentInChildren<Animator>();

            // Gets the weapon's AudioSource.
            _audio = GetComponentInChildren<AudioSource>();
            

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
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out rayHit, 1000f, raycastMask))
                    {
                        // Looks to the point.
                        if (rayHit.distance > 0.5f) // If a point is too clsoe weird angles may happen, so uses a minimum distance
                            bulletSpawn.LookAt(rayHit.point);
                        else
                            bulletPrefab.LookAt(Camera.main.transform.position + Camera.main.transform.forward * 0.5f);
                    }

                }

                return base.Shot();

            }

            return false;

        }

        // Fires the weapon.
        protected override void Fire() 
        {

            Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation); // Creates the bullet.
            delayTimer = 0; // Resets the delay.

            // Plays the animation.
            if(_animator != null) {
                _animator.SetBool("Fire", true);
                _animator.SetBool("NoAmmo", false);
            }

            // Plays the audio.
            if(_audio != null) {
                _audio.clip = shotClip;
                _audio.Play();
            }

        }

         // Do something when not enough ammo is avaliable.
        protected override void NoAmmo() 
        {

            delayTimer = 0; // Resets the delay.

            // Plays the audio.
            if(_audio != null) {
                _audio.clip = noAmmoCLip;
                _audio.Play();
            }

        }

    }

}