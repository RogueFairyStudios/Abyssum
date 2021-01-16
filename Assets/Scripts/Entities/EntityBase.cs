using System.Collections;

using UnityEngine;

using DEEP.Utility;
using DEEP.Pooling;

namespace DEEP.Entities
{

    // Contains possible heal types.
    public enum HealType { Regular, Overload };
    // Contains possible damage types.
    public enum DamageType { Regular, Fire, Electric, Drowning, IgnoreArmor };

    // Base script for an entity (any object that has health).
    public abstract class EntityBase : MonoBehaviour
    {
        
        // HEALTH ==========================================================================================

        // Entity's current health;
        [Header("Health")]
        [SerializeField] protected int health;

        [Tooltip("Entity's max health.")]
        [SerializeField] protected int maxHealth = 100;

        [Tooltip("Entity's max overloaded health.")]
        [SerializeField] protected int maxOverloadedHealth = 200;

        [Tooltip("Prefab to be spawned for the blood effect.")]
        public GameObject bloodEffect = null;

        [Tooltip("Prefab to be spawned when the entity dies.")]
        [SerializeField] protected GameObject deathPrefab = null;

        [Tooltip("Delay between dying and despawning the entities.")]
        [SerializeField] protected float despawnDeathDelay = 0.0f;

        // Used to ensure Die(), can't be called twice.
        protected bool isDead;

        [Tooltip("Reference to the ConductorBox that represents the conductive range of this entity.")]
        [SerializeField] public ConductorBox conductorBox;
        protected float baseSpeed;


        protected virtual void Start()
        {

            health = maxHealth; // Sets the initial health to the maximum health.
            OnChangeHealth(maxHealth, maxHealth);
            
            isDead = false;

        }

        // =================================================================================================

        // Returns the current health of the entity.
        public int CurrentHealth() { return health; }

        // Heals the entity by a certain amount, allows the specification of a heal type.
        public virtual bool Heal (int amount, HealType type) 
        {
            
            // Saves the old value.
            int prevHealth = CurrentHealth();

            // Doesn't heal because health is above max or overloaded max.
            if(type == HealType.Regular && health >= maxHealth) return false;
            if(type == HealType.Overload && health >= maxOverloadedHealth) return false;

            health += amount; // Adds the health.

            // Ensures not going above max health.
            if(type == HealType.Regular && health > maxHealth)
                health = maxHealth;
            else if(type == HealType.Overload && health > maxOverloadedHealth)
                health = maxOverloadedHealth;

            // Handles any changes that have to be made when modifying health.
            OnChangeHealth(prevHealth, CurrentHealth());

            return true;

        }

        // Deals a certain amount of damage to an entity and verifies if it's "dead", allows the specification of a damage type.
        public virtual void Damage (int amount, DamageType type) 
        { 

            // Saves the old value.
            int prevHealth = CurrentHealth();

            // Decreases health and verifies if the entity has "died".
            health -= amount;

            // Handles any changes that have to be made when modifying health.
            OnChangeHealth(prevHealth, CurrentHealth());

        }

        // Called when health changes.
        protected virtual void OnChangeHealth(int oldValue, int newValue) { 
            
            // Verifies if the entity died.
            if(health <= 0)
                Die();

        }

        // "Kills" an entity.
        protected virtual void Die() {

            // Checks if the entity isn't already dead.
            if(isDead)
                return;

            if(despawnDeathDelay <= 0.0f)
                Despawn();
            else
                StartCoroutine(DespawnDelay());
                
            isDead = true;

        }
        
        // Despawns after a certain amount of time.
        protected virtual IEnumerator DespawnDelay()
        {

            // Waits for the delay.
            float time = 0;
            while(time < despawnDeathDelay) // Waits for the delay.
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            Despawn();

        }

        // Destroys the entity and spawns death prefab if available.
        protected virtual void Despawn() {

            if(deathPrefab != null) // Spawns a prefab after death if assigned.
                PoolingSystem.Instance.PoolObject(deathPrefab, transform.position, transform.rotation);

            Destroy(gameObject);

        }

        public virtual void SetSlow(){}
        public virtual void SetBaseSpeed(){}

    }
}