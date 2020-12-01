using UnityEngine;

using System.Collections;

using DEEP.Stage;
using DEEP.Pooling;

namespace DEEP.Entities
{
    public class CucumberEntity : StaticEnemy
    {

        protected SkinnedMeshRenderer meshRenderer;
        protected Rigidbody cRigidbody;
        protected Collider[] colliders;

        [Header("Death effect")]
        [Tooltip("Used to move the cucumber to \"fade\" the poll.")]
        [SerializeField] protected Vector3 deathPositionDelta = new Vector3(0, -3, 0);

        [Tooltip("The velocity in which the object is moved during the death effect.")]
        [SerializeField] protected float deathFadeVelocity = 0.1f;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();
        protected override void Start() {

            base.Start();

            // Gets the necessary components at start.
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            cRigidbody = GetComponentInChildren<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();

        }

        protected override void Die() {

            // Checks if the entity isn't already dead.
            if(isDead)
                return;
            isDead = true;

            // Hides the gameObject.
            meshRenderer.enabled = false;

            // Plays death audio
            if(death.Length > 0)
                AudioSource.PlayClipAtPoint(death[Random.Range(0, death.Length)], transform.position, _audio.volume);

            // Disable physics and all colliders on the object (with the exception of the slowness trigger).
            cRigidbody.useGravity = false;
            cRigidbody.isKinematic = true;
            foreach(Collider collider in colliders) {
                if (collider.isTrigger == false)
                    collider.enabled = false;
            }

            if (deathPrefab != null) // Spawns a prefab after death if assigned.
                PoolingSystem.Instance.PoolObject(deathPrefab, transform.position, transform.rotation);

            // Moves the object to "fade" the pool.
            StartCoroutine(DyingFade());

            // Counts this enemy's death as a kill.
            if(!IsSpawned)
                StageManager.Instance.CountKill();

        }

        protected IEnumerator DyingFade() {

            // Performs the death effect.
            Vector3 originalPosition = transform.position;

            while (Vector3.Distance(originalPosition, originalPosition + deathPositionDelta) > 0.05f) {
                transform.Translate(deathPositionDelta.normalized * deathFadeVelocity * Time.fixedDeltaTime);
                yield return waitForFixed;
            }

        }

    }

}
