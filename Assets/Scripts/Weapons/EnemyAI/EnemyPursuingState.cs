using System;
using System.Collections;
using System.Collections.Generic;
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

    public static EnemyPursuingState instance{

        get{

            if(instance == null){
                new EnemyPursuingState();
            }
            
            return instance;
        }
    }

    public override void EnterState(EnemyAISystem owner){}

    public override void ExitState(EnemyAISystem owner){}

    public override void UpdateState(EnemyAISystem owner){}
}
