using UnityEngine;

namespace DEEP.Entities
{

    // Contains possible heal types.
    public enum HealType { Regular, Overload };
    // Contains possible damage types.
    public enum DamageType { Regular, Fire, Eletric, Drowning };

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
        
        protected virtual void Start()
        {

            health = maxHealth; // Sets the initial health to the maximum health.

        }

        // =================================================================================================

        // Heals the entity by a certain amount, allows the specification of a heal type.
        public virtual bool Heal (int amount, HealType type) 
        {
            
            // Doesn't heal because health is above max or overloaded max.
            if(type == HealType.Regular && health >= maxHealth) return false;
            if(type == HealType.Overload && health >= maxOverloadedHealth) return false;

            health += amount; // Adds the health.

            // Ensures not going above max health.
            if(type == HealType.Regular && health > maxHealth)
                health = maxHealth;
            else if(type == HealType.Overload && health > maxOverloadedHealth)
                health = maxOverloadedHealth;

            OnChangeHealth();

            return true;

        }

        // Deals a certain amount of damage to an entity and verifies if it's "dead", allows the specification of a damage type.
        public virtual void Damage (int amount, DamageType type) 
        { 
            // Decreases health and verifies if the entity has "died".
            health -= amount;
            OnChangeHealth();

        }

        // "Kills" an entity.
        protected abstract void Die();

        // Called when health changes.
        protected virtual void OnChangeHealth() { 
            
            // Verifies if the entity died.
            if(health <= 0)
                Die();

        }

    }
}