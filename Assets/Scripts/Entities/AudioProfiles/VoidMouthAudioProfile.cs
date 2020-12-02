using UnityEngine;

namespace DEEP.Entities
{

    // Contains information about an entities sounds.
    [CreateAssetMenu(fileName = "newVoidMouthAudioProfile", menuName = "ScriptableObjects/Audio Profiles/Void Mouth", order = 3)]
    public class VoidMouthAudioProfile : EntityAudioProfile {

        public AudioClip idleHum = null, persueHum = null;

    }
}