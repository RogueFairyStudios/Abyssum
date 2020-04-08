using System.Net;
using System.Collections.ObjectModel;
using UnityEngine;
using DEEP.Entities;

namespace DEEP.Weapons.Bullets {

	// Bullet that spawns another object after hitting something.
	[RequireComponent(typeof(Rigidbody))]
	public class SpawnerBullet : BulletBase {

		[Tooltip("Prefab to be spawned when hitting something.")]
        [SerializeField] protected GameObject prefabToSpawn;

		protected override void OnCollisionEnter(Collision col) {

			SpawnPrefab();
			//Destroys the object on collision.
			Destroy(gameObject);

		}

		private void SpawnPrefab() {
			if (prefabToSpawn == null) {
				Debug.LogWarning("Missing prefab attribute!");
				return;
			}
			Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
		}
	}
}