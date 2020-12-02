using UnityEngine;

using DEEP.Pooling;

namespace DEEP.Entities{

    public class BlowfishEntity : EnemyBase
    {


        [SerializeField] protected AudioSource _audioloop = null;
        [SerializeField] protected bool swimSoundEnabled = true;

        protected BlowfishAudioProfile blowfishAudio;

        [Header("Explosion")]
        [SerializeField] protected GameObject explosionPrefab = null;

        protected override void Start()
        {
            base.Start();

            if(audioProfile is BlowfishAudioProfile)
                blowfishAudio = (BlowfishAudioProfile)audioProfile;
            else
                Debug.LogError("DEEP.Entities.Blowfish.Start: Audio profile is not a BlowfishAudioProfile!");
                
        }

        public override void Damage(int amount, DamageType type)
        {
            AI.Aggro();
            base.Damage(amount,type);
        }

        public void Inflate()
        {
            if(!_audio.isPlaying || _audio.clip != blowfishAudio.inflate)
            {
                _audioloop.Stop();
                _audio.Stop();
                _audio.clip = blowfishAudio.inflate;
                _audio.time = enemyAnimator.GetFloat("FuseTime");
                _audio.Play();
            }
        }

        public void Deflate()
        {
            if(_audio.clip == blowfishAudio.inflate)
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
                _audioloop.clip = blowfishAudio.swim;
                _audioloop.pitch = Mathf.Lerp(blowfishAudio.maxSwimPitch, blowfishAudio.minSwimPitch, enemyAnimator.GetFloat("Speed"));
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
