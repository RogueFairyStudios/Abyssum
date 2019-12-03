using UnityEngine;

namespace DEEP.Utilty
{

    // Sets the bool of an animator, used to test animations.
    public class AnimatorSetBool : MonoBehaviour
    {

        [SerializeField] string parameterName = "";
        [SerializeField] bool parameterValue = true;

        void Start()
        {
            
            Animator animator = GetComponentInChildren<Animator>();
            animator.SetBool(parameterName, parameterValue);

        }

    }
}
