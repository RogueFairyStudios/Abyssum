using UnityEngine;

using DEEP.Entities;
using DEEP.Utility;

namespace DEEP.Weapons.Bullets {

	// Bullet that deal damage to entities in contact with the object hit.
	[RequireComponent(typeof(Rigidbody))]
	public class TeslaBullet : BulletBase {

        [Tooltip("Damage dealt to other entities in contact.")]
		[SerializeField] private int eletricDamage = 10;

		protected override void OnCollisionEnter(Collision col) {
            if (avoidDoubleHit && hasHit)
                return;
            hasHit = true;

            // Tries to get an entity component from the object.
            EntityBase entity;
            Rigidbody rigid = col.rigidbody; // Verifies if the object hit has a rigidbody.
            Debug.Log(col.gameObject);
            if(rigid != null)
                entity = rigid.GetComponent<EntityBase>();
            else
                entity = col.gameObject.GetComponent<EntityBase>();

            // Checks if an entity was hit.
            if (entity != null) {

                // Spawn the blood splatter effect if avaliable and hit a player or enemy.
                if (entity.bloodEffect != null)
                    Instantiate(entity.bloodEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));
                else
                    Instantiate(otherHitEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));

				if (entity.conductorBox != null) {
					entity.conductorBox.Electrify(eletricDamage);
				}
                // Does the damage.
                entity.Damage(damage, 0);

            } else if(otherHitEffect != null) // Else, spawn the other hit effect if avaliable.
                Instantiate(otherHitEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));

            //Destroys the object on collision.
            Destroy(gameObject);

        }

        protected void OnTriggerEnter(Collider col) {
            ConductorBox conductorBox = col.GetComponent<ConductorBox>();
            // Check if the object collided have an conductor box and doesnt have any entity attached to.
            if (conductorBox == null || conductorBox.entity != null)
                return;
            
            print("Goint go electrify the object " + col.name);
            // If those requisites are satisfied, then this object must be an conductor (water). So, Electrify!
            conductorBox.Electrify(eletricDamage);

            Destroy(gameObject);
        }

	}
}