using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Weapons.Bullets
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletSeeking : BulletBase
    {

        [SerializeField] protected float acceleration = 9.81f;

        protected override void FixedUpdate() {

            bRigidbody.AddForce(acceleration * (PlayerController.Instance.transform.position - transform.position).normalized, ForceMode.Acceleration);

            if(bRigidbody.velocity.magnitude > 0)
                transform.localRotation = Quaternion.LookRotation(bRigidbody.velocity, Vector3.up);

            base.FixedUpdate();

        }

    }

}
