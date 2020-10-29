using UnityEngine;

using DEEP.Decals;
using DEEP.Pooling;
using DEEP.Entities;

namespace DEEP.Weapons.Bullets
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletBase : PoolableObject
    {

        //Bullet's rigidbody.
        protected Rigidbody bRigidbody;

        [Tooltip("Velocity of the projectile.")]
        [SerializeField] protected float velocity = 10f;

        [Tooltip("Damage inflicted by the projectile.")]
        [SerializeField] protected int damage = 15;

        [Tooltip("Max time this bullet can exist before being dispawned.")]
        [SerializeField] protected float lifespam = 10.0f;

        [Tooltip("Bullet hole decal.")]
        [SerializeField] protected Material bulletHoleMaterial = null;

        [Tooltip("Bullet hole decal scale.")]
        [SerializeField] protected Vector2 bulletHoleScale = Vector2.one;

        [Tooltip("Effect to be spawned when hitting objects without special hit effects.")]
        [SerializeField] protected GameObject otherHitEffect = null;

        [Tooltip("If this bullet should have a attached TrailRenderer.")]
        [SerializeField] protected bool hasTrail = true;

        [Tooltip("The attached TrailRenderer.")]
        [SerializeField] protected TrailRenderer trail = null;

        protected bool isTargeted = false;
        protected bool avoidDoubleHit = true;
        protected bool hasHit = false;
        protected float lifeTimer = 0.0f;

        protected virtual void Awake()
        {

            // Gets the rigidbody.
            if(bRigidbody == null)
                bRigidbody = GetComponent<Rigidbody>();

            // Tries getting a bullet trail if necessary.
            if(hasTrail && trail == null)
                trail = GetComponentInChildren<TrailRenderer>();


        }

        public virtual void OnEnable() {

            hasHit = false;

            if(isTargeted == false)
                bRigidbody.velocity = transform.forward * velocity;

            if(hasTrail)
                trail.Clear();

            lifeTimer = 0.0f;

        }

        protected virtual void FixedUpdate()
        {

            // Dispawns the object after it's lifespam.
            if(lifeTimer < lifespam)
                lifeTimer += Time.fixedDeltaTime;
            else
                Despawn();

        }

        /// <summary>
        ///     Launches itself towards the target
        /// </summary>
        public virtual void SetTarget(Vector3 target)
        {
            Vector3 targetDir = target - transform.position;
            bRigidbody.velocity = targetDir.normalized * velocity;
            isTargeted = true;
        }

        protected virtual void OnCollisionEnter(Collision col) {

            if (avoidDoubleHit && hasHit)
                return;
            hasHit = true;

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
                if (entity.bloodEffect != null)
                    PoolingSystem.Instance.PoolObject(entity.bloodEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));
                else
                    PoolingSystem.Instance.PoolObject(otherHitEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));

                // Does the damage.
                entity.Damage(damage, 0);

            } else { // Else, tries spawning bullet hole decal the default hit effect.


                if(bulletHoleMaterial != null) {
                    DecalSystem.Instance.PlaceDecal(bulletHoleMaterial, col.contacts[0].point, col.contacts[0].normal, bulletHoleScale);
                }

                if(otherHitEffect != null)
                     PoolingSystem.Instance.PoolObject(otherHitEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));
            }

            // Despawns the object on collision.
            Despawn();

        }

    }

}
