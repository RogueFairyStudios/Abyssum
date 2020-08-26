using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Weapons.Bullets
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletSeeking : BulletBase
    {

        [SerializeField] protected float acceleration = 9.81f;

        private void FixedUpdate() {

            _rigidbody.AddForce(acceleration * (PlayerController.Instance.transform.position - transform.position).normalized, ForceMode.Acceleration);

            if(_rigidbody.velocity.magnitude > 0)
                transform.localRotation = Quaternion.LookRotation(_rigidbody.velocity, Vector3.up);

        }

    }

}
