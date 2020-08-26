using UnityEngine;

using System.Collections;

using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.Utility
{

    public class DamageTrigger : MonoBehaviour
    {

        private PlayerEntity target;
        private Coroutine damageCoroutine;
        private float time = 0.0f;

        [SerializeField] private float delay = 0.2f;
        [SerializeField] private int amount = 10;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        void OnTriggerEnter(Collider other) 
        {

            // Checks if the player entered the trigger.
            target = other.GetComponent<PlayerEntity>();

            // Does the damage.
            if(target != null)
                damageCoroutine = StartCoroutine(DamageOverTime());

        }

        void OnTriggerExit(Collider other) 
        {

            // Checks if the player exited the trigger.
            target = other.GetComponent<PlayerEntity>();

            // Stop doing damage.
            if (target != null)
                StopCoroutine(damageCoroutine);
        
        }

        // Does damage from time to time.
        IEnumerator DamageOverTime()
        {

            while(true) 
            {

                time += Time.fixedDeltaTime;

                if(time >= delay) {

                    if(target != null) {
                        target.Damage(amount, DamageType.Regular);
                        time = 0.0f;
                    }

                }

                yield return waitForFixed;

            }

        }
    }
}
