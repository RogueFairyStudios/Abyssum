using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Weapons.Bullets
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletSeeking : BulletBase
    {

        [SerializeField] protected float acceleration = 9.81f;

        Transform target;

        public override void OnEnable() {

            // Finds a target for the bullet.
            PlayerController player = FindObjectOfType<PlayerController>();
            target = player.transform;

            base.OnEnable();

        }

        protected override void FixedUpdate() {

            bRigidbody.AddForce(acceleration * (target.position - transform.position).normalized, ForceMode.Acceleration);

            if(bRigidbody.velocity.magnitude > 0)
                transform.localRotation = Quaternion.LookRotation(bRigidbody.velocity, Vector3.up);

            base.FixedUpdate();

        }

    }

}
