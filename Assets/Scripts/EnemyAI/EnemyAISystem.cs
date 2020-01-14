using DEEP.StateMachine;
using DEEP.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DEEP.AI
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAISystem : MonoBehaviour
    {

        private Vector3 originalPosition; // Stores agent original position.

        public NavMeshAgent agent;
        [Tooltip("The offset from agent position used to calculate sight.")]
        public Vector3 agentSightOffset;

        public Animator anim;

        [SerializeField] protected float detectRange = 40.0f;
        [SerializeField] protected float attackRange = 10.0f;

        public WeaponBase weapon;

        [Tooltip("Should the enemy face the player when attacking")]
        public bool aimOnAttack = true;

        public GameObject target;

        public Vector3 lastTargetLocation; //location to search if the target has been missed

        protected StateMachine<EnemyAISystem> enemySM;

        [Tooltip("Random movimentation settings")]
        [SerializeField] protected List<GameObject> patrolPoints = new List<GameObject>();
        [SerializeField] protected int actualPoint = 0;

        public delegate void Reaction();
        public Reaction OnAggro, OnLoseAggro;

        [SerializeField] protected LayerMask sightMask = new LayerMask();

        void Start()
        {

            originalPosition = transform.position; // Gets the original position.

            if (agent == null) // Tries getting the enemy NavMeshAgent if none is found.
                agent = GetComponent<NavMeshAgent>();

            // Calculates from what point of the agent to check for sight.
            agentSightOffset = Vector3.up * (agent.baseOffset + (agent.height / 2.0f));

            if (anim == null) // Tries getting the enemy Animator if none is found.
                anim = GetComponentInChildren<Animator>();

            if (weapon == null) // Tries getting the enemy weapon if none is found. 
                weapon = GetComponentInChildren<WeaponBase>();

            target = GameObject.FindGameObjectWithTag("Player"); // Find the player and set it as the target.
            lastTargetLocation = transform.position; // Inititializes lastTarget location with temporary value.

            // Creates and initializes the state machine.
            enemySM = new StateMachine<EnemyAISystem>(this);
            enemySM.ChangeState(EnemyWaitingState.Instance);//first state

            // Setups delegates.
            OnAggro += AlertAllies;

        }

        void Update()
        {
            enemySM.update(); // Update the actual state
        }

        public virtual void Waiting()
        {

            if (patrolPoints.Count > 0)
            {
                //randon movementation
                //is in the patrol point
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    agent.SetDestination(patrolPoints[actualPoint].transform.position);
                    actualPoint++;
                    actualPoint = (actualPoint) % patrolPoints.Count;
                }

                anim.SetBool("Walk", true);
                return;

            }

        }

        public virtual void Pursuing()
        {

            // Gets the enemy destination.
            Vector3 destination; 
            if (HasTargetSight())
                destination = target.transform.position;
            else
                destination = lastTargetLocation;

            // Ensures destination is on the NavMesh.
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(destination, out navHit, 50.0f, agent.areaMask))
            {

                // Tries getting a path to the destination.
                NavMeshPath path = GetPath(navHit.position);
                if (path.status == NavMeshPathStatus.PathComplete)
                    agent.SetPath(path);
                else // Returns to original position if unable to reach.
                    agent.SetDestination(originalPosition);

            }
            else // Returns to original position if unable to reach.
                agent.SetDestination(originalPosition);

        }

        public virtual void Shooting()
        {

            if (weapon != null)
            {
                // Tries to attack and plays the animation on success.
                bool attacked = weapon.Shot();

                if (attacked)
                    anim.SetBool("Attack", true);

            }

        }

        public NavMeshPath GetPath(Vector3 target)
        {

            // Calculates a path to the target.
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(target, path);

            return path;

        }

        public void getAim()
        {

            var pos = (lastTargetLocation - transform.position).normalized;
            /*if(pos.y != 0){
                //arms movimentation
            }*/
            var rotate = Quaternion.LookRotation(pos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
        }

        // Checks if has sight to target.
        public bool HasTargetSight()
        {

            // Checks for sight.
            bool hasSight = HasSight(target.transform.position);

            // Stores the target location if it is seen.
            if (hasSight)
                lastTargetLocation = target.transform.position;

            return hasSight;

        }

        // Checks if AI has sight of a point.
        public bool HasSight(Vector3 point)
        {

            // Checks for visibility blocks.
            if (Physics.Linecast(point, transform.position + agentSightOffset, sightMask))
                return false;

            // Checks for detection range.
            if (Vector3.Distance(point, transform.position + agentSightOffset) > detectRange)
                return false;

            return true;
        }

        public bool InAttackRange()
        {

            // Checks for sight in addition to the attack range.
            return (HasTargetSight() && (Vector3.Distance(transform.position, target.transform.position) <= attackRange));

        }

        public bool OutAttackRange() { return !InAttackRange(); }

        public virtual void ChangeState(State<EnemyAISystem> newState)
        {

            enemySM.ChangeState(newState);

        }

        public bool ReachedLastPosition()
        {

            return (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance);

        }

        public void Hitted()
        {

            if (enemySM.currentState != EnemyWaitingState.Instance)
                return;

            enemySM.ChangeState(EnemyPursuingState.Instance);

        }

        // Alerts close allies.
        public void AlertAllies()
        {

            EnemyAISystem[] allies = FindObjectsOfType<EnemyAISystem>();

            foreach (EnemyAISystem ally in allies)
            {

                if (HasSight(ally.transform.position + ally.agentSightOffset))
                    ally.Hitted();

            }

        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {

            if (target == null)
                return;

            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (HasSight(target.transform.position))
            {

                Gizmos.color = Color.blue;

                if (distance < attackRange)
                    Gizmos.color = Color.red;


            }
            else
                Gizmos.color = Color.white;

            Gizmos.DrawLine(lastTargetLocation, transform.position + agentSightOffset);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(agent.destination, transform.position);

        }

#endif

    }
}
