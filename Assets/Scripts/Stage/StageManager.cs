using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.UI;
using DEEP.Entities;
using DEEP.Entities.Player;
using DEEP.Collectibles;

namespace DEEP.Stage
{

    public class StageManager : MonoBehaviour
    {

        // Singleton for the StageManager =====================================================================================
        private static StageManager instance;
        public static StageManager Instance { get { return instance; } }

        // PlayerController reference =========================================================================================
        private PlayerController controller;

        // Obtains and stores a reference to the PlayerController instance.
        public PlayerController targetPlayer {
            get { 
                if(controller != null)
                    return controller; 
                else {
                    controller = FindObjectOfType<PlayerController>();
                    return controller;
                }
            } 
        }
        // ====================================================================================================================

        [Tooltip("ScriptableObject with information about the stage")]
        public StageInfo stageInfo;

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

        // Current duration of the level.
        private float duration;

        [Tooltip("Prefab to be used for the player.")]
        [SerializeField] private GameObject playerPrefab = null;

        [Tooltip("Where the player will be spawned.")]
        [SerializeField] private Transform playerSpawn = null;

        [Tooltip("If the player inventory should be reset on the start of the level.")]
        [SerializeField] private bool resetPlayerInventory = false;

        // Stores if the level has already started.
        private bool canStart;
        private bool started;

        [System.Serializable]
        private class Cutscene 
        {
            [Tooltip("Objects that should be enabled only during the cutscene.")]
            public GameObject[] cutsceneObjects = new GameObject[0];
            [Tooltip("Objects that should be enabled only after the cutscene.")]
            public GameObject[] inGameObjects = new GameObject[0];
        }
        [SerializeField] private Cutscene cutscene = new Cutscene();

        private void Awake()
        {
            // Ensures theres only one instance of this script.
            if (instance != null) {
                Debug.LogError("StageInfo: more than one instance of singleton found!");
                Destroy(Instance.gameObject);
            }
            instance = this;

            // Resets time to ensure the game is not paused.
            Time.timeScale = 1;

            // Initializes the speedrun counters;
            numStageEnemies = FindObjectsOfType<EnemyBase>().Length;
            numStageCollectibles = FindObjectsOfType<CollectibleBase>().Length;
            numStageSecrets = FindObjectsOfType<SecretTrigger>().Length;

            numEnemiesKilled = 0;
            numCollectiblesCollected = 0;
            numSecretsFound = 0;

            duration = 0.0f;

            // Waits for level start.
            canStart = false;
            started = false;

        }

        // Enables the player to spawn and the level to start.
        public void EnableLevelStart() { canStart = true; }

        // Spawns Player and starts level.
        private void StartLevel() {

            // Spawns the player.
            Debug.Log("Spawning player...");
            GameObject player = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation);

            // Finishes the cutscene.
            if(cutscene.cutsceneObjects != null)
                foreach(GameObject obj in cutscene.cutsceneObjects)
                    obj.SetActive(false);
            if(cutscene.inGameObjects != null)
                foreach(GameObject obj in cutscene.inGameObjects)
                    obj.SetActive(true);
            


            started = true;

        }

        private void FixedUpdate()
        {

            // Returns if the level has not started yet (is in the cutscene).
            if(!started) {

                // Checks for a Main Menu that might be running a cutscene
                MainMenu mainMenu = FindObjectOfType<MainMenu>();

                // If no Main Menu was found the game can start right away.
                if(mainMenu == null)
                    canStart = true;

                // Starts the level as soon as allowed to.
                if(canStart)
                    StartLevel();

            } else {

                // Counts the time spent on the stage after the level started.
                duration += Time.fixedDeltaTime;

            }

        }

        // Counts an enemy kill.
        public void CountKill() { 
            numEnemiesKilled++; 
            targetPlayer.HUD.Speedrun.SetKillCount(numEnemiesKilled, numStageEnemies);
        }

        // Counts a collected item.
        public void CountCollection() { 
            numCollectiblesCollected++; 
            targetPlayer.HUD.Speedrun.SetItemCount(numCollectiblesCollected, numStageCollectibles);
        }

        // Counts a secret found.
        public void CountSecretFound() { 
            numSecretsFound++; 
            targetPlayer.HUD.Speedrun.SetSecretCount(numSecretsFound, numStageSecrets);
        }

        // Gets the stage name.
        public string GetStageName() { return stageInfo.stageName; }

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
        public static string GetDurationString(float duration) {  

            if(duration < 0)
                return "-.--.-";

            float minutes = (int)Mathf.Floor(duration / 60);
            float seconds = (int)Mathf.Floor(duration - (minutes * 60));
            float secondFraction = (int)Mathf.Floor((duration - seconds - (minutes * 60)) * 10);

            return minutes + ":" + seconds.ToString().PadLeft(2, '0') + "." + secondFraction;

        }

        // Returns the name of the next stage to be loaded.
        public string GetNextStage() {

            if(stageInfo.isBetaEnd) { return "BetaEnd"; }

            if(stageInfo.isWebEnd) {
                #if UNITY_WEBGL
                    return "WebEnd";
                #endif
            }

            // Resets the game if there is no nextStage.
            if(stageInfo == null) {
                Debug.LogWarning("StageManager.GetNextStage: No next scene! Reseting game...");
                return SceneManager.GetSceneByBuildIndex(0).name;
            }

            return stageInfo.nextStage.stageScene;

        }

        // Returns if the player inventory should be reset at start.
        public bool GetResetInventory() { return resetPlayerInventory; }

    }

}
