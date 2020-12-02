using UnityEngine;
using UnityEngine.Audio;

namespace DEEP.Entities
{

    // Contains information about an entities sounds.
    [CreateAssetMenu(fileName = "newEntityAudioProfile", menuName = "ScriptableObjects/Audio Profiles/Entity", order = 1)]
    public class EntityAudioProfile : ScriptableObject {

        public AudioMixerGroup mixerGroup;

        public AudioClip[] damage;

        [Tooltip("Sound that plays out when the enemy dies.")]
        public AudioClip[] death;

        [System.Serializable]
        public struct Grunt {

            [Tooltip("Grunts the entity shout out every now and then.")]
            public AudioClip[] clips;
            
            [Tooltip("Min and max interval between grunts.")]
            public float minInterval, maxInterval;
        }
        public Grunt grunt;

        [System.Serializable]
        public struct Growl {

            [Tooltip("Sound that plays out when the enemy is chasing the player.")]
            public AudioClip[] clips;
            
            [Tooltip("Min and max interval between growls.")]
            public float minInterval, maxInterval;
        }
        public Growl growl;

        [Tooltip("Volume for the AudioSource.")]
        public float volume = 1.0f;
        [Tooltip("Pitch for the AudioSource.")]
        public float pitch = 1.0f;

    }
}