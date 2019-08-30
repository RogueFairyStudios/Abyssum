using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.Weapons.Bullets
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletBase : MonoBehaviour
    {
        
        //Bullet's rigidbody.
        private Rigidbody _rigidbody;

        [Tooltip("Velocity of the projectile.")]
        [SerializeField] protected float velocity = 10f;

        protected virtual void Start()
        {

            //Gets the rigidbody.
            _rigidbody = GetComponent<Rigidbody>();
            // Applies the velocity.
            _rigidbody.velocity = transform.forward * velocity;
            
        }

        protected virtual void OnCollisionEnter(Collision col)
        {

            //Destroys the object on collision.
            Destroy(gameObject);

        }

    }

}
