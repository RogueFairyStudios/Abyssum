using UnityEngine;

public class OnStateExitSetBool : StateMachineBehaviour
{

    [SerializeField] private string parameterName = "no name";
    [SerializeField] private bool parameterValue = false;


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(parameterName, parameterValue);
    }


}