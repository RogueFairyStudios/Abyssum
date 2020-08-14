using System.Collections;

using kTools.Decals;

using UnityEngine;

namespace DEEP.Utility {

    // Waits for a delay before casting a ray and placing a decal where it hits.
    public class DelayedRaycastDecal : MonoBehaviour
    {

         [SerializeField] private float delay = 2.0f;

        [SerializeField] protected DecalData decal = null;
        [SerializeField] protected Vector3 decalScale = Vector3.one;
        [SerializeField] protected Vector3 decalDir = Vector3.down;

        [SerializeField] protected float maxDecalDistance = 2.0f;

        [SerializeField] protected LayerMask raycastMask = new LayerMask();

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        void Start()
        {
            StartCoroutine(CastAfterDelay(delay));
        }

        IEnumerator CastAfterDelay(float delay)
        {
            
            float time = 0.0f;
            while(time < delay) {
                time += Time.fixedDeltaTime;
                yield return waitForFixed;
            }

            RaycastHit hit;
            if(Physics.Raycast(transform.position, decalDir, out hit, maxDecalDistance, raycastMask, QueryTriggerInteraction.UseGlobal)) {
                DecalSystem.GetDecal(decal, hit.point, -hit.normal, decalScale);
            }

        }
    }

}
