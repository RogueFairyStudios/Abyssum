using System.Net;
using System.Collections.ObjectModel;
using UnityEngine;
using DEEP.Entities;

namespace DEEP.Weapons.Bullets {

	[RequireComponent(typeof(Rigidbody))]
	public class Rocket : BulletBase {

        [Tooltip("Radius of the explosion which will inflict damage.")]
        [SerializeField] protected float explosionRadius = 3.0f;
        [Tooltip("Force applied to the objects in the radius of the explosion.")]
        [SerializeField] protected float explosionForce = 5.0f;
        [SerializeField] protected GameObject explosionPrefab;
		
		[Space(5)]

		[Header("Rocket Jump")]
		[Tooltip("Damage when dealt to self.")]
		[SerializeField] protected int playerDamage;
		[Tooltip("Range in which the explosion will have maximum propulsion.")]
		[SerializeField] protected float maxPowerRadius;
		[Tooltip("Special, extra upwards force to rocket jump")]
		[SerializeField] protected float upwardsForce;

		protected override void OnCollisionEnter(Collision col) {
			print("Rocket collided. Now its time to explode!!");
			Explosion();
			SpawnExplosionPrefab();
			//Destroys the object on collision.
			Destroy(gameObject);

		}

		private void Explosion() {
			Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
			for (int i = 0; i < colliders.Length; ++i) {
				// Check if the collision object is an Entity, then inflict damage to it
				if (colliders[i].GetComponent(typeof(EntityBase)) != null) {
					// Check if it is the player specifically for rocket jumps and decreased damage
					if(colliders[i].GetComponent(typeof(Player)) != null) {
						Player player = colliders[i].GetComponent<Player>();
						Rigidbody playerRb = player.GetComponent<Rigidbody>();
						player.Damage(playerDamage, 0);
						// If within maxPowerRadius, apply explosive force without distance decay, otherwise, apply distance decay
						if(Vector3.Distance(this.transform.position, playerRb.transform.position) <= maxPowerRadius)
							playerRb.AddExplosionForce(explosionForce, transform.position, 0, upwardsForce, ForceMode.VelocityChange);
						else
							playerRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsForce, ForceMode.VelocityChange);
					}
					else {
						EntityBase entity = colliders[i].GetComponent<EntityBase>();
						entity.Damage(damage, 0);
					}
				}
				else {
					Rigidbody rigid = colliders[i].attachedRigidbody;
					if (rigid != null) {	// Check if the object has an rigidbody attached to it
						// Then add force to it to simulate the explosion physics
						rigid.AddExplosionForce(explosionForce, transform.position, explosionRadius);
					}
				}
			}
		}

		private void SpawnExplosionPrefab() {
			if (explosionPrefab == null) {
				Debug.LogWarning("Missing explosionPrefab attribute!");
				return;
			}
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		}

# if UNITY_EDITOR

		private void OnDrawGizmos() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, explosionRadius);
		}

# endif

	}
}