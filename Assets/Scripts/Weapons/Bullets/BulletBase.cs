using System.Net;
using System.Collections.ObjectModel;
using UnityEngine;
using DEEP.Entities;

namespace DEEP.Weapons.Bullets
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletBase : MonoBehaviour
    {
        
        //Bullet's rigidbody.
        private Rigidbody _rigidbody;

        [Tooltip("Velocity of the projectile.")]
        [SerializeField] protected float velocity = 10f;

        [Tooltip("Damage inflicted by the projectile.")]
        [SerializeField] protected int damage = 15;

        [Tooltip("Bllod effect to be spawned when hitting an entity.")]
        [SerializeField] protected GameObject bloodEffect = null;

        protected virtual void Start()
        {

            //Gets the rigidbody.
            _rigidbody = GetComponent<Rigidbody>();
            // Applies the velocity.
            _rigidbody.velocity = transform.forward * velocity;
            
        }

        protected virtual void OnCollisionEnter(Collision col)
        {
            GameObject hitted = col.gameObject;
            EntityBase entity = hitted.GetComponent<EntityBase>();
            if (entity != null)
            {
                
                if(bloodEffect != null) {

                    GameObject blood = Instantiate(bloodEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));

                }
                
                entity.Damage(damage, 0);

            }

            //Destroys the object on collision.
            Destroy(gameObject);

        }

    }

}
