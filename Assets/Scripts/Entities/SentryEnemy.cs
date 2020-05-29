using DEEP.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.Entities
{

    public class SentryEnemy : StaticEnemy
    {

        [System.Serializable]
        protected class Sentry {

            public float attackDelay = 4.0f;
            public float attackRange = 5.0f;

            public GameObject attackObject;
            public Transform attackSpawn;

        }

        [SerializeField] protected Sentry sentry;

        protected Animator animator;
        protected float delayTimer = 0.0f;

        protected override void Start() {

            base.Start();
            //Invoke(nameof(Grunt), Random.Range(minGruntInterval, maxGruntInterval));

            animator = GetComponentInChildren<Animator>();

            delayTimer = 0.0f;
            StartCoroutine(SentryBehaviour());

        }

        protected IEnumerator SentryBehaviour() {

            while(true) {

                // Waits for the attack delay.
                while (delayTimer < sentry.attackDelay) {

                    delayTimer += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();

                }

                // Makes the attack if the player is in range.
                if(Vector3.Distance(Player.Instance.transform.position, transform.position) <= sentry.attackRange) {

                    Attack();
                    delayTimer = 0.0f;

                }

                yield return new WaitForFixedUpdate();

            }

        }

        protected void Attack() {

            animator.SetBool("Attack", true);
            Instantiate(sentry.attackObject, sentry.attackSpawn.position, sentry.attackSpawn.rotation);

        }

#if UNITY_EDITOR
        private void OnDrawGizmos() // To visualize the attack range.
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sentry.attackRange);
        }
#endif

    }

}
