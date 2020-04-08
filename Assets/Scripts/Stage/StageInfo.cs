using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DEEP.AI;
using DEEP.Collectibles;

namespace DEEP.Stage
{

    public class StageInfo : MonoBehaviour
    {

        public static StageInfo current;

        [SerializeField] private string stageName = "no name";

        public string nextStageSceneName = "no name";

        private int numStageEnemies;
        private int numStageCollectibles;
        private int numStageSecrets;

        private float duration;

        private void Awake()
        {
            current = this;

            numStageEnemies = FindObjectsOfType<EnemyAISystem>().Length;
            numStageCollectibles = FindObjectsOfType<CollectibleBase>().Length;
            numStageSecrets = FindObjectsOfType<SecretTrigger>().Length;

            duration = 0.0f;

        }

        private void FixedUpdate()
        {

            duration += Time.fixedDeltaTime;

        }

        // Gets the stage name.
        public string GetStageName() { return stageName; }

        // Initial number of enemies
        public int GetTotalEnemies() { return numStageEnemies; }

        // Initial number of collectibles.
        public int GetTotalCollectibles() { return numStageCollectibles;  }

        // Initial number of secrets.
        public int GetTotalSecrets() { return numStageSecrets;  }

        // Number of enemies killed.
        public int GetKillCount() { return numStageEnemies - FindObjectsOfType<EnemyAISystem>().Length; }

        // Number of collectibles collected.
        public int GetCollectibleCount() { return numStageCollectibles - FindObjectsOfType<CollectibleBase>().Length; }

        // Number of secrets found.  
        public int GetSecretCount() { return numStageSecrets - FindObjectsOfType<SecretTrigger>().Length; }

        // Percentage of enemies killed.
        public float GetKillPercentage() 
        { 
            
            if(numStageEnemies <= 0)
                return 1;

            return (float)GetKillCount() / (float)numStageEnemies; 
        }

        // Percentage of collectibles collected.
        public float GetCollectiblePercentage() 
        { 
            if(numStageCollectibles <= 0)
                return 1;

            return (float)GetCollectibleCount() / (float)numStageCollectibles; 
        }

        // Percentage of secrets collected.  
        public float GetSecretPercentage() 
        { 
            if(numStageSecrets <= 0)
                return 1;
                
            return (float)GetSecretCount() / (float)numStageSecrets; 
        }

        // Get current time elapsed from the start of the level, without pauses.
        public float GetDuration() { return duration; }

    }

}
