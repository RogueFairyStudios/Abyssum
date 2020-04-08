using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.Stage
{
    public class MusicChange : MonoBehaviour
    {
        [Tooltip("Audio source for music.")]
        [SerializeField] private AudioSource source = null;

        [Tooltip("If instead you just want to mute whatever song is playing.")]
        [SerializeField] private bool muteSong = false;

        [Tooltip("Which song to swap to.")]
        [SerializeField] private AudioClip song = null;

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<Entities.Player>())
            {
                if(!muteSong)
                {
                    source.clip = song;
                    source.Play();
                }
                else
                    source.Stop();

                Destroy(this.gameObject);
            }
        }
    }
}
