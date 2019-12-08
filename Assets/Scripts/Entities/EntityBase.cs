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
        [SerializeField] private int health;

        [Tooltip("Entity's max life.")]
        [SerializeField] private int maxHealth = 100;

        protected virtual void Start()
        {

            health = maxHealth; // Sets the initial health to the maximum health.

        }

        // =================================================================================================

        // Heals the entity by a certain amount, allows the specification of a heal type.
        public virtual void Heal (int amount, HealType type) 
        {
            
            health += amount; // Adds the health.

            // Allows going over max health based on the damage type.
            if(type != HealType.Overload && health > maxHealth)
                health = maxHealth;

        }

        // Deals a certain amount of damage to an entity and verifies if it's "dead", allows the specification of a damage type.
        public virtual void Damage (int amount, DamageType type) 
        { 
            // Decreases health and verifies if the entity has "died".
            health -= amount;
            if(health <= 0)
                Die();  
        }

        // "Kills" an entity.
        protected abstract void Die();

    }
}