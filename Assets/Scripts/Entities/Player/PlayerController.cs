using System.Collections;

using UnityEngine;

using DEEP.UI;
using DEEP.HUD;
using DEEP.Stage;
using DEEP.Weapons;
using DEEP.DoorsAndKeycards;

namespace DEEP.Entities.Player
{

    // ========================================================================================================================
    // Class that controls the gameplay flow of the player and links it's components.
    // ========================================================================================================================
    [RequireComponent(typeof(PlayerEntity))]
    [RequireComponent(typeof(PlayerMovementation))]
    [RequireComponent(typeof(PlayerWeaponController))]
    public class PlayerController : MonoBehaviour
    {

        // ====================================================================================================================
        // Player State
        // ====================================================================================================================

        protected enum State { Playing, Paused, Dead, End, Locked }

        // Used to determine what state the game is currently in.
        // Controls the player ability to move, use weapons and interact. Also controls cursor and time.
        private State currentState;
        protected State CurrentState {
            get { return currentState; }
            set {
                switch(value) {
                    case State.Playing:
                        SetControl(true);   SetCursor(true);    SetTime(1.0f);
                        break;
                    case State.Paused:
                        SetControl(false);  SetCursor(false);   SetTime(0.0f);
                        break;
                    case State.Dead:
                        SetControl(false);  SetCursor(true);    SetTime(1.0f);
                        break;
                    case State.End:
                        SetControl(false);  SetCursor(false);   SetTime(0.0f);
                        break;
                    case State.Locked:
                        SetControl(false);  SetCursor(true);    SetTime(1.0f);
                        break;
                }
                currentState = value;
            }
        }

        // Sets the ability of the player to control it's movementation and weapons.
        protected virtual void SetControl(bool enabled) { 
            Movementation.enabled = enabled; 
            Weapons.enabled = enabled; 
        }
        // Sets the cursor lock, if the cursor is locked it also becomes invisible.
        protected virtual void SetCursor(bool locked) { 
            Cursor.visible = (!locked); 
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None; 
        }
        // Sets the timeScale.
        protected virtual void SetTime(float scale) { Time.timeScale = scale; }

        // ====================================================================================================================
        // Pause
        // ====================================================================================================================

        [Header("Pause")]
        [Tooltip("GameObject that contains the pause menu.")]
        [SerializeField] protected GameObject pauseMenu = null;

        // Pauses and un-pauses the game.
        public virtual void TogglePause() {

            // Checks if the player can toggle pause.
            if (CurrentState != State.Playing && currentState != State.Paused) return;

            // Inverts the pause state.
            CurrentState = (currentState == State.Paused) ? State.Playing : State.Paused;
            pauseMenu.SetActive(currentState == State.Paused);

            // Uses the pausing of the game to collect garbage as potential lag will be less noticeable.
            if(CurrentState == State.Paused) System.GC.Collect();

        }

        // ====================================================================================================================
        // Death
        // ====================================================================================================================

        [Header("Death")]
        [Tooltip("The screen overlay for when the player dies.")]
        [SerializeField] protected GameObject deathScreen = null;
        [Tooltip("The menu items for when the player dies.")]
        [SerializeField] protected GameObject deathMenu = null;
        [Tooltip("Possible audio clips for player death.")]
        [SerializeField] protected AudioClip[] playerDeath;

        // Handles the Player entity's death.
        public void Die() {
            
            // Avoids zombie deaths
            if(CurrentState != State.Playing) return;

            // Ends the game and player control.
            CurrentState = State.Dead;

            // Starts the death process, plays animation and audio.
            Camera.main.GetComponent<Animator>().SetBool("Death", true);
            feedbackAudioSource.PlayOneShot(playerDeath[Random.Range(0, playerDeath.Length)], 1.0f);
            deathScreen.SetActive(true);

            // Shows the menu after some time.
            StartCoroutine(ShowDeathMenu());

        }

        // Shows death menu after a certain amount of time.
        protected IEnumerator ShowDeathMenu()
        {

            // Waits for the delay.
            float time = 0;
            while(time < 2.0f) // Waits for the delay.
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Finishes the game and shows the death menu.
            CurrentState = State.End;
            deathMenu.SetActive(true);

        }

        // ====================================================================================================================
        // Level-End
        // ====================================================================================================================

        [Header("Level-End")]
        [Tooltip("The screen overlay for when the level ends.")]
        [SerializeField] protected EndScreen endLevelScreen = null;

        // Finishes the game and shows the end level menu.
        public void EndLevel() {
            Debug.Log("Level completed!");
            CurrentState = State.End;
            endLevelScreen.ShowScreen(); 
        }

        // ====================================================================================================================
        // Feedback
        // ====================================================================================================================

        [Header("Feedback")]
        [Tooltip("Audio source used to play clips related to feedback to the player.")]
        public AudioSource feedbackAudioSource = null;

        // ====================================================================================================================
        // Component References
        // ====================================================================================================================

        // PlayerEntity reference.
        private PlayerEntity entity;
        public PlayerEntity Entity { get { return entity; } }

        // PlayerMovementation reference.
        private PlayerMovementation movementation;
        public PlayerMovementation Movementation { get { return movementation; } }

        // PlayerWeaponsController reference.
        private PlayerWeaponController weapons;
        public PlayerWeaponController Weapons { get { return weapons; } }

        // HUDController reference.
        private HUDController hud;
        public HUDController HUD { get { return hud; } }

        // KeyInventory instance.
        private KeyInventory keyInventory;
        // Returns the KeyInventory from this PlayerController, 
        // creates a new one if it doesn't exists yet.
        public KeyInventory Keys { get { return keyInventory; } }

        // ====================================================================================================================
        // ====================================================================================================================
        // ====================================================================================================================

        protected virtual void Awake()
        {
            
            Debug.Log("Initializing Player...");
            
            // Gets/creates the necessary components and sets their owner when needed.
            entity = GetComponent<PlayerEntity>();
            entity.Owner = this;

            movementation = GetComponent<PlayerMovementation>();
            movementation.Owner = this;

            weapons = GetComponent<PlayerWeaponController>();
            weapons.Owner = this;

            keyInventory = new KeyInventory();
            keyInventory.Owner = this;

            hud = GetComponentInChildren<HUDController>();
            if(hud == null)
                Debug.LogError("DEEP.Entities.Player.PlayerController.Start: No HUDController was found!");
            
            // Loads the player inventory if it wasn't reset.
            if(StageManager.Instance != null && !StageManager.Instance.GetResetInventory()) {

                // Loads the inventory for a save if there is one.
                // TODO: Load

                // If no save is avaliable gives the Player the first weapon.
                //weaponController.GiveWeapon(0, 0, null);

            }

            // Starts the game.
            CurrentState = State.Playing;
            
        }

        protected virtual void Update() {

            // Pauses and un-pauses the game.
            if (CurrentState == State.Playing || currentState == State.Paused) {
                if (Input.GetButtonDown("Cancel")) 
                    TogglePause(); 
            }

        }

        // Signalizes the player has found a secret.
        public void FoundSecret(AudioClip feedbackAudio) {    
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);
        }

    }

}