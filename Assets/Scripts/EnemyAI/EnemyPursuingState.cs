using UnityEngine;

using DEEP.StateMachine;

namespace DEEP.AI
{

    public class EnemyPursuingState : State<EnemyAISystem>
    {
        private static EnemyPursuingState instance;

# if UNITY_EDITOR
        // Only used in the Editor, set to false to hide logs.
        private bool showDebug = true;
# endif


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

#if UNITY_EDITOR
        if(showDebug) Debug.Log(owner.transform.name + ": Entering Enemy Pursuing State");
#endif

            owner.anim.SetBool("Walk", true);
            
            if(owner.HasTargetSight())
                owner.OnAggro?.Invoke();
        }

        public override void ExitState(EnemyAISystem owner){

#if UNITY_EDITOR
            if (showDebug) Debug.Log(owner.transform.name + ": Exiting Enemy Shooting State");
# endif

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
