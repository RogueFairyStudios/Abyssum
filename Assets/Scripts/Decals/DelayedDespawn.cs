using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using DEEP.Pooling;

namespace DEEP.Utility
{
    // Destroy an object after a delay.
    public class DelayedDespawn : PoolableObject {

        [SerializeField] private float delay = 2.0f;

        [SerializeField] private UnityEvent onDestroyEvents = null;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        void Start()
        {
            StartCoroutine(DestroyAfterDelay(delay));
        }

        // Update is called once per frame
        IEnumerator DestroyAfterDelay(float delay)
        {
            
            float time = 0.0f;
            while(time < delay) {
                time += Time.fixedDeltaTime;
                yield return waitForFixed;
            }

            if(onDestroyEvents != null)
                onDestroyEvents.Invoke();

            Despawn();
            
        }
    }
}
