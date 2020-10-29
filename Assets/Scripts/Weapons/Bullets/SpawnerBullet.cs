using UnityEngine;

using DEEP.Decals;
using DEEP.Pooling;

namespace DEEP.Weapons.Bullets {

	// Bullet that spawns another object after hitting something.
	[RequireComponent(typeof(Rigidbody))]
	public class SpawnerBullet : BulletBase {

		[Tooltip("Prefab to be spawned when hitting something.")]
        [SerializeField] protected GameObject prefabToSpawn;

		protected override void OnCollisionEnter(Collision col) {

			SpawnPrefab();

			// Spawns decal if avaliable.
			if(bulletHoleMaterial != null) {
				DecalSystem.Instance.PlaceDecal(bulletHoleMaterial, col.contacts[0].point, col.contacts[0].normal, bulletHoleScale);
			}

			//Destroys the object on collision.
			Destroy(gameObject);

		}

		private void SpawnPrefab() {
			if (prefabToSpawn == null) {
				Debug.LogWarning("Missing prefab attribute!");
				return;
			}
			PoolingSystem.Instance.PoolObject(prefabToSpawn, transform.position, Quaternion.identity);
		}
	}
}