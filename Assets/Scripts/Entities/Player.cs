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
        [SerializeField] private float maxVelocity = 6f;
        [SerializeField] private float acceleration = 2f;

        [SerializeField] private Vector2 mouseSensitivity = new Vector2(1, 1);

        [Header("Weapons")]

        [Tooltip("Ammo sources carried by the player.")]
        public List<AmmoSource> ammoTypes;

        private Rigidbody _rigidbody = null;
        private Camera _camera = null;

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
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

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
            Vector3 movInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Calculates the Player velocity on the x-z plane.
            float planarVel = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z).magnitude;

            // If the max velocity on the x-z plane has not yet been achieved, accelerates the Player.
            if(planarVel < maxVelocity) _rigidbody.AddRelativeForce(movInput * acceleration * _rigidbody.mass, ForceMode.Impulse);

        }

        protected override void Die() {}

    }

}