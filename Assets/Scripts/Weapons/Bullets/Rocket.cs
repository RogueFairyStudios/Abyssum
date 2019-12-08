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
					EntityBase entity = colliders[i].GetComponent<EntityBase>();
					entity.Damage(damage, 0);
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

		private void OnDrawGizmos() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, explosionRadius);
		}
	}
}