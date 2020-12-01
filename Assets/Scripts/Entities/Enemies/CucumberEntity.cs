using UnityEngine;

using System.Collections;

using DEEP.Stage;
using DEEP.Pooling;

namespace DEEP.Entities
{
    public class CucumberEntity : EnemyBase
    {

        protected Rigidbody cRigidbody;
        protected Collider[] colliders;

        [Header("Death effect")]
        [Tooltip("Used to move the cucumber to \"fade\" the poll.")]
        [SerializeField] protected Vector3 deathPositionDelta = new Vector3(0, -3, 0);

        protected override void Start() {

            base.Start();

            // Gets the necessary components at start.
            cRigidbody = GetComponentInChildren<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();

        }

        protected override void Die() {

            // Checks if the entity isn't already dead.
            if(isDead)
                return;

            base.Die();

            // Hides the gameObject.
            enemyRenderer.enabled = false;

            // Disable physics and all colliders on the object (with the exception of the slowness trigger).
            cRigidbody.useGravity = false;
            cRigidbody.isKinematic = true;
            foreach(Collider collider in colliders) {
                if (collider.isTrigger == false)
                    collider.enabled = false;
            }

            // Spawns here to cover the cucumber dissapearing.
            if(deathPrefab != null) // Spawns a prefab after death if assigned.
                PoolingSystem.Instance.PoolObject(deathPrefab, transform.position, transform.rotation);

        }

        protected override IEnumerator DespawnDelay() {

            // Performs the death effect.
            Vector3 originalPosition = transform.position;

            // Calculates the fade velocity.
            float deathFadeVelocity = 0.0f;
            if(despawnDeathDelay != 0.0f)
                deathFadeVelocity = deathPositionDelta.magnitude / despawnDeathDelay;

            while (Vector3.Distance(originalPosition, originalPosition + deathPositionDelta) > 0.05f) {
                transform.Translate(deathPositionDelta.normalized * deathFadeVelocity * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }

            Despawn();

        }

        protected override void Despawn() { Destroy(gameObject); }

    }

}
