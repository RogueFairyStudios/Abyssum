using System.Collections;

using UnityEngine;

using DEEP.Entities.Player;

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

            public ParticleSystem particles;

            public AudioClip attackClip;

        }

        [SerializeField] protected Sentry sentry;

        protected Animator animator;
        protected float delayTimer = 0.0f;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

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
                    yield return waitForFixed;

                }

                // Makes the attack if the player is in range.
                if(Vector3.Distance(PlayerController.Instance.transform.position, transform.position) <= sentry.attackRange) {

                    Attack();
                    delayTimer = 0.0f;

                }

                yield return waitForFixed;

            }

        }

        protected void Attack() {

            // Plays animations.
            animator.SetBool("Attack", true);

            // Plays a particle effect.
            sentry.particles.Play();

            // Plays the attack audio.
            _audio.clip = sentry.attackClip;
            _audio.Play();

            // Spawns the attack.
            Instantiate(sentry.attackObject, sentry.attackSpawn.position, sentry.attackSpawn.rotation);

        }

        public override void Damage(int amount, DamageType type) {

            base.Damage(amount, type);

            if (delayTimer >= sentry.attackDelay) {
                Attack();
                delayTimer = 0.0f;
            }

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
