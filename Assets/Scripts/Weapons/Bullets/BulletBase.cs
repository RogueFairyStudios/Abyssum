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

        [Tooltip("Blood effect to be spawned when hitting an entity.")]
        [SerializeField] protected GameObject bloodEffect = null;

        [Tooltip("Effect to be spawned when hitting other objects.")]
        [SerializeField] protected GameObject otherHitEffect = null;

        private bool isTargeted = false;

        protected virtual void Awake() {

            //Gets the rigidbody.
            _rigidbody = GetComponent<Rigidbody>();        
        }

        private void Start()
        {
            if(isTargeted == false)
                _rigidbody.velocity = transform.forward * velocity;    
        }

        /// <summary>
        ///     Launches itself towards the target
        /// </summary>
        public virtual void SetTarget(Vector3 target)
        {
            Vector3 targetDir = target - transform.position;
            _rigidbody.velocity = targetDir.normalized * velocity;
            isTargeted = true;
        }

        protected virtual void OnCollisionEnter(Collision col) {

            // Tries to get an entity component from the object.
            EntityBase entity;
            Rigidbody rigid = col.rigidbody; // Verifies if the object hit has a rigidbody.
            if(rigid != null)
                entity = rigid.GetComponent<EntityBase>();
            else
                entity = col.gameObject.GetComponent<EntityBase>();

            // Checks if an entity was hit.
            if (entity != null) {
                
                // Spawn the blood splatter effect if avaliable and hit a player or enemy.
                if(bloodEffect != null  && (entity.GetType() == typeof(Player) || entity.GetType() == typeof(Enemy))) 
                    Instantiate(bloodEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));
                
                // Does the damage.
                entity.Damage(damage, 0);

            } else if(otherHitEffect != null) // Else, spawn the other hit effect if avaliable.
                Instantiate(otherHitEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));

            //Destroys the object on collision.
            Destroy(gameObject);

        }

    }

}
