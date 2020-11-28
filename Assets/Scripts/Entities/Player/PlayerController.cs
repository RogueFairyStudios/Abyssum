using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.UI;
using DEEP.HUD;
using DEEP.Stage;
using DEEP.Weapons;
using DEEP.DoorsAndKeycards;

namespace DEEP.Entities.Player
{

    // Class that controls the Player.
    [RequireComponent(typeof(PlayerEntity))]
    [RequireComponent(typeof(PlayerMovementation))]
    [RequireComponent(typeof(PlayerWeaponController))]
    public class PlayerController : MonoBehaviour
    {

        // Control variables ==================================================================================================

        // If the player is not dead or the level has not ended yet, them the game is playing.
        protected bool isPlaying;

        [Header("Pause")] // ==================================================================================================

        protected bool isPaused;

        [Tooltip("GameObject that contains the pause menu.")]
        [SerializeField] protected GameObject pauseMenu = null;

        [Header("Death")] // ==================================================================================================
        [Tooltip("The screen overlay for when the player dies.")]
        [SerializeField] protected GameObject deathScreen = null;
        [Tooltip("The menu items for when the player dies.")]
        [SerializeField] protected GameObject deathMenu = null;
        [Tooltip("Possible audio clips for player death.")]
        [SerializeField] protected AudioClip[] playerDeath;

        [Header("Level-End")] // ==============================================================================================
        [Tooltip("The screen overlay for when the level ends.")]
        [SerializeField] protected EndScreen endLevelScreen = null;

        [Header("Feedback")] // ===============================================================================================
        [Tooltip("Audio source used to play clips related to feedback to the player.")]
        public AudioSource feedbackAudioSource = null;

        // PlayerEntity reference =============================================================================================
        private PlayerEntity entity;
        // Returns the reference for the PlayerEntity from this PlayerController, 
        // tries getting it if it's not yet available.
        public PlayerEntity Entity {
            get { return entity; } 
        }

        // PlayerMovementation reference ======================================================================================
        private PlayerMovementation movementation;
        // Returns the reference for the PlayerMovementation from this PlayerController, 
        // tries getting it if it's not yet available.
        public PlayerMovementation Movementation {
            get { return movementation; } 
        }

        // PlayerWeaponsController reference ==================================================================================
        private PlayerWeaponController weapons;
        // Returns the reference for the PlayerWeaponController from this PlayerController, 
        // tries getting it if it's not yet available.
        public PlayerWeaponController Weapons {
            get { return weapons; } 
        }

        // HUDController reference ============================================================================================
        private HUDController hud;
        // Returns the reference for the PlayerWeaponController from this PlayerController, 
        // tries getting it if it's not yet available.
        public HUDController HUD {
            get { return hud; } 
        }

        // KeyInventory instance ==============================================================================================
        private KeyInventory keyInventory;
        // Returns the KeyInventory from this PlayerController, 
        // creates a new one if it doesn't exists yet.
        public KeyInventory Keys {
            get { return keyInventory; } 
        }

        // ====================================================================================================================

        protected virtual void Awake()
        {
            
            Debug.Log("Initializing Player...");
            
            // Gets/creates the necessary components and initializes them when needed.
            entity = GetComponent<PlayerEntity>();
            entity.Owner = this;

            movementation = GetComponent<PlayerMovementation>();
            movementation.Owner = this;

            weapons = GetComponent<PlayerWeaponController>();
            weapons.Owner = this;

            hud = GetComponentInChildren<HUDController>();

            keyInventory = new KeyInventory(this);


            // The Player always starts the game in regular play mode.
            isPlaying = true;
            isPaused = false;
            
            // Loads the player inventory if it wasn't reset.
            if(StageManager.Instance != null && !StageManager.Instance.GetResetInventory()) {

                // Loads the inventory for a save if there is one.
                // TODO: Load

                // If no save is avaliable gives the Player the first weapon.
                //weaponController.GiveWeapon(0, 0, null);

            }

            // Ensures timeScale is correct.
            Time.timeScale = 1;

            // Locks and hides the cursor.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Collects garbage at start to avoid potential lag.
            System.GC.Collect();
            
        }

        protected virtual void Update() {

            // Pauses and un-pauses the game.
            if (isPlaying) {
                if (Input.GetButtonDown("Cancel")) 
                    TogglePause(); 
            }

        }


        // Pauses and un-pauses the game.
        public virtual void TogglePause() {

            // Checks if the player can toggle pause.
            if(!isPlaying)
                return;

            // Pauses or un-pauses the game.
            isPaused = (!isPaused);

            // Shows or hides the pause menu and the cursor
            pauseMenu.SetActive(isPaused);
            Cursor.visible = isPaused;

            // Enables or disables player components.
            Movementation.enabled = (!isPaused);
            Weapons.enabled = (!isPaused);

            // Set the cursor LockMode and timeScale.
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            SetTimeScale(isPaused ? 0 : 1);

            // Uses the pausing of the game to collect garbage as potential lag will be less noticeable.
            if(isPaused)
                System.GC.Collect();

        }

        // Sets the timeScale, used by the TogglePause function.
        protected virtual void SetTimeScale(float scale) {  Time.timeScale = scale; }

        // Handles the Player entity's death.
        public void Die() {
            
            // Avoids zombie deaths
            if(!isPlaying)
                return;

            // The game has ended.
            isPlaying = false;

            // Disables player components.
            Movementation.enabled = false;
            Weapons.enabled = false;

            // Enables the death menu overlay.
            deathScreen.SetActive(true);

            // Plays death animation.
            Camera.main.GetComponent<Animator>().SetBool("Death", true);

            // Plays death sound effect
            feedbackAudioSource.PlayOneShot(playerDeath[Random.Range(0, playerDeath.Length)], 1.0f);

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