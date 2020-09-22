using UnityEngine;

using DEEP.DoorsAndKeycards;

namespace DEEP.Entities.Player
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovementation : MonoBehaviour
    {

        [Tooltip("Player acceleration on ground.")]
        public float baseGroundAcceleration = 8.0f;

        [Tooltip("Player acceleration on ground when under the slow effect.")]
        public float slowGroundAcceleration = 1.5f;

        private float groundAcceleration;

        [Tooltip("If the Player is allowed jumping.")]
        public bool canJump = true;

        [Tooltip("Player acceleration when jumping.")]
        public float jumpAcceleration = 7.5f;

        [Range(0, 5)]
        [Tooltip("Drag used to slow down the Player when walking.")]
        public float groundDrag = 4.0f;

        [Range(0, 5)]
        [Tooltip("Drag used to slow down the Player midair.")]
        public float airDrag = 1.0f;

        [Space(10)]
        [Tooltip("Stores if the Player is touching the ground.")]
        public bool onGround = false;

        [Range(float.Epsilon, Mathf.Infinity)]
        [Tooltip("How much height tolerance is used to determine if the Player is touching the ground (Proportional to the collider heigth).")]
        public float heightTolerance = 0.89f;

        [Tooltip("Mask used for raycast checks.")]
        public LayerMask raycastMask = new LayerMask();

        [Tooltip("Radius for the ground check.")]
        public float checkRadius = 0.25f;

        [Tooltip("Sensitivity for the mouselook")]
        public float sensitivity = 6.0f;

        [Tooltip("Layer mask for opening door's raycast.")]
        [SerializeField] protected LayerMask tryOpenMask = new LayerMask();

        // Original rotations for body and camera.
        protected Quaternion originalBodyRotation;
        protected Quaternion originalCamRotation;

        protected float rotationX = 0.0f; // Rotation on the x angle.
        protected float rotationY = 0.0f; // Rotation on the y angle.

        private Rigidbody pRigidbody = null;
        private CapsuleCollider pCollider = null;
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

        private void Update() {

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

        private void FixedUpdate() {

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
        private float ClampAngle(float angle, float min, float max) {
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

        void TryOpenDoor() {

			RaycastHit hit;
			if (Physics.Raycast(PlayerController.Instance.transform.position, PlayerController.Instance.transform.TransformDirection(Vector3.forward), out hit, 5.0f, tryOpenMask)) {
				try{
					hit.collider.GetComponent<Door>().TryOpenDoor();
				} catch {
					Debug.LogWarning("Couldn't access the ColorsDoor script from the object " + hit.collider.name);
				}
			}

		}

#if UNITY_EDITOR

        private void OnDrawGizmos() // To visualize the ground check
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
