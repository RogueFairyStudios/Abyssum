using UnityEngine;

using System.Collections;

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

        // Used to mark the the fade death effect is happening.
        private bool isDying;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();
        protected override void Start() {

            base.Start();

            // Not dying at start.
            isDying = false;

            // Gets the necessary components at start.
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            cRigidbody = GetComponentInChildren<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();

        }

        protected override void Die() {

            // Ensures death doesn't happen twice.
            if (isDying) return;

            // Hides the gameObject.
            meshRenderer.enabled = false;

            // Disable physics and all colliders on the object (with the exception of the slowness trigger).
            cRigidbody.useGravity = false;
            cRigidbody.isKinematic = true;
            foreach(Collider collider in colliders) {
                if (collider.isTrigger == false)
                    collider.enabled = false;
            }

            if (deathPrefab != null) // Spawns a prefab after death if assigned.
                Instantiate(deathPrefab, transform.position, transform.rotation);

            // Moves the object to "fade" the pool.
            StartCoroutine(DyingFade());

            // Marks as dead.
            isDying = true;

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
