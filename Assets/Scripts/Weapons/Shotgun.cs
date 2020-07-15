using UnityEngine;

namespace DEEP.Weapons {

	// Base script for a simple weapons that fires common bullets..
	public class Shotgun : SimpleWeapon {

        [Tooltip("The amount of pellets (projectiles) fired per shot.")]
        [SerializeField] protected int pelletsPerShot = 6;
        [Tooltip("The opening angle of the pellets fired.")]
        [Range(0.0f, 180.0f)]
        [SerializeField] protected float shotSpreadAngle = 90.0f;

		protected override void Fire() {

			for (int i = 0; i < pelletsPerShot; ++i) {
				Quaternion direction = GenerateRandomRotation(bulletSpawn.rotation);
				Instantiate(bulletPrefab, bulletSpawn.position, direction); // Creates the bullet.
			}
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
		protected override void Fire(Vector3 target)
		{
			
			// As shootgun bullets are random, there's no need to worry about the target.
			Fire();

		}

		protected Quaternion GenerateRandomRotation(Quaternion baseRotation) {
			float angleVariation = shotSpreadAngle / 2.0f;
			float xVariation = Random.Range(-angleVariation, angleVariation);
			float yVariation = Random.Range(-angleVariation, angleVariation);
			Quaternion randRotation = baseRotation * Quaternion.Euler(xVariation, yVariation, 0);

			return randRotation;
		}
	}		
}