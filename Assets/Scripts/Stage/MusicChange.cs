using UnityEngine;

using DEEP.Entities;

namespace DEEP.Stage
{
    public class MusicChange : MonoBehaviour
    {
        [Tooltip("Audio source for music.")]
        [SerializeField] private AudioSource introSource = null, loopSource = null;

        [Tooltip("If instead you just want to mute whatever song is playing.")]
        [SerializeField] private bool muteSong = false;

        [Tooltip("Which song to swap to.")]
        [SerializeField] private AudioClip songLoop = null, songIntro = null;
        [SerializeField] private bool hasIntro = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag != "Player")
                return;

            if(!muteSong)
            {
                if(hasIntro)
                {
                    introSource.clip = songIntro;
                    loopSource.clip = songLoop;
                    introSource.Play();
                    loopSource.PlayDelayed(songIntro.length);
                }
                else
                {
                    introSource.Stop();
                    loopSource.clip = songLoop;
                    loopSource.Play();
                }
            }
            else
                loopSource.Stop();

            Destroy(this.gameObject);

        }
    }
}
