using UnityEngine;
using DEEP.StateMachine;

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

    public override void EnterState(EnemyAISystem owner){
        Debug.Log("entering Enemy Shooting State");
        owner.Shooting();
    }

    public override void ExitState(EnemyAISystem owner){
        Debug.Log("exiting Enemy Shooting State");
    }

    public override void UpdateState(EnemyAISystem owner){
        owner.Shooting();
    }
}