﻿using UnityEngine;

using DEEP.DoorsAndKeycards;

namespace DEEP.Entities.Player
{

    // ========================================================================================================================
    // Class that manages the player's movementation and scenery interaction.
    // ========================================================================================================================
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovementation : MonoBehaviour
    {

        // PlayerController that owns this script.
        protected PlayerController ownerPlayer;
        public PlayerController Owner {
            get { return ownerPlayer; }
            set { ownerPlayer = value; }
        }

        [Tooltip("Player acceleration on ground.")]
        [SerializeField] protected float baseGroundAcceleration = 8.0f;

        [Tooltip("Player acceleration on ground when under the slow effect.")]
        [SerializeField] protected float slowGroundAcceleration = 1.5f;

        protected float groundAcceleration;

        [Tooltip("If the Player is allowed jumping.")]
        [SerializeField] protected bool canJump = true;

        [Tooltip("Player acceleration when jumping.")]
        [SerializeField] protected float jumpAcceleration = 7.5f;

        [Range(0, 5)]
        [Tooltip("Drag used to slow down the Player when walking.")]
        [SerializeField] protected float groundDrag = 4.0f;

        [Range(0, 5)]
        [Tooltip("Drag used to slow down the Player midair.")]
        [SerializeField] protected float airDrag = 1.0f;

        [Space(10)]
        [Tooltip("Stores if the Player is touching the ground.")]
        [SerializeField] protected bool onGround = false;

        [Range(float.Epsilon, Mathf.Infinity)]
        [Tooltip("How much height tolerance is used to determine if the Player is touching the ground (Proportional to the collider heigth).")]
        [SerializeField] protected float heightTolerance = 0.89f;

        [Tooltip("Mask used for raycast checks.")]
        [SerializeField] protected LayerMask raycastMask = new LayerMask();

        [Tooltip("Radius for the ground check.")]
        [SerializeField] protected float checkRadius = 0.25f;

        [Tooltip("Sensitivity for the mouselook")]
        [SerializeField] protected float sensitivity = 6.0f;

        [Tooltip("Layer mask for opening door's raycast.")]
        [SerializeField] protected LayerMask tryOpenMask = new LayerMask();

        // Original rotations for body and camera.
        protected Quaternion originalBodyRotation;
        protected Quaternion originalCamRotation;

        protected float rotationX = 0.0f; // Rotation on the x angle.
        protected float rotationY = 0.0f; // Rotation on the y angle.

        protected Rigidbody pRigidbody = null;
        protected CapsuleCollider pCollider = null;
        protected Camera pCamera = null;

        public void Start() {

            Debug.Log("Initializing PlayerMovementation...");

            // Gets the necessary components.
            pRigidbody = GetComponent<Rigidbody>();
            pCollider = GetComponent<CapsuleCollider>();
            pCamera = Camera.main;

            // Gets the mouse sensitivity.
            if (!PlayerPrefs.HasKey("Mouse sensitivity"))
                PlayerPrefs.SetFloat("Mouse sensitivity", 7.0f);
            sensitivity = PlayerPrefs.GetFloat("Mouse sensitivity");

            // Gets the original rotations for mouselook.
            originalBodyRotation = transform.localRotation;
            originalCamRotation = pCamera.transform.localRotation;

            // Initializes the player speed.
            SetBaseSpeed();

        }

        protected void Update() {

            // Physics ========================================================================================================

            // Verifies if the Player is touching the ground.
            onGround = Physics.OverlapCapsule(transform.position, transform.position + Vector3.down * (pCollider.height / 2.0f) * heightTolerance, checkRadius, raycastMask).Length != 0;

            // Turns gravity off if grounded to avoid sliding
            pRigidbody.useGravity = !onGround;
            pRigidbody.drag = onGround ? groundDrag : airDrag;

            // MouseLook ======================================================================================================

            // Rotates on the x-axis.
            rotationX += Input.GetAxis("Mouse X") * sensitivity;
            rotationX = ClampAngle(rotationX, -360.0f, 360.0f);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalBodyRotation * xQuaternion;

            // Rotates on the y-axis.
            rotationY += Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = ClampAngle(rotationY, -90.0f, 90.0f);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            pCamera.transform.localRotation = originalCamRotation * yQuaternion;

            // Jumping ========================================================================================================

            // Makes the Player jump if touching the ground.
            if (canJump && onGround && Input.GetButtonDown("Jump"))
                pRigidbody.AddForce(Vector3.up * jumpAcceleration * pRigidbody.mass, ForceMode.Impulse);

            // Opening doors ==================================================================================================

            if (Input.GetButtonDown("Interact"))	// If the player tries to interact, raycast to see if there is a door
				TryOpenDoor();

        }

        protected void FixedUpdate() {

            // Velocity calculation ===========================================================================================

            // Gets the local velocity of the Rigidbody.
            Vector3 localVel = transform.InverseTransformDirection(pRigidbody.velocity);

            // Input ==========================================================================================================

            // Gets the input for the Player movimentation.
            Vector2 movInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            // Clamps so that moving diagonally isn't faster than going straight
            if (movInput.magnitude > 1f)
                movInput.Normalize();

            // Movement =======================================================================================================

            // In order to make movement be based on where you're looking, we get the direction the player is facing and move accordingly
            Vector3 movementVector = this.transform.forward * movInput.y + this.transform.right * movInput.x;
            pRigidbody.MovePosition(transform.position + movementVector * groundAcceleration * Time.fixedDeltaTime);

        }

        // Clamps player's rotation angles.
        protected float ClampAngle(float angle, float min, float max) {
            if (angle < -360.0f)
                angle += 360.0f;
            if (angle > 360.0f)
                angle -= 360.0f;
            return Mathf.Clamp(angle, min, max);
        }

        public void SetSlow() {
            groundAcceleration = slowGroundAcceleration;
            canJump = false;
        }

        public void SetBaseSpeed() {
            groundAcceleration = baseGroundAcceleration;
            canJump = true;
        }

        public void SetMouseSensitivity(float sensitivity) { this.sensitivity = sensitivity; }

        protected void TryOpenDoor() {

			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5.0f, tryOpenMask)) {
				DoorBase door = hit.collider.GetComponent<DoorBase>();
                if(door != null)
					hit.collider.GetComponent<DoorBase>().TryOpenDoor(ownerPlayer.Keys);
			}

		}

#if UNITY_EDITOR

        protected void OnDrawGizmos() // To visualize the ground check
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, checkRadius);
            if (pCollider != null) {
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (pCollider.height / 2.0f) * heightTolerance);
                Gizmos.DrawWireSphere(transform.position + Vector3.down * (pCollider.height / 2.0f) * heightTolerance, checkRadius);
            }
        }

# endif

    }
}
