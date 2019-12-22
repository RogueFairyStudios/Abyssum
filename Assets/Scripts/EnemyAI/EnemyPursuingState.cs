using UnityEngine;

using DEEP.StateMachine;

namespace DEEP.AI
{

    public class EnemyPursuingState : State<EnemyAISystem>
    {
        private static EnemyPursuingState instance;
        
        public EnemyPursuingState(){
            if (instance != null)
                return;
            instance = this;
        }

        public static EnemyPursuingState Instance{

            get {

                if(instance == null){
                    new EnemyPursuingState();
                }
                
                return instance;
            }
        }

        public override void EnterState(EnemyAISystem owner){

            Debug.Log("entering Enemy Pursuing State");

            owner.GoToTarget();

            if(owner.OnAggro != null)
                owner.OnAggro();
        }

        public override void ExitState(EnemyAISystem owner){

             Debug.Log("exiting Enemy Pursuing State");
            if(owner.OnLoseAggro != null)
                owner.OnLoseAggro();

        }

        public override void UpdateState(EnemyAISystem owner) {
            
            owner.Pursuing();//go to last know enemy position

            if (owner.InAttackRange()) {
                owner.ChangeState(EnemyShootingState.Instance);
                return;
            }

            if (!owner.HasSight(owner.target.transform.position) && owner.ReachedLastPosition()) {
                owner.ChangeState(EnemyWaitingState.Instance);
                return;
            }

        }
    }
}
