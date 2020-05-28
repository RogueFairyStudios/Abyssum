using UnityEngine;

namespace DEEP.Spawn
{

    public class CrawlerSpawnAfterAnim : StateMachineBehaviour
    {

        [SerializeField] private string parameterName = "no name";
        [SerializeField] private bool parameterValue = false;


        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

            animator.SetBool(parameterName, parameterValue);

            animator.transform.parent.GetComponent<Spawner>().Spawn();

        }


    }

}