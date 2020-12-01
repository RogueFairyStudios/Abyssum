using UnityEngine;

using DEEP.Pooling;

namespace DEEP.Entities{

    public class Blowfish : EnemyBase
    {
        [SerializeField] protected GameObject explosionPrefab = null;
        [SerializeField] protected AudioClip swim = null, inflate = null;
        [SerializeField] protected bool swimSoundEnabled = true;
        [SerializeField] protected float minSwimPitch = 0.8f, maxSwimPitch = 1.2f;
        [SerializeField] protected private AudioSource _audioloop = null;

        public override void Damage(int amount, DamageType type)
        {
            AI.Aggro();
            base.Damage(amount,type);
        }

        public void Inflate()
        {
            if(!_audio.isPlaying || _audio.clip != inflate)
            {
                _audioloop.Stop();
                _audio.Stop();
                _audio.clip = inflate;
                _audio.time = enemyAnimator.GetFloat("FuseTime");
                _audio.Play();
            }
        }

        public void Deflate()
        {
            if(_audio.clip == inflate)
                _audio.Stop();
        }

        public virtual void AIGrowl()
        {
            if(audioProfile.growl.clips.Length > 0)
            {
                _audio.Stop();
                _audio.clip = audioProfile.growl.clips[Random.Range(0, audioProfile.growl.clips.Length)];
                _audio.Play();
            }
        }

        private void Update()
        {
            if(swimSoundEnabled && enemyAnimator.GetBool("Swim") && !enemyAnimator.GetBool("Attack")) {
                _audioloop.clip = swim;
                _audioloop.pitch = Mathf.Lerp(maxSwimPitch, minSwimPitch, enemyAnimator.GetFloat("Speed"));
                if(!_audioloop.isPlaying)
                    _audioloop.Play();
            }
            else
                _audioloop.Stop();
        }

        public void Explode()
        {
            Transform currentPosition = this.transform;
            PoolingSystem.Instance.PoolObject(explosionPrefab, currentPosition.position, currentPosition.rotation);
            Destroy(this.gameObject);
        }
    }
}
