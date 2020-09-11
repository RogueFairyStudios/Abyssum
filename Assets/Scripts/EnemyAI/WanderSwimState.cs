using UnityEngine;
using System.Collections;

namespace DEEP.AI
{
    [RequireComponent(typeof(BaseSwimAI))]
    public class WanderSwimState : MonoBehaviour
    {

        protected Vector3 initialPosition;
        protected Vector3 targetPosition;

        [SerializeField] private float minMovementDelay = 5.0f;
        [SerializeField] private float maxMovementDelay = 10.0f;

        [SerializeField] private Vector3 movementBoxSize = new Vector3();

        [SerializeField] private float movementSpeed = 2.0f;
        [SerializeField] private float rotationVelocity = 90.0f;

        BaseSwimAI baseSwimAI;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        private void Awake()
        {
            baseSwimAI = GetComponent<BaseSwimAI>();
            this.enabled = false;
        }

        protected void OnEnable()
        {
            // Initializes variables.
            initialPosition = transform.position;
            targetPosition = initialPosition;
            
            // Starts the movement cycle.
            StartCoroutine(WanderDelay());
        }

        private IEnumerator WanderDelay()
        {

            // Waits for a random time inside the allowed range.
            float delay = Random.Range(minMovementDelay, maxMovementDelay);
            float time = 0.0f;

            while(time < delay)
            {
                time += Time.fixedDeltaTime;
                yield return waitForFixed;
            }

            // Start movement.
            StartCoroutine(WanderAround());

        }

        private IEnumerator WanderAround()
        {
            Vector3 deltaPos;
            do
            {
                // Gets a random position.
                targetPosition = initialPosition + new Vector3(Random.Range(-movementBoxSize.x, movementBoxSize.x) / 2.0f,
                                                                Random.Range(-movementBoxSize.y, movementBoxSize.y) / 2.0f,
                                                                Random.Range(-movementBoxSize.z, movementBoxSize.z) / 2.0f);
                deltaPos = targetPosition - transform.position;
            }
            while (Physics.BoxCast(baseSwimAI._collider.bounds.center, baseSwimAI._collider.bounds.extents, deltaPos, Quaternion.LookRotation(deltaPos.normalized), deltaPos.magnitude)); // Makes sure there isn't anything in the way

            // Rotates the body.
            Quaternion rotate = Quaternion.LookRotation(deltaPos.normalized);
            while(Quaternion.Angle(transform.rotation, rotate) > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.fixedDeltaTime * Mathf.Deg2Rad * rotationVelocity);
                yield return new WaitForFixedUpdate();
            }

            // Clamps the rotation.
            transform.LookAt(targetPosition);

            // Swims.
            baseSwimAI._animator.SetBool("Swim", true);

            while(Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                // Moves
                deltaPos = targetPosition - transform.position;
                baseSwimAI._rigid.velocity = 1f/Time.fixedDeltaTime * deltaPos * movementSpeed * 0.01f;

                yield return new WaitForFixedUpdate();
            }

            // Stops swimming.
            baseSwimAI._animator.SetBool("Swim", false);
            
            // Restarts the cycle.
            StartCoroutine(WanderDelay());  

        }

        private void OnDrawGizmosSelected()
        {
            // Draws the random movement box.
            Gizmos.color = Color.cyan;
            if(initialPosition == Vector3.zero) // For when placing in the scene
                Gizmos.DrawWireCube(this.transform.position, movementBoxSize);
            else
                Gizmos.DrawWireCube(initialPosition, movementBoxSize);

            // Draws a line to the destination.
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, targetPosition);
        }

        protected void OnDisable()
        {
            // Stops all movement
            StopAllCoroutines();
        }
    }

}
