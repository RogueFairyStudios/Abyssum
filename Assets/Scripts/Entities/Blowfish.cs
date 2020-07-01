using UnityEngine;
using System.Linq;

using DEEP.AI;

namespace DEEP.Entities{

    public class Blowfish : EntityBase
    {
        [SerializeField] GameObject explosionPrefab = null;
        [SerializeField] AudioClip swim, inflate;
        [SerializeField] bool swimSoundEnabled;
        [SerializeField] float minSwimPitch, maxSwimPitch;

        [Tooltip("Sound that plays out when the enemy spots the enemy.")]
        [SerializeField] protected AudioClip[] growl;

        [Tooltip("Sound that plays out when the enemy is hit.")]
        [SerializeField] protected AudioClip[] damage;

        [Tooltip("Sound that plays out when the enemy dies.")]
        [SerializeField] protected AudioClip[] death;


        [SerializeField] private AudioSource _audio, _audioloop;
        private Animator _animator;

        private BlowfishAI AI;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            AI = GetComponentInChildren<BlowfishAI>();
        }

        public override void Damage(int amount, DamageType type)
        {
            AI.Aggro();

            if(damage.Length > 0 && !(_audio.isPlaying && (_audio.clip == inflate || damage.Contains(_audio.clip))))
            {
                _audio.clip = damage[Random.Range(0, damage.Length)];
                _audio.Play();
            }

            base.Damage(amount,type);
        }

        protected override void Die()
        {
            if(death.Length > 0)
                AudioSource.PlayClipAtPoint(death[Random.Range(0, death.Length)], transform.position, _audio.volume);

            base.Die();
        }


        public void Inflate()
        {
            if(!_audio.isPlaying || _audio.clip != inflate)
            {
                _audioloop.Stop();
                _audio.Stop();
                _audio.clip = inflate;
                _audio.time = _animator.GetFloat("FuseTime");
                _audio.Play();
            }
        }

        public void Deflate()
        {
            if(_audio.clip == inflate)
                _audio.Stop();
        }

        public void Growl()
        {
            if(growl.Length > 0)
            {
                _audio.Stop();
                _audio.clip = growl[Random.Range(0, damage.Length)];
                _audio.Play();
            }
        }

        private void Update()
        {
            if(swimSoundEnabled && _animator.GetBool("Swim") && !_animator.GetBool("Attack"))
            {
                _audioloop.clip = swim;
                _audioloop.pitch = Mathf.Lerp(maxSwimPitch, minSwimPitch, _animator.GetFloat("Speed"));
                if(!_audioloop.isPlaying)
                    _audioloop.Play();
            }
            else
                _audioloop.Stop();
        }

        public void Explode()
        {
            Transform currentPosition = this.transform;
            Destroy(this.gameObject);
            Instantiate(explosionPrefab, currentPosition.position, currentPosition.rotation);
        }
    }
}
