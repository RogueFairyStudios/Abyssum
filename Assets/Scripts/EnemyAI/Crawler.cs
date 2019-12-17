using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DEEP.Weapons;
using DEEP.StateMachine;
using DEEP.Entities;


[RequireComponent(typeof(NavMeshAgent))]
public class Crawler : EnemyAISystem
{
    public override void Shooting() {

        if(Vector3.Distance(LastTargetLocation, this.transform.position) > 1.8f){   
            agent.SetDestination(LastTargetLocation);
        }else{
            agent.SetDestination(this.transform.position);
        }
        base.Shooting();
        
    }
}
