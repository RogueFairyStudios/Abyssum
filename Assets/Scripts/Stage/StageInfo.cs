using UnityEngine;

using DEEP.AI;
using DEEP.Collectibles;
using System.Collections.Generic;
using DEEP.Entities;
using System.Linq;

namespace DEEP.Stage
{

    public class StageInfo : MonoBehaviour
    {

        public static StageInfo Instance;

        [SerializeField] private string stageName = "no name";

        public string nextStageSceneName = "no name";

        // Initial number of enemies in the stage.
        private int numStageEnemies;

        // Initial number of collectibles in the stage.
        private int numStageCollectibles;

        // Number of secrets in the stage.
        private int numStageSecrets;

        // Current number of enemies killed.
        private int numEnemiesKilled;

        // Initial number of collectibles already collected.
        private int numCollectiblesCollected;

        // Number of secrets found.
        private int numSecretsFound;

        private float duration;

        private void Start()
        {
            // Ensures theres only one instance of this script.
            if (Instance != null) {
                Debug.LogError("StageInfo: more than one instance of singleton found!");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            numStageEnemies = FindObjectsOfType<EnemyBase>().Length;
            numStageCollectibles = FindObjectsOfType<CollectibleBase>().Length;
            numStageSecrets = FindObjectsOfType<SecretTrigger>().Length;

            numEnemiesKilled = 0;
            numCollectiblesCollected = 0;
            numSecretsFound = 0;

            Player.Instance.HUD.SetKillCount(numEnemiesKilled, numStageEnemies);
            Player.Instance.HUD.SetItemCount(numCollectiblesCollected, numStageCollectibles);
            Player.Instance.HUD.SetSecretCount(numSecretsFound, numStageSecrets);

            duration = 0.0f;

        }

        private void FixedUpdate()
        {

            // Counts the time spent on the stage.
            duration += Time.fixedDeltaTime;

        }

        // Counts an enemy kill.
        public void CountKill() { 
            numEnemiesKilled++; 
            Player.Instance.HUD.SetKillCount(numEnemiesKilled, numStageEnemies);
        }

        // Counts a collected item.
        public void CountCollection() { 
            numCollectiblesCollected++; 
            Player.Instance.HUD.SetItemCount(numCollectiblesCollected, numStageCollectibles);
        }

        // Counts a secret found.
        public void CountSecretFound() { 
            numSecretsFound++; 
            Player.Instance.HUD.SetSecretCount(numSecretsFound, numStageSecrets);
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
        public int GetKillCount() { return numEnemiesKilled; }

        // Number of collectibles collected.
        public int GetCollectibleCount() { return numCollectiblesCollected; }

        // Number of secrets found.  
        public int GetSecretCount() { return numSecretsFound; }

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

        // Get current time elapsed from the start of the level, without pauses and returns as a float.
        public float GetDuration() { return duration; }

        // Get current time elapsed from the start of the level, without pauses and returns as a formated string.
        public string GetDurationString() {  

            float minutes = (int)Mathf.Floor(duration / 60);
            float seconds = (int)Mathf.Floor(duration - (minutes * 60));
            float secondFraction = (int)Mathf.Floor((duration - seconds - (minutes * 60)) * 10);

            return minutes + ":" + seconds.ToString().PadLeft(2, '0') + "." + secondFraction;

        }

    }

}
