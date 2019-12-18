using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DEEP.StateMachine;

public class EnemyWaitingState : State <EnemyAISystem>
{
    private static EnemyWaitingState instance;

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
    public override void EnterState(EnemyAISystem owner){

        owner.Waiting();
        if(owner.OnLoseAggro != null)
            owner.OnLoseAggro();

        Debug.Log("entering waiting state");

    }

    public override void ExitState(EnemyAISystem owner){
        Debug.Log("exiting waiting state");
    }

    public override void UpdateState(EnemyAISystem owner){
        
        if (owner.search) //verify if the enemy know where the target is
            owner.ChangeState(EnemyPursuingState.Instance);
        else
            owner.Waiting();
            
    }

}
