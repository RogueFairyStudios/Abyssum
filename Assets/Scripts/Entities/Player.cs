using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Weapons;

namespace DEEP.Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : EntityBase
    {

        [Header("Movimentation")]

        [Tooltip("Max velocity the Player can reach when walking.")]
        [SerializeField] private float maxVelocity = 8f;

        [Tooltip("Player acceleration.")]
        [SerializeField] private float acceleration = 5f;

        [Range(0,1)]
        [Tooltip("Drag used to slow down the Player when no walking.")]
        [SerializeField] private float groundDrag = 0.25f;

        [Tooltip("How fast the Player should turn.")]
        [SerializeField] private Vector2 mouseSensitivity = new Vector2(1, 1);

        [Header("Weapons")]

        [Tooltip("Ammo sources carried by the player.")]
        public List<AmmoSource> ammoTypes;

        private Rigidbody _rigidbody = null; // Player's Rigidbody.
        private Camera _camera = null; // Player's Camera.

        protected override void Start()
        {

            base.Start();

            // Gets the Player's Rigidbody.
            _rigidbody = GetComponent<Rigidbody>();

            // Gets the Player's Camera.
            _camera = GetComponentInChildren<Camera>();
            if(_camera == null) Debug.LogError("DEEP.Entities.Player.Start: Camera not found!");

        }

        private void Update()
        {

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

        }

        private void FixedUpdate()
        {

            // Gets the input for the Player movimentation.
            Vector3 movInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            // Gets the local velocity of the Rigidbody.
            Vector3 localVel = transform.InverseTransformDirection(_rigidbody.velocity);
            Vector3 planeVel = localVel; planeVel.y = 0; // Gets the local velocity on the x-z plane.

            // Applies drag to the Rigidbody (this is done via script in order to not apply drag on the y-axis).
            if(movInput.x == 0)
                localVel.x *= (1 - groundDrag);
            if(movInput.z == 0)
                localVel.z *= (1 - groundDrag);

            // Guarantees the Player velocity on the x-z plane never goes above maximum.
            if(planeVel.magnitude > maxVelocity)
            {
                localVel.x = planeVel.normalized.x * maxVelocity; // Clamps the velocity.
                localVel.z = planeVel.normalized.z * maxVelocity;
            }

            // Assigns the new velocity to the Player.
            _rigidbody.velocity = transform.TransformDirection(localVel);

            // Recalculates the velocity on the plane.
            planeVel = localVel; planeVel.y = 0;

            // Accelerates the Player based on input and if it won't go above the maximum velocity.
            if((planeVel + movInput * acceleration * Time.fixedDeltaTime).magnitude < maxVelocity)
                _rigidbody.AddRelativeForce(movInput * acceleration * _rigidbody.mass, ForceMode.Impulse);

        }

        protected override void Die() {}

    }

}