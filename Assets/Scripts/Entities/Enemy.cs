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


        private EnemyAISystem AI;

        protected override void Start()
        {
            base.Start();
            AI = GetComponent<EnemyAISystem>();
            Invoke(nameof(Grunt), Random.Range(minGruntInterval, maxGruntInterval));
        }

        protected void Grunt()
        {
            if(damage.Length > 0) {
                _audio.clip = damage[Random.Range(0, damage.Length)];
                _audio.Play();
                Invoke(nameof(Grunt), Random.Range(minGruntInterval, maxGruntInterval));
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

            //Destroys the object on collision.
                Destroy(gameObject);
        }
    }
}
