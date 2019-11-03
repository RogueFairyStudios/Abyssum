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
        owner.waiting();
        Debug.Log("entering waiting state");
    }

    public override void ExitState(EnemyAISystem owner){
        Debug.Log("exiting wating state");
    }

    public override void UpdateState(EnemyAISystem owner){
        
        if (owner.search)//verify if the enemy know where the target is
        {
            if(owner.inRange())
                owner.ChangeState(EnemyShootingState.Instance);
            else
                owner.ChangeState(EnemyPursuingState.Instance);
        }
        else
            owner.waiting();
    }

}
