using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace  DEEP.Utility
{
    // Destroy an object after a delay.
    public class DelayedDestroy : MonoBehaviour {

        [SerializeField] private float delay = 2.0f;

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
                yield return new WaitForFixedUpdate();
            }

            Destroy(gameObject);
            
        }
    }
}
