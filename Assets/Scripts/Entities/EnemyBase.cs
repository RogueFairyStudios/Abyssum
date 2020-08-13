using UnityEngine;

using DEEP.Stage;

namespace DEEP.Entities
{

    // Base script for an enemy entity (used to identify them).
    public abstract class EnemyBase : EntityBase
    {

        // An spawned enemy doesn't count as a kill at the stage statistics.
        private bool spawned = false;
        [SerializeField] protected bool IsSpawned {

            get {
                return spawned;
            }

        }

        // Used to mark that an enemy has being spawned after the level start.
        public void MarkSpawned() {
            spawned = true;
        }

        // "Kills" an entity.
        protected override void Die() {

            // Checks if the entity isn't already dead.
            if(isDead)
                return;

            // Counts this enemy's death as a kill.
            if(!IsSpawned)
                StageInfo.Instance.CountKill();
                
            base.Die();

        }

    }
}