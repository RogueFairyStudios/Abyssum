using UnityEngine;

using DEEP.AI;

namespace DEEP.Entities
{

    public class Voidmouth : EnemyBase
    {
        [SerializeField] protected AudioClip idleHum = null, persueHum = null;

        [Tooltip("Sound that plays out when the enemy is hit.")]
        [SerializeField] protected AudioClip[] damage = new AudioClip[0];

        [Tooltip("Sound that plays out when the enemy dies.")]
        [SerializeField] protected AudioClip[] death = new AudioClip[0];


        [SerializeField] protected AudioSource _audio = null, _audioloop = null;
        protected Animator _animator;

        protected BaseEntityAI AI;

        protected override void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            AI = GetComponentInChildren<BaseEntityAI>();

            // Sets delegates to start and stop growling.
            if(idleHum != null)
                AI.OnAggro += PersueHum;
            if(persueHum)
                AI.OnLoseAggro += IdleHum;

            IdleHum();

            base.Start();
        }

        protected void IdleHum()
        {
            if(_audioloop.clip != persueHum)
            {
                _audioloop.Stop();
                _audioloop.clip = idleHum;
                _audioloop.Play();
            }
        }

        protected void PersueHum()
        {
            if(_audioloop.clip != persueHum)
            {
                _audioloop.Stop();
                _audioloop.clip = persueHum;
                _audioloop.Play();
            }
        }

        public override void Damage(int amount, DamageType type){

            AI.Aggro();

            if(damage.Length > 0) {
                _audio.clip = damage[Random.Range(0, damage.Length)];
                _audio.Play();
            }

            base.Damage(amount,type);
        }

        protected override void Die(){

            // Checks if the entity isn't already dead.
            if(isDead)
                return;

            if(death.Length > 0)
                AudioSource.PlayClipAtPoint(death[Random.Range(0, death.Length)], transform.position, _audio.volume);

            base.Die();
            
        }
    }
}
