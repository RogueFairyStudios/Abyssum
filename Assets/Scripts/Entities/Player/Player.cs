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

        protected override void Start()
        {

            // Ensures theres only one instance of this script.
            if (Instance != null) {
                Debug.LogError("Player: more than one instance of singleton found!");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            base.Start();

            // Resets the time.
            Time.timeScale = 1;

            // Initializes the player state.
            currentState = PlayerState.Play;

            // Get components ==============================================================================

            // Gets the Player's Movementation script.
            movementation = GetComponent<PlayerMovementation>();

            // Gets the Player's HUD Controller and initializes the UI.
            HUD = GetComponentInChildren<HUDController>();
            HUD.SetHealthHUD(health, maxHealth);
            HUD.SetArmorHUD(armor, maxArmor);

            // Gets the weapons controller.
            weaponController = GetComponent<PlayerWeaponController>();

            // Gets the key inventory.
            keyInventory = GetComponent<InventoryKey>();

        }

        private void Update()
        {

            // Pause ============================================================================================

            // Pauses and un-pauses the game.
            if (currentState == PlayerState.Play || currentState == PlayerState.Paused) {
                if (Input.GetButtonDown("Cancel")) 
                    TogglePause(); 
            }

            // Returns execution if the game is paused, the player has died or the level has ended. ==============
            if (currentState != PlayerState.Play)
                return;

            //Equiping weapons ===================================================================================

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
            // Player movementation is handled by the PlayerMovimentation script referenced by movementation.
            // ===================================================================================================

        }

        public override void Damage(int amount, DamageType type) {

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
            
            Debug.Log("You died!");

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

            Debug.Log("Adding the " + color.ToString() + " key to the inventory");
			keyInventory.AddKey(color);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Updates the collected keys on the HUD.
            HUD.SetKeyHUD();

        }

        // Signalizes the player has found a secret;
        public void FoundSecret(AudioClip feedbackAudio) {
            
            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

        }

        private void OnChangeArmor() { HUD.SetArmorHUD(armor, maxArmor); }

        protected override void OnChangeHealth() {

            HUD.SetHealthHUD(health, maxHealth);

            base.OnChangeHealth();

        }

        // Pauses and un-pauses the game.
        public void TogglePause() {

            // Pauses or un-pauses the game.
            if (currentState == PlayerState.Play)
                currentState = PlayerState.Paused;
            else
                currentState = PlayerState.Play;

            // Stops player movimentation.
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
        public void UpdateMouseSensitivity(float sensitivity) { movementation.sensitivity = sensitivity; }

        public void EndLevel() {
            
            Debug.Log("Level completed!");

            // Marks the level as finished.
            currentState = PlayerState.EndLevel;

            // Disables player control.
            movementation.enabled = false;

            // Displays the ebnd level screen
            endLevelScreen.ShowScreen();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;
            
        }

        public void ContinueLevel() {  SceneManager.LoadSceneAsync(StageInfo.Instance.nextStageSceneName); }

        public void RestartGame() { SceneManager.LoadSceneAsync(0); }

        public void RestartLevel() { SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name); }

        public override void SetSlow() { movementation.SetSlow(); }
        public override void SetBaseSpeed() { movementation.SetBaseSpeed(); }

    }

}