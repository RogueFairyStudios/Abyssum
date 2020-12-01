using UnityEngine;

namespace DEEP.Entities {

    public class Voidmouth : EnemyBase {
        [SerializeField] protected AudioClip idleHum = null, persueHum = null;

        [SerializeField] protected AudioSource _audioloop = null;

        protected override void Start() {

            base.Start();
            
            // Sets delegates to start and stop growling.
            if(idleHum != null)
                AI.OnAggro += PersueHum;
            if(persueHum)
                AI.OnLoseAggro += IdleHum;

            IdleHum();

        }

        protected void IdleHum() {
            if(_audioloop.clip != persueHum)
            {
                _audioloop.Stop();
                _audioloop.clip = idleHum;
                _audioloop.Play();
            }
        }

        protected void PersueHum() {
            if(_audioloop.clip != persueHum)
            {
                _audioloop.Stop();
                _audioloop.clip = persueHum;
                _audioloop.Play();
            }
        }
    }
}
