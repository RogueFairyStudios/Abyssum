using UnityEngine;

namespace DEEP.Weapons {

	public class MachineGun : WeaponBase {

		[Tooltip("Where the bullet should be spawned.")]
		[SerializeField] protected Transform bulletSpawn = null;
		[Tooltip("Where the bullet should be spawned.")]
		[SerializeField] protected Transform bulletPrefab = null;

		[Tooltip("Amount of time to wait between two consecutive shots.")]
		[SerializeField] protected float delayBetweenShots = 0.1f;
		private float delayTimer = 0; //Used to count the time between shots.

		private Animator _animator; // Stores the weapon's Animator.
		private AudioSource _audio; // Stores the weapon's AudioSource.

		[Tooltip("AudioClip to be played when shooting.")]
		[SerializeField] protected AudioClip shotClip = null;

		[Tooltip("AudioClip to be played when trying to shoot whitout ammo.")]
		[SerializeField] protected AudioClip noAmmoCLip = null;

		protected virtual void Start() {

			// Allows the weapon to be fired at start.
			delayTimer = delayBetweenShots;

			// Gets the weapon's animator.
			_animator = GetComponentInChildren<Animator>();
			if(_animator == null) Debug.LogError("DEEP.Weapons.SimpleWeapon.Start: Animator not found!");

			// Gets the weapon's AudioSource.
			_audio = GetComponentInChildren<AudioSource>();
			if(_audio == null) Debug.LogError("DEEP.Weapons.SimpleWeapon.Start: AudioSource not found!");

		}

		protected virtual void Update() {

			// Waits for the delay between shots.
			if(delayTimer < delayBetweenShots)
				delayTimer += Time.deltaTime;

		}

		// Attempts to fire the weapon.
		public override void Shot() {

			// Verifies if the weapon can be fired.
			if(delayTimer >= delayBetweenShots)
				base.Shot();

		}

		// Fires the weapon.
		protected override void Fire() {

			Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation); // Creates the bullet.
			delayTimer = 0; // Resets the delay.

			// Plays the animation.
			_animator.SetBool("Fire", true);
			_animator.SetBool("NoAmmo", false);

			// Plays the audio.
			_audio.clip = shotClip;
			_audio.Play();

		}

		 // Do something when not enough ammo is avaliable.
		protected override void NoAmmo() {

			delayTimer = 0; // Resets the delay.

			// Plays the audio.
			_audio.clip = noAmmoCLip;
			_audio.Play();

		}
	}
}