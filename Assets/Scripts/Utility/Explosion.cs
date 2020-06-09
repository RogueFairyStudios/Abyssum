using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Entities;

namespace DEEP.Utility
{

    public class Explosion : MonoBehaviour
    {

        [Tooltip("Damage done to entities that are not the player.")]
        [SerializeField] protected int entityDamage = 30;
        [Tooltip("Radius of the explosion which will inflict damage.")]
        [SerializeField] protected float explosionRadius = 3.0f;
        [Tooltip("Force applied to the objects in the radius of the explosion.")]
        [SerializeField] protected float explosionForce = 15.0f;
        [Tooltip("The explosion object will be deleted after this amount of time.")]
        [SerializeField] protected float explosionDuration = 5.0f;

        [Space(5)]

        [Header("Rocket Jump")]
        [Tooltip("Damage when dealt to self.")]
        [SerializeField] protected int playerDamage = 15;
        [Tooltip("Range in which the explosion will have maximum propulsion.")]
        [SerializeField] protected float maxPowerRadius = 1.5f;
        [Tooltip("Special, extra upwards force to rocket jump")]
        [SerializeField] protected float upwardsForce = 0.0f;

        // Used to stored which entities have already received damage from the explosion in order
        // to not apply damage multiple times for entities with more than 1 collider.
        HashSet<EntityBase> entitiesHit;

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        // Explodes immediatly.
        void Start() { 

            entitiesHit = new HashSet<EntityBase>();    
            Explode(); 
            
        }

        // Does damage and applies physics for the explosion.
        protected void Explode() {

            // Despawns the explosion after sometime.
            StartCoroutine(DespawnExplosion());

            // Gets and checks the objects hit by the explosion.
			Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
			foreach (Collider col in colliders) {

				// Tries to get the attached rigidbody of the object if there's one.
				Rigidbody rigid = col.attachedRigidbody;

				// Tries to get an entity script reference from the object.
				EntityBase entity;
				if(rigid != null)
				 	entity = rigid.GetComponent<EntityBase>();
				else
					entity = col.GetComponent<EntityBase>();

                if(entity == null) { // Applies explosion force to simulate physics if necessary.
                    if(rigid != null)
                        rigid.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    continue;
                }

                // Verifies if hasn't already verified this entity.
                if(!entitiesHit.Contains(entity)) {

                    // Checks if the object hit is a player.
                    if(entity.GetType() == typeof(Player)) {

                        // If it's a player, does decreased damage and applies better physics for rocket jumps.
                        entity.Damage(playerDamage, 0);

                        // If within maxPowerRadius, apply explosive force without distance decay, otherwise, apply distance decay
                        if(Vector3.Distance(this.transform.position, rigid.transform.position) <= maxPowerRadius)
                            rigid.AddExplosionForce(explosionForce, transform.position, 0, upwardsForce, ForceMode.VelocityChange);
                        else
                            rigid.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsForce, ForceMode.VelocityChange);

                    } else { // Otherwise, applies regular explosion force and damage.

                        if (rigid != null) // Applies explosion force to simulate physics if necessary.
                            rigid.AddExplosionForce(explosionForce, transform.position, explosionRadius);

                            // Does damage.
                            entity.Damage(entityDamage, 0); 

                    }

                    // Adds the entity to the list of entities already hit.s
                    entitiesHit.Add(entity);
                    
                }
			}
		}

        // Despawns explosion after a time.
        protected IEnumerator DespawnExplosion() {

            float time = 0.0f; // Waits for the delay
            while (time < explosionDuration)
            {
                time += Time.fixedDeltaTime;
                yield return waitForFixed;
            }

            Destroy(gameObject);

        }

# if UNITY_EDITOR

        void OnDrawGizmos() {

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);

        }

# endif

    }
}
