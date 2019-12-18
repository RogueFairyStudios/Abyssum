using UnityEngine;

namespace DEEP.Utility
{

    // Sets the bool of an animator, used to test animations.
    public class AnimatorParameterSet : MonoBehaviour
    {

        public enum ParameterType { Int, Float, Boolean }

        [SerializeField] string parameterName = ""; // Name of the parameter to be set.
        [SerializeField] ParameterType parameterType = ParameterType.Int; // Type of the parameter to be set.
        [SerializeField] int parameterIntValue = 1; // Value of the parameter to be set if it's an Int.
        [SerializeField] float parameterFloatValue = 1.0f; // Value of the parameter to be set if it's a Float.
        [SerializeField] bool parameterBooleanValue = true; // Value of the parameter to be set if it's a Boolean.

        void Start()
        {
            
            // Gets the animator.
            Animator animator = GetComponentInChildren<Animator>();

            // Sets the correct parameter.
            if(parameterType == ParameterType.Int) {
                animator.SetInteger(parameterName, parameterIntValue);
                return;
            }

            if(parameterType == ParameterType.Float) {
                animator.SetFloat(parameterName, parameterFloatValue);
                return;
            }

            if(parameterType == ParameterType.Boolean) {
                animator.SetBool(parameterName, parameterBooleanValue);
                return;
            }

        }

    }
}
