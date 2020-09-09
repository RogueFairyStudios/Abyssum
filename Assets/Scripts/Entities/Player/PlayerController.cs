using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.UI;
using DEEP.Stage;
using DEEP.Weapons;
using DEEP.DoorsAndKeycards;

namespace DEEP.Entities.Player
{

    // Class that controls the Player.
    [RequireComponent(typeof(PlayerMovementation))]
    [RequireComponent(typeof(PlayerWeaponController))]
    [RequireComponent(typeof(KeyInventory))]
    [RequireComponent(typeof(PlayerEntity))]
    public class PlayerController : MonoBehaviour
    {

        // Singleton for the player controller.
        public static PlayerController Instance;

        // If the player is not dead or the level has not ended yet, them the game is playing.
        protected bool isPlaying;

        [Header("Pause")] // ==============================================================================

        protected bool isPaused;

        [Tooltip("GameObject that contains the pause menu.")]
        [SerializeField] protected GameObject pauseMenu = null;

        [Header("Death")] // ==============================================================================
        [Tooltip("The screen overlay for when the player dies.")]
        [SerializeField] protected GameObject deathScreen = null;
        [Tooltip("The menu items for when the player dies.")]
        [SerializeField] protected GameObject deathMenu = null;

        [Header("Level-End")] // ==============================================================================
        [Tooltip("The screen overlay for when the level ends.")]
        [SerializeField] protected EndScreen endLevelScreen = null;

        [Header("Feedback")] // ===========================================================================
        [Tooltip("Audio source used to play clips related to feedback to the player.")]
        public AudioSource feedbackAudioSource = null;

        [Header("Components")] // =========================================================================
        public KeyInventory keyInventory;
        public PlayerMovementation movementation;
        public PlayerWeaponController weaponController;
        public HUDController HUD;
        public PlayerEntity entity;

        public void Awake()
        {
            
            Debug.Log("Initializing Player...");

            // Ensures there's only one instance of this script.
            if (Instance != null) {
                Debug.LogError("Player: more than one instance of singleton found!");
                Destroy(Instance.gameObject);
                return;
            }
            Instance = this;
            
            // The Player always starts the game in regular play mode.
            isPlaying = true;
            isPaused = false;

            // Creates the KeyInventory instance.
            keyInventory = new KeyInventory();
            
            // Get components =================================================================================================

            // Tries getting the necessary components if they are not avaliable.
            if(movementation == null) movementation = GetComponent<PlayerMovementation>();
            if(weaponController == null) weaponController = GetComponent<PlayerWeaponController>();
            if(HUD == null) HUD = GetComponentInChildren<HUDController>();
            if(entity == null) entity = GetComponent<PlayerEntity>();

            // ================================================================================================================
            
            // Loads the player inventory if it wasn't reset.
            if(!StageManager.Instance.GetResetInventory()) {

                // Loads the inventory for a save if there is one.
                // TODO: Load

                // If no save is avaliable gives the Player the first weapon.
                weaponController.GiveWeapon(0, 0, null);

            }

            // Ensures timeScale is correct.
            Time.timeScale = 1;

            // Locks and hides the cursor.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Collects garbage at start to avoid potential lag.
            System.GC.Collect();
            
        }

        void Update() {

            // Pauses and un-pauses the game.
            if (isPlaying) {
                if (Input.GetButtonDown("Cancel")) 
                    TogglePause(); 
            }

        }


        // Pauses and un-pauses the game.
        public void TogglePause() {

            // Checks if the player can toggle pause.
            if(!isPlaying)
                return;

            // Pauses or un-pauses the game.
            isPaused = (!isPaused);

            // Shows or hides the pause menu and the cursor
            pauseMenu.SetActive(isPaused);
            Cursor.visible = isPaused;

            // Enables or disables player components.
            movementation.enabled = (!isPaused);
            weaponController.enabled = (!isPaused);

            // Set the cursor LockMode and time to the correct value.
            if (!isPaused) {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            } else {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                System.GC.Collect(); // Uses the pausing of the game to collect garbage as potential lag will be less noticeable.
            }


        }

        // Handles the Player entity's death.
        public void Die() {
            
            // The game has ended.
            isPlaying = false;

            // Disables player control.
            movementation.enabled = false;
            weaponController.enabled = false;

            // Enables the death menu overlay.
            deathScreen.SetActive(true);

            // Plays death animation.
            Camera.main.GetComponent<Animator>().SetBool("Death", true);

            // Shows the menu after some time.
            StartCoroutine(ShowDeathMenu());

        }

        // Shows death menu after a certain amount of time.
        protected IEnumerator ShowDeathMenu()
        {

            float time = 0;
            while(time < 2.0f) // Waits for the delay.
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Displays the death menu.
            deathMenu.SetActive(true);

            // Unlocks the mouse.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Pauses the game time.
            Time.timeScale = 0;

        }

        public void EndLevel() {
            
            Debug.Log("Level completed!");

            // The game has ended.
            isPlaying = false;

            // Disables player control.
            movementation.enabled = false;
            weaponController.enabled = false;

            // Displays the end level screen
            endLevelScreen.ShowScreen();

            // Unlocks the mouse.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Pauses the game time.
            Time.timeScale = 0;
            
        }

        public void ContinueLevel() {  SceneManager.LoadSceneAsync(StageManager.Instance.GetNextStage()); }

        public void RestartGame() { SceneManager.LoadSceneAsync(0); }

        public void RestartLevel() { SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name); }

        // Signalizes the player has found a secret;
        public void FoundSecret(AudioClip feedbackAudio) {
            
            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

        }

    }

}