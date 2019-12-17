using UnityEngine;
using DEEP.Weapons;

namespace DEEP.Entities{

    [RequireComponent(typeof(EnemyAISystem))]
    public class Enemy : EntityBase
    {
        [Space(5)]
        [Header("Audio")]
        [Tooltip("Audio source for the audio clips.")]
        [SerializeField] protected AudioSource _audio;

        [Tooltip("Grunts the entity shout out every now and then.")]
        [SerializeField] protected AudioClip[] grunt;
        
        [Tooltip("Min and max interval between grunts.")]
        [SerializeField] protected float minGruntInterval, maxGruntInterval;

        [Tooltip("Sound that plays out when the enemy is hit.")]
        [SerializeField] protected AudioClip[] damage;

        [Tooltip("Sound that plays out when the enemy dies.")]
        [SerializeField] protected AudioClip[] death;

        [Tooltip("Sound that plays out when the enemy is chasing the player.")]
        [SerializeField] protected AudioClip[] growl;

        [Tooltip("Min and max interval between growls.")]
        [SerializeField] protected float minGrowlInterval, maxGrowlInterval;


        private EnemyAISystem AI;

        protected override void Start()
        {
            base.Start();
            AI = GetComponent<EnemyAISystem>();

            // Sets delegates to start and stop growling.
            if(growl.Length > 0)
            {
                AI.OnAggro      = () => {   CancelInvoke(nameof(Grunt));    Invoke(nameof(Growl), 0f);  };
                AI.OnLoseAggro  = () => {   CancelInvoke(nameof(Growl));    Invoke(nameof(grunt), 0f);  };
            }

            Invoke(nameof(Grunt), Random.Range(minGruntInterval, maxGruntInterval));
        }

        protected void Grunt()
        {
            if(grunt.Length > 0)
            {
                if(!_audio.isPlaying)
                {
                    _audio.clip = grunt[Random.Range(0, grunt.Length)];
                    _audio.Play();
                }
                Invoke(nameof(Grunt), Random.Range(minGruntInterval, maxGruntInterval));
            }
        }

        private void Growl()
        {
            if(growl.Length > 0)
            {
                if(!_audio.isPlaying)
                {
                    _audio.clip = growl[Random.Range(0, growl.Length)];
                    _audio.Play();
                }
                Invoke(nameof(Growl), Random.Range(minGrowlInterval, maxGrowlInterval));
            }
        }

        public override void Damage(int amount, DamageType type){
            Debug.Log("enemy hitted");

            if(!this.AI.search){
                AI.search = true;
                AI.hitted();
            }

            if(damage.Length > 0) {
                _audio.clip = damage[Random.Range(0, damage.Length)];
                _audio.Play();
            }

            base.Damage(amount,type);
        }

        protected override void Die(){

            if(death.Length > 0)
                AudioSource.PlayClipAtPoint(death[Random.Range(0, death.Length)], transform.position, 1f);

            base.Die();
            
        }
    }
}
