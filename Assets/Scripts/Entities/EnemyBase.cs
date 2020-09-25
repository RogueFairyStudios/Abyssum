using UnityEngine;

using DEEP.Stage;

using DEEP.Entities.Player;

namespace DEEP.Entities
{

    // Base script for an enemy entity (used to identify them).
    public abstract class EnemyBase : EntityBase
    {

        // PlayerController reference =========================================================================================
        private PlayerController controller;

        // Obtains and stores a reference to the PlayerController instance.
        public PlayerController TargetPlayer {
            get { 
                if(controller != null)
                    return controller; 
                else {
                    controller = FindObjectOfType<PlayerController>();
                    return controller;
                }
            } 
        }
        // ====================================================================================================================

        [Header("Rendering")]

        [Tooltip("Reference to the SkinnedMeshRenderer of this enemy.")]
        [SerializeField] public SkinnedMeshRenderer enemyRenderer;

        [Tooltip("Material variants for this enemy, one will be picked at random at start, leave empty for no variants.")]
        [SerializeField] public Material[] materialVariants = null;

        // An spawned enemy doesn't count as a kill at the stage statistics.
        private bool spawned = false;
        [SerializeField] protected bool IsSpawned {

            get {
                return spawned;
            }

        }

        protected override void Start()
        {

            // Tries getting a reference to the enemies Skinned Mesh Renderer if necessary.
            if(enemyRenderer == null)
                enemyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

            // Gets a random variant for the material if they are avaliable.
            if(materialVariants != null && materialVariants.Length > 0)
                enemyRenderer.material = materialVariants[Random.Range(0, materialVariants.Length)];

            // Creates an instance of the enemy's main material so it can be changed independently if using the KillableEntity shader.
            if(enemyRenderer.material.HasProperty("_Damage"))
                enemyRenderer.material = Instantiate(enemyRenderer.material);  

        }

        public override void Damage(int amount, DamageType type){

            base.Damage(amount,type);
            OnChangeHealth();

        }

        // Called when health changes.
        protected override void OnChangeHealth() { 
            
            // Applies the damage effect to the material if avaliable.
            if(enemyRenderer.material.HasProperty("_Damage"))
                enemyRenderer.material.SetFloat("_Damage", Mathf.Clamp(1 - ((float)health / (float)maxHealth), 0.0f, 1.0f));

            base.OnChangeHealth();

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
                StageManager.Instance.CountKill();
                
            base.Die();

        }

    }
}