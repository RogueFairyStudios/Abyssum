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

        get{

            if(instance == null){
                new EnemyPursuingState();
            }
            
            return instance;
        }
    }

    public override void EnterState(EnemyAISystem owner){}

    public override void ExitState(EnemyAISystem owner){}

    public override void UpdateState(EnemyAISystem owner){
        if (owner.inRange())
            owner.ChangeState(EnemyShootingState.Instance);
        else{
            owner.Pursuing();//go to last know enemy position
            if (owner.outRange())
            {
                owner.ChangeState(EnemyWaitingState.Instance);
            }
            owner.Shooting();
        }
    }
}
