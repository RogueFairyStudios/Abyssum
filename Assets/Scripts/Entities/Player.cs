using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Weapons;

namespace DEEP.Entities
{

    // Class that controls the Player.
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : EntityBase
    {

        [Header("Movimentation")] // =======================================================================

        [Tooltip("Max velocity the Player can reach when walking.")]
        [SerializeField] private float maxVelocity = 6f;

        [Tooltip("Player acceleration on ground.")]
        [SerializeField] private float groundAcceleration = 3f;

        [Tooltip("Player acceleration on air.")]
        [SerializeField] private float airAcceleration = 3f;

        [Tooltip("If the Player is allowed jumping.")]
        [SerializeField] private bool canJump = true;

        [Tooltip("Player acceleration when jumping.")]
        [SerializeField] private float jumpAcceleration = 4f;

        [Range(0,1)]
        [Tooltip("Drag used to slow down the Player when no walking.")]
        [SerializeField] private float groundDrag = 0.25f;

        [Space(10)]
        [Tooltip("Stores if the Player is touching the ground.")]
        [SerializeField] private bool onGround = false;

        [Range(1 + float.Epsilon, Mathf.Infinity)]
        [Tooltip("How much height tolerance is used to determine if the Player is touching the ground (Proportional to the collider heigth).")]
        [SerializeField] private float heigthTolerance = 1.1f;

        [Tooltip("Mask used for raycast checks")]
        [SerializeField] private LayerMask raycastMask = new LayerMask();


        [Space(10)]
        [Tooltip("How fast the Player should turn.")]
        [SerializeField] private Vector2 mouseSensitivity = new Vector2(1, 1);

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

        // Player components ================================================================================
        private Rigidbody _rigidbody = null; // Player's Rigidbody.
        private CapsuleCollider _collider = null; // Player's CapsuleCollider.
        private Camera _camera = null; // Player's Camera.

        protected override void Start()
        {

            base.Start();

            // Get components ===========================================================================================

            // Gets the Player's Rigidbody.
            _rigidbody = GetComponent<Rigidbody>();

            // Gets the Player's Rigidbody.
            _collider = GetComponent<CapsuleCollider>();

            // Gets the Player's Camera.
            _camera = GetComponentInChildren<Camera>();
            if(_camera == null) Debug.LogError("DEEP.Entities.Player.Start: Camera not found!");

            // Creates a dictionary with the ammo sources.
            ammoDict = new Dictionary<string, AmmoSource>();
            foreach(AmmoSource source in ammoTypes)
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

            // Physics ======================================================================================== 

            // Verifies if the Player is touching the ground.
            onGround = Physics.Raycast(transform.position, -Vector3.up, (_collider.height / 2.0f) * heigthTolerance, raycastMask);

            // Verifies if the Player can move.
            if(canMove)
            {

                // Rotation =======================================================================================

                // Gets the input for the Player rotation.
                Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                // Rotates the Player arount the y-axis.
                transform.transform.Rotate(new Vector3(0, mouseInput.x * 360 * mouseSensitivity.x * Time.deltaTime, 0));

                // Rotates the Player's camera arount the x-axis.
                Vector3 camRotation = _camera.transform.localEulerAngles; // Gets the actual rotation.
                if(camRotation.x > 180) camRotation.x -= 360; // Converts the angle to allow negative values.

                // Performs and clamps the camera rotation.
                camRotation.x = Mathf.Clamp(camRotation.x - (mouseInput.y * 360 * mouseSensitivity.y * Time.deltaTime), -90, 90);
                _camera.transform.localEulerAngles = camRotation; // Assigns the correct rotation.

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
                if(Input.GetButton("Fire1") && currentWeapon != null)
                    currentWeapon.Shot();

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

                //Input =======================================================================================

                // Gets the input for the Player movimentation.
                Vector3 movInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

                // Drag ========================================================================================

                // Applies drag to the Rigidbody if the Player is on the ground (this is done via script in order to not apply 
                // drag on the y-axis).
                localVel.x *= (1 - groundDrag);
                localVel.z *= (1 - groundDrag);

                // Velocity clamp ==============================================================================

                Vector3 planeVel = localVel; planeVel.y = 0; // Gets the local velocity on the x-z plane.

                // Guarantees the Player velocity on the x-z plane never goes above maximum.
                if(planeVel.magnitude > maxVelocity)
                {
                    localVel.x = planeVel.normalized.x * maxVelocity; // Clamps the velocity.
                    localVel.z = planeVel.normalized.z * maxVelocity;
                }

                // Assigns the new velocity to the Player.
                _rigidbody.velocity = transform.TransformDirection(localVel);

                // Movimentation =================================================================================

                // Recalculates the velocity on the plane.
                planeVel = localVel; planeVel.y = 0;

                // Accelerates the Player in the x-z plane based on input and if it won't go above the maximum velocity.
                if(onGround) // First verifies with acceleration to use.
                {
                    if((planeVel + movInput * groundAcceleration * Time.fixedDeltaTime).magnitude < maxVelocity)
                        _rigidbody.AddRelativeForce(movInput * groundAcceleration * _rigidbody.mass, ForceMode.Impulse);
                }
                else
                {
                    if((planeVel + movInput * airAcceleration * Time.fixedDeltaTime).magnitude < maxVelocity)
                        _rigidbody.AddRelativeForce(movInput * airAcceleration * _rigidbody.mass, ForceMode.Impulse);
                }

            }

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

        }

        protected override void Die() {}

    }

}