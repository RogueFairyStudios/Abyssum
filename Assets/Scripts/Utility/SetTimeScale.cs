using UnityEngine;

namespace DEEP.Utility
{

    public class SetTimeScale : MonoBehaviour
    {

        [SerializeField] protected float timeScale = 1.0f;

        void Awake()
        {

            Time.timeScale = timeScale;

        }
    }

}
