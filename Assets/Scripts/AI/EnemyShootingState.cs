using UnityEngine;

using DEEP.StateMachine;

namespace DEEP.AI
{

    public class EnemyShootingState: State<EnemyAISystem>{

        private static EnemyShootingState instance;

#if UNITY_EDITOR
        // Only used in the Editor, set to false to hide logs.
        private bool showDebug = false;
# endif

        public EnemyShootingState(){
            if (instance != null)
                return;
            
            instance = this;
        }

        public static EnemyShootingState Instance
        {
            get 
            {
                if (instance == null)
                {
                    new EnemyShootingState();
                }

                return instance;
            }
        }

        public override void EnterState(EnemyAISystem owner)
        {

#if UNITY_EDITOR
            if (showDebug) Debug.Log(owner.transform.name + ": Entering Enemy Shooting State");
# endif

            // Makes sure enemy is not move when shooting.
            owner.ownerEnemy.enemyAnimator.SetBool("Walk", false);
            owner.agent.ResetPath();

            if(owner.OnAggro != null)
                owner.OnAggro();

        }

        public override void ExitState(EnemyAISystem owner)
        {

#if UNITY_EDITOR
            if (showDebug) Debug.Log(owner.transform.name + ": Exiting Enemy Shooting State");
# endif

        }

        public override void UpdateState(EnemyAISystem owner) {

            // Waits for attack to end.
            if (owner.ownerEnemy.enemyAnimator.GetBool("Attack"))
            {
                // Aims towards target during attack.
                if (owner.aimOnAttack)
                    owner.getAim(); 

                return;
            }

            owner.getAim();// Aims towards target.
            if (owner.InAttackRange()) // Check if can attack.
                owner.Shooting();
            else //the enemy is trying to flee
                owner.ChangeState(EnemyPursuingState.Instance);
                
        }
    }
}