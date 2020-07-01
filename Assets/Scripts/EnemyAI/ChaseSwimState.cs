using UnityEngine;
using System.Collections;

namespace DEEP.AI
{
    [RequireComponent(typeof(BlowfishAI))]
    public class ChaseSwimState : MonoBehaviour
    {
        [SerializeField] private float minChasingSpeed = 0.0f, maxChasingSpeed = 6.0f, chasingSpeedScaling = 1.2f;
        [SerializeField] private float minChasingTurningSpeed = 16.0f, maxChasingTurningSpeed = 32.0f, chasingTurningSpeedScaling = 2.0f;
        [SerializeField] private float timeBeforeCharging = 0.3f;
        [SerializeField] private float pointOfNoReturn = 1.0f, timeBeforeExploding = 1.5f;
        [SerializeField] private float startExpandingRange = 5.0f;

        BlowfishAI blowfishAI;
        float fuseTime;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        private void Awake()
        {
            blowfishAI = GetComponent<BlowfishAI>();
        }

        private void OnEnable()
        {
            Debug.Log("Start chasing state");
            StartCoroutine(Chase());
        }

        private IEnumerator Chase()
        {
            float chasingTurningSpeed, chasingSpeed;
            bool chasing = true;
            Vector3 deltaPos;
            Quaternion rotate;

            do
            {
                // Update location
                blowfishAI.HasTargetSight();
                deltaPos = blowfishAI.lastTargetLocation - transform.position;
                rotate = Quaternion.LookRotation(deltaPos.normalized);

                // Face target
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.fixedDeltaTime * Mathf.Deg2Rad * maxChasingTurningSpeed * 10f);
                
                yield return waitForFixed;

                Debug.Log("Angle:" + Quaternion.Angle(transform.rotation, rotate));

            } while(Quaternion.Angle(transform.rotation, rotate) > 5f);

            // Waits before charging
            yield return new WaitForSeconds(timeBeforeCharging);

            // Clamps the rotation.
            transform.LookAt(blowfishAI.lastTargetLocation);
            rotate = Quaternion.LookRotation(deltaPos.normalized);

            // Swims.
            blowfishAI._animator.SetBool("Swim", true);

            while(chasing)
            {
                // Scale speeds with distance
                chasingTurningSpeed = Mathf.Lerp(minChasingTurningSpeed, maxChasingTurningSpeed, 1/(deltaPos.magnitude / chasingTurningSpeedScaling + 1));
                chasingSpeed = Mathf.Lerp(minChasingSpeed, maxChasingSpeed, (deltaPos.magnitude * chasingSpeedScaling) / blowfishAI.DetectRange);
                blowfishAI._animator.SetFloat("Speed", chasingSpeed / maxChasingSpeed);

                // Rotates
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.fixedDeltaTime * Mathf.Deg2Rad * chasingTurningSpeed * 10f);

                // Moves
                blowfishAI._rigid.velocity = 1f/Time.fixedDeltaTime * transform.forward * chasingSpeed * 0.1f;

                yield return waitForFixed;

                // Check Line of Sight
                bool withinLoS = blowfishAI.HasTargetSight();

                // Update distances
                deltaPos = blowfishAI.lastTargetLocation - transform.position;
                rotate = Quaternion.LookRotation(deltaPos.normalized);
                
                // If within sight, if it's within expanding range or past the point of no return, expand
                if(withinLoS && (deltaPos.magnitude < startExpandingRange || fuseTime >= pointOfNoReturn))
                {
                    // Set animation and sound to inflate
                    blowfishAI.Inflate();
                    blowfishAI._animator.SetBool("Attack", true);
                
                    // Inflate
                    fuseTime += Time.fixedDeltaTime;
                }
                else // Else, shrink back to normal
                {
                    blowfishAI.Deflate();
                    fuseTime -= Time.fixedDeltaTime;
                }
                
                // Clamps fuse time
                fuseTime = Mathf.Clamp(fuseTime, 0f, timeBeforeExploding);

                // Sets animation time
                blowfishAI._animator.SetFloat("FuseTime", fuseTime / timeBeforeExploding);
                
                // If the fuse reaches it's limit, explode
                if(fuseTime == timeBeforeExploding)
                    blowfishAI.Explode();
                // If it's shrunk back to normal size
                else if(fuseTime == 0f) // 
                {
                    // Stop expanding animation
                    blowfishAI._animator.SetBool("Attack", false);

                    // If it's reached the target's last known position and can't see him anymore
                    if(!withinLoS && deltaPos.magnitude < 0.5f)
                    {
                        // Go back to the wander state (Idle)
                        blowfishAI._animator.SetBool("Swim", false);
                        blowfishAI.Wander();
                        chasing = false;
                    }
                }
            }
        } 

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}