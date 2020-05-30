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

            Debug.Log(owner.transform.name + ": Entering Enemy Pursuing State");

            owner.anim.SetBool("Walk", true);
            
        }

        public override void ExitState(EnemyAISystem owner){

            Debug.Log(owner.transform.name + ": Exiting Enemy Shooting State");

            if (owner.OnLoseAggro != null)
                owner.OnLoseAggro();

        }

        public override void UpdateState(EnemyAISystem owner) {

            owner.Pursuing();

            if (owner.InAttackRange()) {
                owner.ChangeState(EnemyShootingState.Instance);
                return;
            }

            if (owner.ReachedLastPosition()) {
                owner.ChangeState(EnemyWaitingState.Instance);
                return;
            }

        }
    }
}
