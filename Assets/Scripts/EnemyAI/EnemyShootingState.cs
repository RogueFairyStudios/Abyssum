using UnityEngine;

using DEEP.StateMachine;

namespace DEEP.AI
{

    public class EnemyShootingState: State<EnemyAISystem>{

        private static EnemyShootingState instance;

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
            
            Debug.Log(owner.transform.name + ": Entering Enemy Shooting State");

            // Makes sure enemy is not move when shooting.
            owner.anim.SetBool("Walk", false);
            owner.agent.ResetPath();

            if(owner.OnAggro != null)
                owner.OnAggro();

        }

        public override void ExitState(EnemyAISystem owner)
        {
            Debug.Log(owner.transform.name + ": Exiting Enemy Shooting State");

        }

        public override void UpdateState(EnemyAISystem owner) {

            Debug.Log("shot");

            // Waits for attack to end.
            if (owner.anim.GetBool("Attack"))
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