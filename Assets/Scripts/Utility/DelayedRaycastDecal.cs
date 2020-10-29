using System.Collections;

using UnityEngine;

namespace DEEP.Decals {

    // Waits for a delay before casting a ray and placing a decal where it hits.
    public class DelayedRaycastDecal : MonoBehaviour
    {

        [SerializeField] private float delay = 2.0f;

        [SerializeField] protected Material decalMaterial = null;
        [SerializeField] protected Vector2 decalScale = Vector2.one;
        [SerializeField] protected Vector3 decalDir = Vector3.down;
        [SerializeField] protected Vector3 rayOffset = Vector3.zero;

        [SerializeField] protected float maxDecalDistance = 2.0f;

        [SerializeField] protected LayerMask raycastMask = new LayerMask();

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        void Start()
        {

            if(delay == 0.0f)
                Cast();
            else
                StartCoroutine(CastAfterDelay(delay));
        }

        void Cast() {

            RaycastHit hit;
            if(Physics.Raycast(transform.position + rayOffset, decalDir, out hit, maxDecalDistance, raycastMask, QueryTriggerInteraction.UseGlobal)) {
                DecalSystem.Instance.PlaceDecal(decalMaterial, hit.point, hit.normal, decalScale);
            }

        }

        IEnumerator CastAfterDelay(float delay)
        {
            
            float time = 0.0f;
            while(time < delay) {
                time += Time.fixedDeltaTime;
                yield return waitForFixed;
            }

            Cast();

        }
    }

}
