using System.Collections;

using UnityEngine;

using DEEP.Pooling;

namespace DEEP.Entities
{

    public class SentryEnemy : EnemyBase
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

        protected float delayTimer = 0.0f;


        protected override void Start() {

            base.Start();

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
                if(Vector3.Distance(TargetPlayer.transform.position, transform.position) <= sentry.attackRange) {
                    Attack();
                    delayTimer = 0.0f;
                }

                yield return new WaitForFixedUpdate();

            }

        }

        protected void Attack() {

            // Plays animations.
            enemyAnimator.SetBool("Attack", true);

            // Plays a particle effect.
            sentry.particles.Play();

            // Plays the attack audio.
            _audio.clip = sentry.attackClip;
            _audio.Play();

            // Spawns the attack.
            PoolingSystem.Instance.PoolObject(sentry.attackObject, sentry.attackSpawn.position, sentry.attackSpawn.rotation);

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
