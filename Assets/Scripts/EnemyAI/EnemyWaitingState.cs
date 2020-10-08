using UnityEngine;
using UnityEngine.AI;

using DEEP.StateMachine;

namespace DEEP.AI
{

    public class EnemyWaitingState : State <EnemyAISystem>
    {
        private static EnemyWaitingState instance;

#if UNITY_EDITOR
        // Only used in the Editor, set to false to hide logs.
        private bool showDebug = false;
# endif

        public EnemyWaitingState(){
            if (instance != null)
                return;
            
            instance = this;
        }

        public static EnemyWaitingState Instance
        {
            get 
            {
                if (instance == null)
                {
                    new EnemyWaitingState();
                }

                return instance;
            }
        }
        public override void EnterState(EnemyAISystem owner)
        {

            // Makes sure enemy movement is reset.
            owner.anim.SetBool("Walk", false);
            owner.agent.ResetPath();
            owner.ResetPatrol();

            if (owner.OnLoseAggro != null)
                owner.OnLoseAggro();

#if UNITY_EDITOR
            if (showDebug) Debug.Log(owner.transform.name + ": Entering Enemy Waiting State");
# endif

        }

        public override void ExitState(EnemyAISystem owner)
        {

#if UNITY_EDITOR
            if (showDebug) Debug.Log(owner.transform.name + ": Exiting Enemy Waiting State");
# endif

        }

        public override void UpdateState(EnemyAISystem owner){

            if(owner.InAttackRange()) // Checks if can attack.
            {
                owner.ChangeState(EnemyShootingState.Instance);
                return;
            }

            if (owner.HasTargetSight()) // Checks  if target is on sight.
            {

                // If can reach target start pursuing.
                if (owner.GetPath(owner.ownerEnemy.TargetPlayer.transform.position).status == NavMeshPathStatus.PathComplete)
                    owner.ChangeState(EnemyPursuingState.Instance);//target finded, engaging
                else // If it can't be reached, just stares at it.
                    owner.getAim();

                return;
            }

            // Execute other waiting behaviours otherwise.
            owner.Waiting();

        }
    }
}
