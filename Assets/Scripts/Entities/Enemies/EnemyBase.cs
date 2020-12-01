using System.Collections;

using UnityEngine;

using DEEP.AI;
using DEEP.Stage;
using DEEP.Entities.Player;

namespace DEEP.Entities
{

    // Base script for an enemy entity.
    public class EnemyBase : EntityBase
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

        // Reference to the entities AI.
        protected BaseEntityAI AI;

        // ====================================================================================================================

        [Header("Rendering")]

        [Tooltip("Reference to the SkinnedMeshRenderer of this enemy.")]
        public SkinnedMeshRenderer enemyRenderer;

        [Tooltip("Material variants for this enemy, one will be picked at random at start, leave empty for no variants.")]
        public Material[] materialVariants = null;
        
        [Tooltip("Amount of extra damage below 0 for the enemy to be instantly gibbed.")]
        [SerializeField] protected int gibThreshold = 30;

        // An spawned enemy doesn't count as a kill at the stage statistics.
        private bool spawned = false;
        [SerializeField] protected bool IsSpawned {

            get {
                return spawned;
            }

        }

        // ====================================================================================================================

        [Header("Animation")]
        public Animator enemyAnimator;

        // ====================================================================================================================

        [Header("Drop")]

        [Tooltip("Reference to the SkinnedMeshRenderer of this enemy.")]
        [SerializeField] public GameObject dropItem;

        [Tooltip("Where to spawn the dropped item.")]
        [SerializeField] public Transform dropPoint;

        // ====================================================================================================================

        [Header("Audio")]

        [Tooltip("Audio source for the audio clips.")]
        [SerializeField] protected AudioSource _audio;

        [Tooltip("Audio profile for this enemy.")]
        [SerializeField] protected EntityAudioProfile audioProfile;

        protected void DoGrunt() {
            if(audioProfile.grunt.clips.Length > 0 &&!_audio.isPlaying) {
                _audio.clip = audioProfile.grunt.clips[Random.Range(0, audioProfile.grunt.clips.Length)];
                _audio.Play();
                Invoke(nameof(DoGrunt), Random.Range(audioProfile.grunt.minInterval, audioProfile.grunt.maxInterval));
            }
        }

        protected virtual void DoGrowl() {
            if(audioProfile.growl.clips.Length > 0 && !_audio.isPlaying) {
                _audio.clip = audioProfile.growl.clips[Random.Range(0, audioProfile.growl.clips.Length)];
                _audio.Play();
                Invoke(nameof(DoGrowl), Random.Range(audioProfile.growl.minInterval, audioProfile.growl.maxInterval));
            }
        }

        // ====================================================================================================================

        protected virtual void Awake() {

             // Tries getting a reference to the enemy's AI if necessary.
            if(AI == null)
                AI = GetComponentInChildren<BaseEntityAI>();

            // Tries getting a reference to the enemy's Animator if necessary.
            if(enemyAnimator == null)
                enemyAnimator = GetComponentInChildren<Animator>();

            // Tries getting a reference to the enemy's Skinned Mesh Renderer if necessary.
            if(enemyRenderer == null)
                enemyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        }

        protected override void Start() {

            // Gets a random variant for the material if they are avaliable.
            if(materialVariants != null && materialVariants.Length > 0)
                enemyRenderer.material = materialVariants[Random.Range(0, materialVariants.Length)];

            // Creates an instance of the enemy's main material so it can be changed independently if using the KillableEntity shader.
            if(enemyRenderer.material.HasProperty("_Damage"))
                enemyRenderer.material = Instantiate(enemyRenderer.material); 

            // Sets delegates to start and stop growling.
            if(AI != null) {
                AI.OnAggro      += () => {   CancelInvoke(nameof(DoGrunt));    Invoke(nameof(DoGrowl), 0f);  };
                AI.OnLoseAggro  += () => {   CancelInvoke(nameof(DoGrowl));    Invoke(nameof(DoGrunt), 0f);  };
            }

            // Starts grunting if clips are available.
            if(audioProfile != null && audioProfile.grunt.clips.Length > 0)
                Invoke(nameof(DoGrunt), Random.Range(audioProfile.grunt.minInterval, audioProfile.grunt.maxInterval));       

        }

        public override void Damage(int amount, DamageType type){

            // Aggro the AI if there is one
            if(AI != null)
                AI.Aggro();

            // Saves the old value.
            int prevHealth = CurrentHealth();

            // Plays the damage sound.
            if(audioProfile != null && audioProfile.damage.Length > 0) {
                _audio.clip = audioProfile.damage[Random.Range(0, audioProfile.damage.Length)];
                _audio.Play();
            }

            base.Damage(amount,type);
            OnChangeHealth(prevHealth, CurrentHealth());

        }

        // Called when health changes.
        protected override void OnChangeHealth(int oldValue, int newValue) { 
            
            // Saves the old value.
            int prevHealth = CurrentHealth();

            // Applies the damage effect to the material if avaliable.
            if(enemyRenderer.material.HasProperty("_Damage"))
                enemyRenderer.material.SetFloat("_Damage", Mathf.Clamp(1 - ((float)health / (float)maxHealth), 0.0f, 1.0f));

            base.OnChangeHealth(prevHealth, CurrentHealth());

        }

        // Used to mark that an enemy has being spawned after the level start.
        public void MarkSpawned() { spawned = true; }

        // "Kills" an entity.
        protected override void Die() {

            // Checks if the entity isn't already dead.
            if(isDead)
                return;

            base.Die();

            // Deals with possible AI when dying.
            if(AI != null && AI is EnemyAISystem navmeshAI) {

                EnemyAISystem aiSystem = (EnemyAISystem)AI;

                // Plays the death animation.
                aiSystem.ownerEnemy.enemyAnimator.SetBool("Dead", true);

                // Destroys the AI.
                aiSystem.SelfDestroy();

            }

            // Counts this enemy's death as a kill.
            if(!IsSpawned)
                StageManager.Instance.CountKill();
            
            // Plays a random death sound.
            if(audioProfile != null && audioProfile.death.Length > 0)
                AudioSource.PlayClipAtPoint(audioProfile.death[Random.Range(0, audioProfile.death.Length)], transform.position, _audio.volume);

            // Spawns the item drop.
            if(dropItem != null)
                Transform.Instantiate(dropItem, dropPoint.position, dropPoint.rotation);

        }

        // Despawns after a certain amount of time.
        protected override IEnumerator DespawnDelay() {

            // Waits for the delay.
            float time = 0;
            while(time < despawnDeathDelay) { // Waits for the delay.
                
                // Gibs if receiving a certain amount of extra damage.
                if(health < -gibThreshold)
                    break;

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

            }

            Despawn();

        }              

        public override void SetSlow(){
            if(AI != null && AI is EnemyAISystem navmeshAI)
                navmeshAI.setSpeed(0.5f);
        }
        public override void SetBaseSpeed(){
            if(AI != null && AI is EnemyAISystem navmeshAI)
                navmeshAI.setSpeed(this.baseSpeed);
        }         

    }
}