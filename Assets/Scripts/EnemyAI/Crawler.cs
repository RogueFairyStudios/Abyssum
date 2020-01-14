using UnityEngine;
using UnityEngine.AI;

namespace DEEP.AI
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class Crawler : EnemyAISystem
    {
        public override void Shooting() {

            if(Vector3.Distance(lastTargetLocation, this.transform.position) > 1.8f){   
                agent.SetDestination(lastTargetLocation);
            }else{
                agent.SetDestination(this.transform.position);
            }
            base.Shooting();
            
        }
    }

}
