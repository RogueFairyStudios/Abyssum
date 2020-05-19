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
    [RequireComponent(typeof(PlayerMovimentation))]
    public class Player : EntityBase
    {

        // Singleton for the player.
        public static Player Instance;

        // Stores if the player is alive.
        private bool isAlive = true;

        
        [Header("Armor")] // ==============================================================================

        [Tooltip("Players's current armor.")]
        [SerializeField] protected int armor = 0;

        [Tooltip("Players's max armor.")]
        [SerializeField] protected int maxArmor = 100;

        [Tooltip("What's the minimum percentage of damage that armor will absorb.")]
        [Range(0.0f, 1.0f)]
        [SerializeField] protected float minArmorAbsorption = 0.3f;

        [Header("Pause")] // ==============================================================================

        [Tooltip("If the game is paused.")]
        public bool isPaused = true;
        [Tooltip("GameObject that contains the pause menu.")]
        [SerializeField] private GameObject pauseMenu = null;

        [Header("Death")] // ==============================================================================
        [Tooltip("The screen overlay for when the player dies.")]
        [SerializeField] private GameObject deathScreen = null;
        [Tooltip("The menu items for when the player dies.")]
        [SerializeField] private GameObject deathMenu = null;

        [Header("Level-End")] // ==============================================================================
        [Tooltip("The screen overlay for when the level ends.")]
        [SerializeField] private EndScreen endLevelScreen = null;

        [Header("Feedback")] // ===========================================================================
        [Tooltip("Audio source used to play clips related to feedback to the player.")]
        public AudioSource feedbackAudioSource = null;

        [Header("Components")] // =========================================================================
        public PlayerMovimentation movimentation = null;
        public PlayerWeaponController waponController = null;
        public HUDController HUD = null;
        public InventoryKey keyInventory = null;

        protected override void Start()
        {

            // Ensures theres only one instance of this script.
            if (Instance != null)
                Destroy(gameObject);
            Instance = this;

            base.Start();

            // Resets the time.
            Time.timeScale = 1;

            // Initializes with the player alive and the game unpaused.
            isAlive = true;
            isPaused = false;

            // Get components ==============================================================================

            // Gets the Player's Movimentation script.
            movimentation = GetComponentInChildren<PlayerMovimentation>();

            // Gets the Player's HUD Controller and initializes the UI.
            HUD = GetComponentInChildren<HUDController>();
            HUD.SetHealthHUD(health, maxHealth);
            HUD.SetArmorHUD(armor, maxArmor);

            // Gets the weapons controller.
            waponController = GetComponentInChildren<PlayerWeaponController>();

            // Gets the key inventory.
            keyInventory = GetComponentInChildren<InventoryKey>();

        }

        private void Update()
        {

            // Pause =========================================================================================

            // Pauses and unpauses the game.
            if (Input.GetButtonDown("Cancel") && isAlive) {
               TogglePause();
            }

            // Returns execution if the game is paused or the player has died. ===============================
            if (!isAlive || isPaused)
                return;

            //Equiping weapons ===============================================================================

            // Verifies the number keys.
            if(Input.GetKeyDown("0")) // Checks for 0.
                waponController.SwitchWeapons(9); // 0 is the rightmost key so it's actually the last weapon at index 9.
            else {
                for(int i = 1; i <= 9; i++) // Checks for the other keys.
                    if(Input.GetKeyDown(i.ToString()))
                        waponController.SwitchWeapons(i - 1);  // Converts the key into the weapon index of the list.
            }

            // Change weapon using mouse scrollwheel =========================================================
            if (waponController.currentWeapon != null) // Check if player has a weapon
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) // Scroll Up
                {
                    waponController.SwitchWeapons(waponController.GetNextEnabledWeaponIndex());
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // Scroll Down
                {
                    waponController.SwitchWeapons(waponController.GetPreviousEnabledWeaponIndex());
                }

                // Firing weapons ============================================================================
                if (Input.GetButton("Fire1")) {

                    // Tries firing the weapon.
                    waponController.FireCurrentWeapon();

                }
            }


            // Player movimentation is handled by the PlayerMovimentation script referenced by movimentation.

        }

        public override void Damage(int amount, DamageType type) {

            // Calculates the percent of damage that should be absorbed by armor.
            float armorAbsorption = Mathf.Clamp(armor / maxArmor, minArmorAbsorption, 1f);

            // Calculates the amount of damage to armor and health.
            int armorDamage = Mathf.Clamp((int)Math.Round(armorAbsorption * amount), 0, armor); // Clamps to ensure if armor breaks the remaining damage will go to health.
            int healthDamage = amount - armorDamage;

            // Decreases armor.
            armor -= armorDamage;

            // Decreases health.
            health -= healthDamage;

            if(health > 0) {
                // Flicks the screen to give feedback.
                HUD.StartScreenFeedback(HUDController.FeedbackType.Damage);
            }

            // Handles any changes that have to be made when modifying armor or health.
            OnChangeArmor();
            OnChangeHealth();

        }

        protected override void Die() {
            
            Debug.Log("You died!");

            // Marks the player as dead.
            isAlive = false;

            // Disables player control.
            movimentation.enabled = false;

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
            if(healed && feedbackAudio != null) {

                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

                // Flicks the screen to give feedback.
                 HUD.StartScreenFeedback(HUDController.FeedbackType.Healing);

            }

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

            // Flicks the screen to give feedback.
             HUD.StartScreenFeedback(HUDController.FeedbackType.Armor);

            // Updates the armor counter on the HUD.
            OnChangeArmor();

            return true;

        }

        // Gives a keycard to the player.
        public void GiveKeyCard(KeysColors color, AudioClip feedbackAudio) {

            print("Adding the " + color.ToString() + " key to the inventory");
			keyInventory.AddKey(color);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Flicks the screen to give feedback.
             HUD.StartScreenFeedback(HUDController.FeedbackType.Keycard);

            // Updates the collected keys on the HUD.
            HUD.SetKeyHUD();

        }

        // Sinalizes the player has found a secret;
        public void FoundSecret() {
            
            // Flicks the screen to give feedback.
             HUD.StartScreenFeedback(HUDController.FeedbackType.Secret);

        }

        private void OnChangeArmor() { HUD.SetArmorHUD(armor, maxArmor); }

        protected override void OnChangeHealth() {

            HUD.SetHealthHUD(health, maxHealth);

            base.OnChangeHealth();

        }

        // Pauses and unpauses the game.
        public void TogglePause() {

            // Inverts the value.
            isPaused = !isPaused;

            // Stops player movimentation.
            movimentation.enabled = !isPaused;

            // Shows or hides the pause menu and the cursor
            pauseMenu.SetActive(isPaused);
            Cursor.visible = isPaused;

            // Set the cursor LockMode and time to the correct value.
            if (isPaused) {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }

        }

        // Updates mouse sensitivity from an outside script.
        public void UpdateMouseSensitivity(float sensitivity) { /*this.sensitivity = sensitivity;*/ }

        public void EndLevel() {
            
            Debug.Log("Level completed!");

            // Disables player control.
            movimentation.enabled = false;

            // Displays the ebnd level screen
            endLevelScreen.ShowScreen();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;
            
        }

        public void ContinueLevel() {  SceneManager.LoadSceneAsync(StageInfo.Instance.nextStageSceneName); }

        public void RestartGame() { SceneManager.LoadSceneAsync(0); }

        public void RestartLevel() { SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name); }

    }

}