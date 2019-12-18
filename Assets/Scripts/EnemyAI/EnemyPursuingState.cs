using UnityEngine;
using DEEP.StateMachine;

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

        owner.OnAggro();

    }

    public override void ExitState(EnemyAISystem owner){}

    public override void UpdateState(EnemyAISystem owner) {
        
        if (!owner.HasSight(owner.target.transform.position) && owner.ReachedLastPosition()) {
            owner.ChangeState(EnemyWaitingState.Instance);
            return;
        }

        if (owner.InAttackRange()) {
            owner.ChangeState(EnemyShootingState.Instance);
            return;
        }

        owner.Pursuing();//go to last know enemy position

    }
}
