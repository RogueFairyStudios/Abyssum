using UnityEngine;

namespace DEEP.Entities
{

    // Contains information about an entities sounds.
    [CreateAssetMenu(fileName = "newBlowfishAudioProfile", menuName = "ScriptableObjects/Audio Profiles/Blowfish", order = 2)]
    public class BlowfishAudioProfile : EntityAudioProfile {

        public AudioClip swim = null, inflate = null;

        public float minSwimPitch = 0.8f, maxSwimPitch = 1.2f;

    }
}