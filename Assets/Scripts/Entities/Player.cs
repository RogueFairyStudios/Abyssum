using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
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

        [Header("Weapons")] // ==============================================================================

        [Tooltip("Weapons carried by the player.")]
        public List<PlayerWeapon> weapons;

        // Stores the weapons instances with their info.
        List<Tuple<bool, GameObject>> weaponInstances;

        [Tooltip("Where Player weapons should be.")]
        public Transform weaponPosition;

        // Stores the current weapon.
        [SerializeField] private WeaponBase currentWeapon;

        [Tooltip("Ammo sources carried by the player.")]
        public List<AmmoSource> ammoTypes;
        // Stores a dictionary with the AmmoSource instances.
        private Dictionary<string, AmmoSource> ammoDict;

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
        [SerializeField] private AudioSource feedbackAudioSource = null;
        [Tooltip("Animator used to play screen effects giving feedback to the player.")]
        [SerializeField] private Image screenFeedback = null;
        private Coroutine screenFeedbackAnim = null; // Stores the current screen feedback coroutine.
        [Tooltip("Duration of the screen feedback.")]
        [SerializeField] private float screenFeedbackDuration = 0.1f;
        [Tooltip("Color for the damage feedback.")]
        [SerializeField] private Color damageFeedbackColor = Color.red;
        [Tooltip("Color for the healing feedback.")]
        [SerializeField] private Color healingFeedbackColor = Color.green;
        [Tooltip("Color for the armor feedback.")]
        [SerializeField] private Color armorFeedbackColor = Color.blue;
        [Tooltip("Color for the weapon/ammo feedback.")]
        [SerializeField] private Color weaponAmmoFeedbackColor = Color.yellow;
        [Tooltip("Color for the keycard feedback.")]
        [SerializeField] private Color keycardFeedbackColor = Color.cyan;
        [Tooltip("Color for the secret feedback.")]
        [SerializeField] private Color secretFeedbackColor = Color.magenta;

        [Header("Death")] // =======================================================================
        [Tooltip("The screen overlay for when the player dies.")]
        [SerializeField] private GameObject deathScreen = null;
        [Tooltip("The menu items for when the player dies.")]
        [SerializeField] private GameObject deathMenu = null;

        // Player components ================================================================================

        private Rigidbody _rigidbody = null; // Player's Rigidbody.
        private CapsuleCollider _collider = null; // Player's CapsuleCollider.
        private Camera _camera = null; // Player's Camera.
        private HUDController _hud = null; // Player's HUD controller.

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

            // Gets the mouse sensitivity for mouselook.
            if(!PlayerPrefs.HasKey("Mouse sensitivity"))
                PlayerPrefs.SetFloat("Mouse sensitivity", 6.0f);
            sensitivity = PlayerPrefs.GetFloat("Mouse sensitivity");

            // Gets the original rotations for mouselook.
            originalBodyRotation = transform.localRotation;
            originalCamRotation = _camera.transform.localRotation;

            // Creates a dictionary with the ammo sources.
            ammoDict = new Dictionary<string, AmmoSource>();
            foreach(AmmoSource source in ammoTypes)
                if(!ammoDict.ContainsKey(source.id))
                    ammoDict.Add(source.id, Instantiate(source));

            // Weapon setup =============================================================================================

            // Instantiates the weapons.
            weaponInstances = new List<Tuple<bool, GameObject>>();
            foreach (PlayerWeapon weapon in weapons)
            {

                // Creates the weapons inside the weapon position.
                GameObject weaponObj = Instantiate(weapon.prefab, weaponPosition.position, weaponPosition.rotation);
                weaponObj.transform.SetParent(weaponPosition);
                // Disables the weapon at start.
                weaponObj.SetActive(false);

                // Gets the weapon script.
                WeaponBase weaponScript = weaponObj.GetComponent<WeaponBase>();
                if(weaponScript == null) Debug.LogError("DEEP.Entities.Player.Start: Weapon has no weapon script!");

                // Sets the ammo source of the weapon.
                if(weapon.ammoId != null && weapon.ammoId != "")
                    if(ammoDict.ContainsKey(weapon.ammoId))
                        weaponScript.ammoSource = ammoDict[weapon.ammoId];
                    else
                        Debug.LogError("DEEP.Entities.Player.Start: Ammo type not found!");

                // Adds the weapon to the list
                weaponInstances.Add(new Tuple<bool, GameObject>(weapon.enabled, weaponObj));

            }

            // If there are weapons, equips the first one by default.
            if(weaponInstances.Count > 0) SwitchWeapons(0);

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
                for(int i = 0; i < 10; i++)
                    if(Input.GetKeyDown(i.ToString()))
                    {
                        if(i != 0) SwitchWeapons(i - 1);  // Converts the key into an order
                        else if(i == 0) SwitchWeapons(9); // on a list.
                        break;
                    }

                // Firing weapons ==================================================================================
                if(Input.GetButton("Fire1") && currentWeapon != null) {

                    // Tries firing the weapon.
                    currentWeapon.Shot();

                    // After that, updates the ammo counter on the HUD.
                    _hud.SetAmmoCounter(ammoDict[currentWeapon.ammoSource.id].ammo);

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

        // Switches between the Player weapons.
        private void SwitchWeapons(int weaponNum)
        {

            // Verifies if it's a valid weapon, if it's not doesn't switch.
            if(weaponNum >= weaponInstances.Count || weaponInstances[weaponNum].Item1 == false)
                return;

            // Disables the current weapon object.
            if(currentWeapon != null) currentWeapon.gameObject.SetActive(false);

            // Assigns the new weapon as current weapon.
            currentWeapon = weaponInstances[weaponNum].Item2.GetComponent<WeaponBase>();

            // Enables the current weapon.
            currentWeapon.gameObject.SetActive(true);

            // Updates the ammo counter on the HUD.
            _hud.SetAmmoCounter(ammoDict[currentWeapon.ammoSource.id].ammo);

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
                StartScreenFeedback(damageFeedbackColor, screenFeedbackDuration);
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

        public virtual bool Heal (int amount, HealType type, AudioClip feedbackAudio) 
        {

            // Tries to heal the entity.
            bool healed = base.Heal(amount, type);

            // If the entity was healed plays the player feedback sound.
            if(healed && feedbackAudio != null) {

                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

                // Flicks the screen to give feedback.
                StartScreenFeedback(healingFeedbackColor, screenFeedbackDuration);

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
            StartScreenFeedback(armorFeedbackColor, screenFeedbackDuration);

            return true;

        }

        // Pick's up a weapon and enables it's use.
        public bool GiveWeapon(int slot, int ammo, AudioClip feedbackAudio) {

            // If the weapon was collected.
            bool collected = false;

            if(!weaponInstances[slot].Item1) {

                // Gets the old instance from the list.
                Tuple<bool, GameObject> weaponInstance;
                weaponInstance = weaponInstances[slot];
                weaponInstances.RemoveAt(slot);
                
                // Creates a new instance that is enabled and re-adds it to the list.
                Tuple<bool, GameObject> enabledInstance = new Tuple<bool, GameObject>(true, weaponInstance.Item2);
                weaponInstances.Insert(slot, enabledInstance);

                Debug.Log("Player.GiveWeapon: " + weaponInstance.Item2.transform.name + " has been collected!");

                collected = true;

                // Equips the weapon.
                    SwitchWeapons(slot);

            }

            // Gives ammo to the player.
            bool givenAmmo = false;
            if(ammo > 0)
                givenAmmo = GiveAmmo(ammo, weaponInstances[slot].Item2.GetComponent<WeaponBase>().ammoSource.id, null);

            // If collected, plays the player feedback sound.
            if((collected || givenAmmo) && feedbackAudio != null) {

                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

                // Flicks the screen to give feedback.
                StartScreenFeedback(weaponAmmoFeedbackColor, screenFeedbackDuration);

            }

            // Returns if the player has collected the weapon or it's ammo.
            return collected || givenAmmo;

        }

        // Gives a certain type of ammo to the player.
        public bool GiveAmmo(int amount, string type, AudioClip feedbackAudio) {

            // Checks if the ammo type is valid.
            if(!ammoDict.ContainsKey(type)) return false;
            
            // Checks if ammo is not maxed out.
            if(ammoDict[type].ammo >= ammoDict[type].maxAmmo) return false;

            // Adds ammo to the source.
            ammoDict[type].GainAmmo(amount);

            // Updates the ammo counter on the HUD.
            if(currentWeapon != null)
                _hud.SetAmmoCounter(ammoDict[currentWeapon.ammoSource.id].ammo);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Flicks the screen to give feedback.
            StartScreenFeedback(weaponAmmoFeedbackColor, screenFeedbackDuration);

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
            StartScreenFeedback(keycardFeedbackColor, screenFeedbackDuration);

        }

        // Sinalizes the player has found a secret;
        public void FoundSecret() {
            
            // Flicks the screen to give feedback.
            StartScreenFeedback(secretFeedbackColor, screenFeedbackDuration);

        }

        // Starts a screen feedback effect.
        private void StartScreenFeedback(Color color, float duration) {

            // If a feedback effect is already happening stop it and start a new one.
            if(screenFeedbackAnim != null)
                StopCoroutine(screenFeedbackAnim);

            screenFeedbackAnim = StartCoroutine(ScreenFeedbackAnim(color, duration));

        }

        private IEnumerator ScreenFeedbackAnim(Color color, float duration) {

            // Sets the feedback color and show it.
            screenFeedback.color = color;
            screenFeedback.enabled = true;

            // Waits for the duration.
            yield return new WaitForSeconds(duration);

            // Ends the feedback.
            screenFeedback.enabled = false;
            screenFeedbackAnim = null;

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

            // Ensures time is reset.
            Time.timeScale = 1;
            SceneManager.LoadScene(StageInfo.current.nextStageSceneName);

        }

        public void RestartGame() {

            // Ensures time is reset.
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        
        }

        public void RestartLevel() {

            // Ensures time is reset.
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        }

    }

}