using UnityEngine;
using System;

namespace DEEP.Stage
{
    public class MusicChange : MonoBehaviour
    {
        [Tooltip("If instead you just want to mute whatever song is playing.")]
        [SerializeField] private bool muteSong = false;

        [Tooltip("Which song to swap to.")]
        [SerializeField] private AudioClip songLoop = null, songIntro = null;
        [SerializeField] private bool hasIntro = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag != "Player")
                return;

            try
            {
                if(!muteSong)
                    if(hasIntro)
                        MusicPlayer.instance.StartSong(songIntro, songLoop);
                    else
                        MusicPlayer.instance.StartSong(songLoop);
                else
                    MusicPlayer.instance.Stop();
            }
            catch(NullReferenceException nullref)
            {
                if(MusicPlayer.instance == null)
                    Debug.LogError("There is no music player in the scene! Please add the music player prefab to the scene!");
                else
                    throw nullref;
            }

            Destroy(this.gameObject);
        }
    }
}
