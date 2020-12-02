using UnityEngine;

using DEEP.Weapons;
using DEEP.Entities;

namespace DEEP.AI
{

    public class SentryAI : BaseEntityAI
    {
        
        // Reference to the sentry weapon.
        public WeaponBase weapon;

        protected void Start()
        {

            if (weapon == null) // Tries getting the enemy weapon if none is found. 
                weapon = GetComponentInChildren<WeaponBase>();

            // Setups delegates.
            OnAggro += AlertAllies;
            OnAggro += Attack;
        }

        protected void FixedUpdate() { Sentry(); }

        public virtual void Sentry() {

            // Returns if there's no target to track.
            if(ownerEnemy == null || ownerEnemy.TargetPlayer == null)
                return;

            // Tries making an attack if the player is in range.
            if(Vector3.Distance(ownerEnemy.TargetPlayer.transform.position, transform.position) <= detectRange)
                Attack();
            
        }

        // Attempts to attack.
        protected virtual void Attack() {
            // Plays animations if shooting.
            if(weapon.Shot())
                ownerEnemy.enemyAnimator.SetBool("Attack", true);   
        }

# if UNITY_EDITOR
        private void OnDrawGizmos() { // To visualize the attack range.
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRange);
        }
# endif

    }
}
