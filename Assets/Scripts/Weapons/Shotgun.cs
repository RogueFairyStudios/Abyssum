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
				Debug.Log("baseRotation = " + bulletSpawn.rotation.eulerAngles);
				Quaternion direction = GenerateRandomRotation(bulletSpawn.rotation);
				Debug.Log("RandRotation = " + direction.eulerAngles);
				Instantiate(bulletPrefab, bulletSpawn.position, direction); // Creates the bullet.
			}
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

		private Quaternion GenerateRandomRotation(Quaternion baseRotation) {
			float angleVariation = shotSpreadAngle / 2.0f;
			float xVariation = Random.Range(-angleVariation, angleVariation);
			float yVariation = Random.Range(-angleVariation, angleVariation);
			Quaternion randRotation = baseRotation * Quaternion.Euler(xVariation, yVariation, 0);

			return randRotation;
		}

	}

}