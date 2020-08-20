using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.UI;
using DEEP.Weapons;
using DEEP.Stage;
using DEEP.DoorsAndKeycards;

namespace DEEP.Entities
{

    // Class that controls the Player.
    [RequireComponent(typeof(PlayerMovementation))]
    [RequireComponent(typeof(PlayerWeaponController))]
    [RequireComponent(typeof(InventoryKey))]
    public class Player : EntityBase
    {

        // Singleton for the player.
        public static Player Instance;

        // Possible states for the player.
        private enum PlayerState { Play, Paused, Dead, EndLevel }

        // Current player state.
        private PlayerState currentState;

        
        [Header("Armor")] // ==============================================================================

        [Tooltip("Players's current armor.")]
        [SerializeField] protected int armor = 0;

        [Tooltip("Players's max armor.")]
        [SerializeField] protected int maxArmor = 100;

        [Tooltip("What's the minimum percentage of damage that armor will absorb.")]
        [Range(0.0f, 1.0f)]
        [SerializeField] protected float minArmorAbsorption = 0.3f;

        [Header("Pause")] // ==============================================================================

        [Tooltip("GameObject that contains the pause menu.")]
        [SerializeField] protected GameObject pauseMenu = null;

        [Tooltip("Reference to the settings screen on the pause menu.")]
        [SerializeField] protected OptionsButtons optionsMenu = null;

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
        public PlayerMovementation movementation = null;
        public PlayerWeaponController weaponController = null;
        public HUDController HUD = null;
        public InventoryKey keyInventory = null;

        // Object used to wait in coroutines.
        protected WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        // If this script has been initialzed by the main Player script.
        private bool initialized = false;

        public void Initialize()
        {

            Debug.Log("Initializing Player...");

            // Ensures there's only one instance of this script.
            if (Instance != null) {
                Debug.LogError("Player: more than one instance of singleton found!");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initializes the player state.
            currentState = PlayerState.Play;

            // Get components ==============================================================================

            // Gets the HUDController and initializes it.
            HUD = GetComponentInChildren<HUDController>();
            HUD.Initialize();
            HUD.health.SetValue(health, maxHealth);
            HUD.armor.SetValue(armor, maxArmor);

            // Gets the PlayerMovementation script and initializes it.
            movementation = GetComponent<PlayerMovementation>();
            movementation.Initialize();

            // Gets thePlayerWeaponController script and initializes it.
            weaponController = GetComponent<PlayerWeaponController>();
            weaponController.Initialize();

            // Gets the InventoryKey script and initializes it.
            keyInventory = GetComponent<InventoryKey>();
            keyInventory.Initialize();


            // Loads the player inventory if it wasn't reset.
            if(!StageManager.Instance.GetResetInventory()) {

                // Loads the inventory for a save if there is one.
                // TODO: Load

                // If no save is avaliable gives the Player the first weapon.
                weaponController.GiveWeapon(0, 0, null);

            }

            initialized = true;

            // Initializes the options menu.
            optionsMenu.Initialize();
            
        }

        private void Update()
        {

            // Returns if not initialized.
            if(!initialized) return;

            // Pause ============================================================================================

            // Pauses and un-pauses the game.
            if (currentState == PlayerState.Play || currentState == PlayerState.Paused) {
                if (Input.GetButtonDown("Cancel")) 
                    TogglePause(); 
            }

            // Returns execution if the game is paused, the player has died or the level has ended. ==============
            if (currentState != PlayerState.Play)
                return;

            // Equiping weapons ===================================================================================

            // Verifies the number keys.
            if(Input.GetKeyDown("0")) // Checks for 0.
                weaponController.SwitchWeapons(9); // 0 is the rightmost key so it's actually the last weapon at index 9.
            else {
                for(int i = 1; i <= 9; i++) // Checks for the other keys.
                    if(Input.GetKeyDown(i.ToString()))
                        weaponController.SwitchWeapons(i - 1);  // Converts the key into the weapon index of the list.
            }

            // Check if player has a weapon
            if (weaponController.currentWeapon != null)
            {

                // Change weapon using mouse scrollwheel =========================================================
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) // Scroll Up
                    weaponController.SwitchWeapons(weaponController.GetNextEnabledWeaponIndex());
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // Scroll Down
                    weaponController.SwitchWeapons(weaponController.GetPreviousEnabledWeaponIndex());

                // Try firing weapons ============================================================================
                if (Input.GetButton("Fire1"))
                    weaponController.FireCurrentWeapon();

            }

            // ===================================================================================================
            // Player movementation is handled by the PlayerMovementation script referenced by movementation.
            // ===================================================================================================

        }

        public override void Damage(int amount, DamageType type) {

            if(!initialized) { 
				Debug.LogError("Damage: You need to initialize the script first!"); 
				return;
			}

            // Initially all damage is to the health.
            int healthDamage = amount;

            // Calculates armor damage absorption.
            if (type != DamageType.IgnoreArmor) {

                // Calculates the percent of damage that should be absorbed by armor.
                float armorAbsorption = Mathf.Clamp(armor / maxArmor, minArmorAbsorption, 1f);

                // Calculates the amount of damage to armor.
                int armorDamage = Mathf.Clamp((int)Math.Round(armorAbsorption * amount), 0, armor); // Clamps to ensure if armor breaks the remaining damage will go to health.

                // Damages the armor.
                armor -= armorDamage;

                // Handles any changes that have to be made when modifying armor.
                OnChangeArmor();

                // Decreases the damage to health by the amount of damage that went to the armor instead.
                healthDamage -= armorDamage;

            }

            // Decreases the health.
            health -= healthDamage;

            // Handles any changes that have to be made when modifying health.
            OnChangeHealth();

            // Flicks the screen to give feedback if player is not dead.
            if (health > 0)
                HUD.StartScreenFeedback(HUDController.FeedbackType.Damage);

        }

        protected override void Die() {
            
            if(!initialized) { 
				Debug.LogError("Die: You need to initialize the script first!"); 
				return;
			}

            Debug.Log("Player died!");

            // Marks the player as dead.
            currentState = PlayerState.Dead;

            // Disables player control.
            movementation.enabled = false;

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
                yield return waitForFixed;
            }

            // Enables menu.
            deathMenu.SetActive(true);

            // Unlocks the mouse.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Pauses the game time.
            Time.timeScale = 0;

        }

        public virtual bool Heal(int amount, HealType type, AudioClip feedbackAudio) 
        {

            if(!initialized) { 
				Debug.LogError("Heal: You need to initialize the script first!"); 
				return false;
			}

            // Tries to heal the entity.
            bool healed = base.Heal(amount, type);

            // If the entity was healed plays the player feedback sound.
            if(healed && feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            return healed;

        }

        // Give armor to the player.
        public virtual bool GiveArmor(int amount, AudioClip feedbackAudio) 
        {

            if(!initialized) { 
				Debug.LogError("GiveArmor: You need to initialize the script first!"); 
				return false;
			}

            // Checks if armor is not maxed out.
            if(armor >= maxArmor) return false;

            armor += amount; // Adds the armor.

            // Ensures not going above max armor.
            if(armor > maxArmor) armor = maxArmor;

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Updates the armor counter on the HUD.
            OnChangeArmor();

            return true;

        }

        // Gives a keycard to the player.
        public void GiveKeyCard(KeysColors color, AudioClip feedbackAudio) {

            if(!initialized) { 
				Debug.LogError("GiveKeyCard: You need to initialize the script first!"); 
				return;
			}

            Debug.Log("Adding the " + color.ToString() + " key to the inventory");
			keyInventory.AddKey(color);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Updates the collected keys on the HUD.
            HUD.keycards.UpdateValues();

        }

        // Signalizes the player has found a secret;
        public void FoundSecret(AudioClip feedbackAudio) {

            if(!initialized) { 
				Debug.LogError("FoundSecret: You need to initialize the script first!"); 
				return;
			}
            
            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

        }

        private void OnChangeArmor() { HUD.armor.SetValue(armor, maxArmor); }

        protected override void OnChangeHealth() {

            HUD.health.SetValue(health, maxHealth);

            base.OnChangeHealth();

        }

        // Pauses and un-pauses the game.
        public void TogglePause() {

            if(!initialized) { 
				Debug.LogError("TogglePause: You need to initialize the script first!"); 
				return;
			}

            // Pauses or un-pauses the game.
            if (currentState == PlayerState.Play)
                currentState = PlayerState.Paused;
            else
                currentState = PlayerState.Play;

            // Stops player movementation.
            movementation.enabled = (currentState == PlayerState.Play);

            // Shows or hides the pause menu and the cursor
            pauseMenu.SetActive(currentState == PlayerState.Paused);
            Cursor.visible = (currentState == PlayerState.Paused);

            // Set the cursor LockMode and time to the correct value.
            if (currentState == PlayerState.Play) {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            } else {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }

        }

        // Updates mouse sensitivity from an outside script.
        public void UpdateMouseSensitivity(float sensitivity) { 

            if(!initialized) { 
				Debug.LogError("UpdateMouseSensitivity: You need to initialize the script first!"); 
				return;
			}

            movementation.sensitivity = sensitivity; 

        }

        public void EndLevel() {

            if(!initialized) { 
				Debug.LogError("EndLevel: You need to initialize the script first!"); 
				return;
			}
            
            Debug.Log("Level completed!");

            // Marks the level as finished.
            currentState = PlayerState.EndLevel;

            // Disables player control.
            movementation.enabled = false;

            // Displays the end level screen
            endLevelScreen.ShowScreen();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;
            
        }

        public void ContinueLevel() {  SceneManager.LoadSceneAsync(StageManager.Instance.GetNextStage()); }

        public void RestartGame() { SceneManager.LoadSceneAsync(0); }

        public void RestartLevel() { SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name); }

        public override void SetSlow() { 
            
            if(!initialized) { 
				Debug.LogError("SetSlow: You need to initialize the script first!"); 
				return;
			}

            movementation.SetSlow(); 
            
        }

        public override void SetBaseSpeed() { 

            if(!initialized) { 
				Debug.LogError("SetBaseSpeed: You need to initialize the script first!"); 
				return;
			}
            
            movementation.SetBaseSpeed(); 
            
        }

    }

}