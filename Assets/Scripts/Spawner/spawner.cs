using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DEEP.AI;
using DEEP.Stage;


namespace DEEP.Spawn
{
    public class Spawner : MonoBehaviour, ITrappable
    {

        protected enum SpawnMode { Single, Loop };

        [SerializeField] protected SpawnMode spawnMode = SpawnMode.Single;

        [SerializeField] protected List<GameObject> entityList = new List<GameObject>();

        [SerializeField] protected Transform spawnPoint = null;

        [SerializeField] protected GameObject patrolStartingPoint = null;

        [SerializeField] protected float spawnDelay = 2.0f;

        [SerializeField] protected bool enableAtStart = false;

        protected bool active = false;

        protected int currentEntity = 0;

        // Object used to wait in coroutines.
        protected WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        protected virtual void Start()
        {

            if (spawnPoint == null)
                spawnPoint = transform;

            currentEntity = 0;

            active = enableAtStart;

            if (active)
                StartCoroutine(SpawnRoutine());

        }

        public virtual void ActivateTrap() {

            active = !active;

            if(active)
                StartCoroutine(SpawnRoutine());

        }

        protected IEnumerator SpawnRoutine() {
            
            while(active) {

                Spawn();

                if (spawnMode == SpawnMode.Loop) {

                    float time = 0.0f;
                    while (time < spawnDelay) {

                        time += Time.fixedDeltaTime;
                        yield return waitForFixed;

                    }

                } else {
                    active = false;
                    break;
                }

                yield return waitForFixed;

            }
        }

        public void Spawn() {

            GameObject instance = Instantiate(entityList[currentEntity], spawnPoint.position, spawnPoint.rotation);

            currentEntity = (currentEntity + 1) % entityList.Count;

            EnemyAISystem enemyAI = instance.GetComponent<EnemyAISystem>();
            enemyAI.spawned = true;

            if (patrolStartingPoint != null) {

                enemyAI.addPatrolPoint(patrolStartingPoint);
                enemyAI.ResetPatrol();

            }

        }

        public void SpawnMultiple(int index, int amount) {

            if (index < 0 || index > entityList.Count) {
                Debug.LogError("Invalid index!");
                return;
            }

            for (int i = 0; i < amount; i++) {

                GameObject instance = Instantiate(entityList[index], spawnPoint.position, spawnPoint.rotation);

                currentEntity = (currentEntity + 1) % entityList.Count;

                EnemyAISystem enemyAI = instance.GetComponent<EnemyAISystem>();
                enemyAI.spawned = true;

                if (patrolStartingPoint != null) {

                    enemyAI.addPatrolPoint(patrolStartingPoint);
                    enemyAI.ResetPatrol();

                }

            }
        }
    }
}
    
