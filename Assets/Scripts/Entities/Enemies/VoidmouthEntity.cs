using UnityEngine;

namespace DEEP.Entities {

    public class VoidmouthEntity : EnemyBase {

        [SerializeField] protected AudioSource _audioloop = null;

        protected VoidMouthAudioProfile voidAudio;

        protected override void Start() {

            base.Start();

            if(audioProfile is VoidMouthAudioProfile)
                voidAudio = (VoidMouthAudioProfile)audioProfile;
            else
                Debug.LogError("DEEP.Entities.Voidmouth.Start: Audio profile is not a VoidMouthAudioProfile!");
            
            // Sets delegates to start and stop growling.
            if(voidAudio.idleHum != null)
                AI.OnAggro += PersueHum;
            if(voidAudio.persueHum)
                AI.OnLoseAggro += IdleHum;

            IdleHum();

        }

        protected void IdleHum() {
            if(_audioloop.clip != voidAudio.persueHum)
            {
                _audioloop.Stop();
                _audioloop.clip = voidAudio.idleHum;
                _audioloop.Play();
            }
        }

        protected void PersueHum() {
            if(_audioloop.clip != voidAudio.persueHum)
            {
                _audioloop.Stop();
                _audioloop.clip = voidAudio.persueHum;
                _audioloop.Play();
            }
        }
    }
}
