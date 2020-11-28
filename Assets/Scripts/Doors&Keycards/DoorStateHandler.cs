using UnityEngine;

namespace DEEP.DoorsAndKeycards
    {

    public class DoorStateHandler : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            if(stateInfo.IsName("Opening")) {
                
                DoorBase door = animator.gameObject.GetComponent<DoorBase>();
                door.OnStartOpening();

            }

            if(stateInfo.IsName("Closing")) {
                
                DoorBase door = animator.gameObject.GetComponent<DoorBase>();
                door.OnStartClosing();

            }

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
          
            if(stateInfo.IsName("Opening")) {
                
                DoorBase door = animator.gameObject.GetComponent<DoorBase>();
                door.OnFinishOpening();

            }

            if(stateInfo.IsName("Closing")) {
                
                DoorBase door = animator.gameObject.GetComponent<DoorBase>();
                door.OnFinishClosing();

            }

        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
