using UnityEngine;

using DEEP.AI;

namespace DEEP.Entities
{

    public class Voidmouth : EntityBase
    {
        [SerializeField] AudioClip idleHum, persueHum;

        [Tooltip("Sound that plays out when the enemy is hit.")]
        [SerializeField] protected AudioClip[] damage;

        [Tooltip("Sound that plays out when the enemy dies.")]
        [SerializeField] protected AudioClip[] death;


        [SerializeField] private AudioSource _audio, _audioloop;
        private Animator _animator;

        private BaseEntityAI AI;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            AI = GetComponentInChildren<BaseEntityAI>();

            // Sets delegates to start and stop growling.
            if(idleHum != null)
                AI.OnAggro += PersueHum;
            if(persueHum)
                AI.OnLoseAggro += IdleHum;

            IdleHum();
        }

        private void IdleHum()
        {
            if(_audioloop.clip != persueHum)
            {
                _audioloop.Stop();
                _audioloop.clip = idleHum;
                _audioloop.Play();
            }
        }

        private void PersueHum()
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

            if(death.Length > 0)
                AudioSource.PlayClipAtPoint(death[Random.Range(0, death.Length)], transform.position, _audio.volume);

            base.Die();
            
        }
    }
}
