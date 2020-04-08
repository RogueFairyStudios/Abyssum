using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.UI;
using DEEP.Weapons;
using DEEP.Stage;
using DEEP.DoorsAndKeycards;

namespace DEEP.Entities
{

    // Class that controls the Player.
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : EntityBase
    {

        // ARMOR ==========================================================================================

        // Entity's current armor;
        [Header("Armor")]
        [SerializeField] protected int armor = 0;

        [Tooltip("Players's max armor.")]
        [SerializeField] protected int maxArmor = 100;

        [Header("Movimentation")] // =======================================================================

        [Tooltip("Player acceleration on ground.")]
        [SerializeField] private float groundAcceleration = 3f;

        [Tooltip("If the Player is allowed jumping.")]
        [SerializeField] private bool canJump = true;

        [Tooltip("Player acceleration when jumping.")]
        [SerializeField] private float jumpAcceleration = 4f;

        [Range(0,5)]
        [Tooltip("Drag used to slow down the Player when walking.")]
        [SerializeField] private float groundDrag = 0.25f;

        [Range(0,5)]
        [Tooltip("Drag used to slow down the Player midair.")]
        [SerializeField] private float airDrag = 0.25f;

        [Space(10)]
        [Tooltip("Stores if the Player is touching the ground.")]
        [SerializeField] private bool onGround = false;

        [Range(float.Epsilon, Mathf.Infinity)]
        [Tooltip("How much height tolerance is used to determine if the Player is touching the ground (Proportional to the collider heigth).")]
        [SerializeField] private float heightTolerance = 1.1f;

        [Tooltip("Mask used for raycast checks.")]
        [SerializeField] private LayerMask raycastMask = new LayerMask();

        [Tooltip("Radius for the ground check.")]
        [SerializeField] private float checkRadius = 0.25f;

        [Header("MouseLook")] // ==============================================================================

        [Tooltip("Sensitivity for the mouselook")]
        [SerializeField] private float sensitivity = 4.0f;

        // Original rotations for body and camera.
        private Quaternion originalBodyRotation;
        private Quaternion originalCamRotation;

        float rotationX = 0F; // Rotation on the x angle.
	    float rotationY = 0F; // Rotation on the y angle.

        [Header("Control")] // ==============================================================================

        [Tooltip("If the Player should be allowed to move.")]
        public bool canMove = true;

        [Header("Pause")]

        [Tooltip("If the game is paused.")]
        public bool isPaused = true;
        [Tooltip("GameObject that contains the pause menu.")]
        [SerializeField] private GameObject pauseMenu = null;

        [Header("Feedback")] // =======================================================================
        [Tooltip("Audio source used to play clips related to feedback to the player.")]
        public AudioSource feedbackAudioSource = null;

        [Header("Death")] // =======================================================================
        [Tooltip("The screen overlay for when the player dies.")]
        [SerializeField] private GameObject deathScreen = null;
        [Tooltip("The menu items for when the player dies.")]
        [SerializeField] private GameObject deathMenu = null;

        // Player components ================================================================================
        [HideInInspector] public Rigidbody _rigidbody = null;
        [HideInInspector] public CapsuleCollider _collider = null;
        [HideInInspector] public Camera _camera = null;
        [HideInInspector] public HUDController _hud = null;
        [HideInInspector] public PlayerWeaponController _weaponController = null;

        private void OnDrawGizmos() // To visualize the ground check
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, checkRadius);
            if(_collider != null) {
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (_collider.height / 2.0f) * heightTolerance);
                Gizmos.DrawWireSphere(transform.position + Vector3.down * (_collider.height / 2.0f) * heightTolerance, checkRadius);
            }
        }

        protected override void Start()
        {

            base.Start();

            // Resets the time.
            Time.timeScale = 1;

            // Sets pause to false.
            isPaused = false;

            // Get components ===========================================================================================

            // Gets the Player's Rigidbody.
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true; // Freezes rotation for mouselook.		

            // Gets the Player's Rigidbody.
            _collider = GetComponent<CapsuleCollider>();

            // Gets the Player's Camera.
            _camera = GetComponentInChildren<Camera>();

            // Gets the Player's HUD Controller and initializes the UI.
            _hud = GetComponentInChildren<HUDController>();
            _hud.SetHealthCounter(health);
            _hud.SetArmorCounter(armor);

            // Gets the weapons controller and assigns the necessary references.
            _weaponController = GetComponentInChildren<PlayerWeaponController>();
            _weaponController._player = this;
            _weaponController._hud = _hud;


            // Gives the first weapon by default.
            _weaponController.SwitchWeapons(0);

            // Gets the mouse sensitivity for mouselook.
            if(!PlayerPrefs.HasKey("Mouse sensitivity"))
                PlayerPrefs.SetFloat("Mouse sensitivity", 6.0f);
            sensitivity = PlayerPrefs.GetFloat("Mouse sensitivity");

            // Gets the original rotations for mouselook.
            originalBodyRotation = transform.localRotation;
            originalCamRotation = _camera.transform.localRotation;

        }

        private void Update()
        {

            // Pause ======================================================================================== 

            // Pauses and unpauses the game.
            if(Input.GetButtonDown("Cancel"))
                if(isPaused || canMove)
                    TogglePause();

            // Physics ======================================================================================== 

            // Verifies if the Player is touching the ground.
            onGround = Physics.OverlapCapsule(transform.position, transform.position + Vector3.down * (_collider.height / 2.0f) * heightTolerance, checkRadius, raycastMask).Length != 0;

            // Turns gravity off if grounded to avoid sliding
            _rigidbody.useGravity = !onGround;
            _rigidbody.drag = onGround ? groundDrag : airDrag;

            // Verifies if the Player can move.
            if(canMove)
            {

                // MouseLook =======================================================================================

                // Rotates on the x-axis.
                rotationX += Input.GetAxis("Mouse X") * sensitivity;
                rotationX = ClampAngle (rotationX, -360.0f, 360.0f);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalBodyRotation * xQuaternion;

                // Rotates on the y-axis.
                rotationY += Input.GetAxis("Mouse Y") * sensitivity;
                rotationY = ClampAngle (rotationY, -90.0f, 90.0f);

                Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
                _camera.transform.localRotation = originalCamRotation * yQuaternion;

                // Jumping =======================================================================================

                // Makes the Player jump if touching the gorund.
                if(canJump && onGround && Input.GetButtonDown("Jump"))
                    _rigidbody.AddForce(Vector3.up * jumpAcceleration * _rigidbody.mass, ForceMode.Impulse);

                //Equiping weapons ===============================================================================

                // Verifies the number keys.
                if(Input.GetKeyDown("0")) // Checks for 0.
                    _weaponController.SwitchWeapons(9); // 0 is actually the last weapon (9).
                else {
                    for(int i = 1; i <= 9; i++) // Checks for the other keys.
                        if(Input.GetKeyDown(i.ToString()))
                            _weaponController.SwitchWeapons(i - 1);  // Converts the key into the weapon index of the list.
                }

                // Firing weapons ==================================================================================
                if(Input.GetButton("Fire1")) {

                    // Tries firing the weapon.
                    _weaponController.FireCurrentWeapon();

                }

            }

        }

        private void FixedUpdate()
        {

            // Velocity calculation =======================================================================

            // Gets the local velocity of the Rigidbody.
            Vector3 localVel = transform.InverseTransformDirection(_rigidbody.velocity);

            // Verifies if the Player can move.
            if(canMove)
            {

                //Input ====================================================================================

                // Gets the input for the Player movimentation.
                Vector2 movInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                // Clamps so that moving diagonally isn't faster than going straight
                if(movInput.magnitude > 1f)
                    movInput.Normalize();

                //Movement =================================================================================

                // In order to make movement be based on where you're looking, we get the direction the player is facing and move accordingly
                Vector3 movementVector = this.transform.forward * movInput.y + this.transform.right * movInput.x;
                _rigidbody.MovePosition(transform.position + movementVector * groundAcceleration * Time.fixedDeltaTime);

            }

        }

        // Clamps pĺayer's rotation angles.
        private float ClampAngle (float angle, float min, float max)
        {
            if (angle < -360.0f)
                angle += 360.0f;
            if (angle > 360.0f)
                angle -= 360.0f;
            return Mathf.Clamp (angle, min, max);
        }

        public override void Damage(int amount, DamageType type) {

            // Calculates the percent of damage that should be absorbed by armor.
            float armorAbsorption = Mathf.Clamp(armor / 100f, 0.3f, 1f);

            // Calculates the amount of damage to armor and health.
            int armorDamage = Mathf.Clamp((int)Math.Round(armorAbsorption * amount), 0, armor); // Clamps to ensure if armor breaks the remaining damage will go to health.
            int healthDamage = amount - armorDamage;

            // Decreases armor.
            armor -= armorDamage;

            // Decreases health.
            health -= healthDamage;

            if(health > 0) {
                // Flicks the screen to give feedback.
                _hud.StartScreenFeedback(HUDController.PlayerFeedback.Type.Damage);
            }

            OnChangeArmor();
            OnChangeHealth();

        }

        protected override void Die() {
            
            Debug.Log("You died!");

            // Stops player.
            _rigidbody.velocity = Vector3.zero;

            // Blocks player interactiuon.
            canMove = false;

            // Enables the death menu overlay.
            deathScreen.SetActive(true);

            // Plays death animation.
            _camera.GetComponent<Animator>().SetBool("Death", true);

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
                 _hud.StartScreenFeedback(HUDController.PlayerFeedback.Type.Healing);

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

            // Updates the armor counter on the HUD.
            OnChangeArmor();

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Flicks the screen to give feedback.
             _hud.StartScreenFeedback(HUDController.PlayerFeedback.Type.Armor);

            return true;

        }

        // Gives a keycard to the player.
        public void GiveKeyCard(KeysColors color, AudioClip feedbackAudio) {

            print("Adding the " + color.ToString() + " key to the inventory");
			InventoryKey.inventory.Add(color);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Flicks the screen to give feedback.
             _hud.StartScreenFeedback(HUDController.PlayerFeedback.Type.Keycard);

        }

        // Sinalizes the player has found a secret;
        public void FoundSecret() {
            
            // Flicks the screen to give feedback.
             _hud.StartScreenFeedback(HUDController.PlayerFeedback.Type.Secret);

        }

        private void OnChangeArmor() {

            _hud.SetArmorCounter(armor);

        }

        protected override void OnChangeHealth() {

            _hud.SetHealthCounter(health);

            base.OnChangeHealth();

        }

        public void TogglePause() {

            isPaused = !isPaused;

            if(isPaused) {

                canMove = false;
                pauseMenu.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                Time.timeScale = 0;

            } else {

                canMove = true;
                pauseMenu.SetActive(false);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                Time.timeScale = 1;

            }

        }

        // Updates mouse sensitivity from an outside script.
        public void UpdateMouseSensitivity(float sensitivity)
        {

            this.sensitivity = sensitivity;

        }

        public void EndLevel() {
            
            Debug.Log("Level completed!");

            _rigidbody.velocity = Vector3.zero;

            canMove = false;
            FindObjectOfType<EndScreen>().ShowScreen();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;
            
        }

        public void ContinueLevel() {

            SceneManager.LoadSceneAsync(StageInfo.current.nextStageSceneName);

        }

        public void RestartGame() {

            SceneManager.LoadSceneAsync(0);
        
        }

        public void RestartLevel() {

            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        
        }

    }

}