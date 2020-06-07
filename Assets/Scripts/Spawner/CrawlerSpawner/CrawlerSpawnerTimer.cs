using UnityEngine;

namespace DEEP.Spawn
{
    public class CrawlerSpawnerTimer : Spawner
    {

        [Tooltip("Number of entities to spawn. Use -1 for infinity spawn.")]
        [SerializeField] protected int amount = -1;

        private int currentSpawned;

        [SerializeField] protected Animator animator;

        protected override void Start() {

            if (spawnPoint == null)
                spawnPoint = transform;

            currentEntity = 0;

            active = enableAtStart;

            if (active)
                animator.SetBool("Spawn", true);

        }

        public override void ActivateTrap() {

            if (active && currentSpawned == amount)
                Destroy(this);
            else
                active = true;

            if (active) {
                currentSpawned++;
                animator.SetBool("Spawn", true);
            }

        }

    }

}
