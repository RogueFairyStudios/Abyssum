using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DEEP.StateMachine;
using DEEP.Weapons;

namespace DEEP.AI
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAISystem : BaseEntityAI
    {

        // Has this enemy been spawned after the start of the game.
        public bool spawned = false;

        private Vector3 originalPosition; // Stores agent original position.

        public NavMeshAgent agent;

        public Animator anim;

        [SerializeField] protected float attackRange = 10.0f;
        
        public WeaponBase weapon;

        [Tooltip("Should the enemy face the player when attacking")]
        public bool aimOnAttack = true;

        [Tooltip("Is the enemy attack meele or ranged?")]
        [SerializeField] protected bool isMeeleAttack = false;

        protected StateMachine<EnemyAISystem> enemySM;

        [Tooltip("Random movimentation settings")]
        [SerializeField] protected List<GameObject> patrolPoints = new List<GameObject>();
        [SerializeField] protected int actualPoint = 0;

        [SerializeField]private bool reversePatrol = false;

        [Tooltip("If angle matters (for shooting for example) set from where the angle will be calculated.")]
        [SerializeField] protected Transform angleReference;

        [Tooltip("Reload system")]
        [SerializeField] protected float reloadTime = 0.0f;
        [SerializeField] protected int clipSize = 0; // how many bullets to shot before reload; 0 if dont want to reload
        private int bullets = 0;
        private float reloadingProcess = 0.0f; 

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

            // Setups delegates.
            OnAggro += AlertAllies;

            // Creates and initializes the state machine.
            enemySM = new StateMachine<EnemyAISystem>(this);
            enemySM.ChangeState(EnemyWaitingState.Instance);//first state

            //create bullets
            bullets = clipSize;
        }

        void Update()
        {
            if(enemySM != null)
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
                    if(!reversePatrol){
                        actualPoint++;
                        actualPoint = (actualPoint) % patrolPoints.Count;
                        if(actualPoint == (patrolPoints.Count - 1))
                            reversePatrol = !reversePatrol;
                    }
                    else{
                        actualPoint--;
                        actualPoint = (actualPoint) % patrolPoints.Count;
                        if(actualPoint == 0)
                            reversePatrol = !reversePatrol;                        
                    }
                }

                anim.SetBool("Walk", true);
                return;
            }
        }

        public virtual void ResetPatrol(){
            if (patrolPoints.Count > 0){
                actualPoint = 0;
                agent.SetDestination(patrolPoints[actualPoint].transform.position);
                reversePatrol = false;
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
                if (path.status != NavMeshPathStatus.PathComplete && isMeeleAttack)
                    agent.SetDestination(originalPosition); // Returns to original position if unable to reach and out of range.
                else 
                    agent.SetPath(path);          

            }
            else // Returns to original position if unable to reach.
                agent.SetDestination(originalPosition);

        }

        public virtual void Shooting()
        {

            if (weapon != null)
            {
                if(!canShoot())
                    return;
                bool attacked;

                // Tries to attack and plays the animation on success.
                if(weapon is SimpleWeapon simpleWeapon)
                    attacked = simpleWeapon.Shot(lastTargetLocation);
                else
                    attacked = weapon.Shot();

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

            if (HasTargetSight())
            {
                Vector3 pos = new Vector3(lastTargetLocation.x - transform.position.x, 0, lastTargetLocation.z - transform.position.z).normalized;
                /*if(pos.y != 0){
                    //arms movimentation
                }*/
                var rotate = Quaternion.LookRotation(pos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * (3 * Mathf.Deg2Rad * agent.angularSpeed));

                // If angle matters to the animation
                foreach(var param in anim.parameters)
                {
                    if(param.name == "Angle")
                    {
                        Vector3 targetDir   = lastTargetLocation - angleReference.position;
                        Vector3 right       = Vector3.Cross(-transform.right, transform.forward);
                        Vector3 forward     = Vector3.Cross(right, -transform.right);
                        float angle         = Mathf.Atan2(Vector3.Dot(targetDir.normalized, right), Vector3.Dot(targetDir.normalized, forward)) * Mathf.Rad2Deg;

                        anim.SetFloat("Angle", angle);
                        return;
                    }
                }
            }
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

        public override void Aggro()
        {

            if (enemySM.currentState != EnemyWaitingState.Instance)
                return;

            enemySM.ChangeState(EnemyPursuingState.Instance);

        }

        public void setSpeed(float newSpeed){
            agent.speed = newSpeed;
        }

        public void addPatrolPoint(GameObject newPoint)
        {
            patrolPoints.Add(newPoint);
        }

        public void removePatrolPoint(int i)
        {
            if(i >= 0 && i < patrolPoints.Count)
                patrolPoints.Remove(patrolPoints[i]);
        }
        public void removePatrolPoint(){
            removePatrolPoint(0);
        }

        public virtual bool canShoot(){
            
            if(clipSize > 0 && bullets > 0){ //can reload and have bullets
                bullets--;
                return true;
            }
            else if(clipSize > 0 && bullets <= 0){ //can reload and haven't bullets; start reloading
                
                //reloading
                reloadingProcess += Time.deltaTime;
                if(reloadingProcess >= reloadTime){
                    
                    bullets = clipSize;
                    reloadingProcess = 0.0f;
                    
                    return true;
                }

                return false;
            }
            else{ // can't reload; skip and shoot
                return true;
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
