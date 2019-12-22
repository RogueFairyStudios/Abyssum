using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using DEEP.Weapons;
using DEEP.StateMachine;
using DEEP.Entities;


namespace DEEP.AI
    {

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAISystem : MonoBehaviour
    {public float temp = 0;

        [SerializeField] private float detectRange = 40.0f;

        [SerializeField] private WeaponBase weapon;
        [SerializeField] private float attackRange = 10;

        public bool search{get;set;}

        public NavMeshAgent agent;
        protected Animator anim;

        public GameObject target;

        public Vector3 LastTargetLocation; //location to search if the target has been missed
        protected StateMachine<EnemyAISystem> enemySM;
        [Tooltip("random movimentation settings")]
        [SerializeField] private List<GameObject> patrolPoints = new List<GameObject>();
        [SerializeField] private int actualPoint =0;

        public delegate void Reaction();
        public Reaction OnAggro, OnLoseAggro;

        [SerializeField] protected LayerMask sightMask = new LayerMask();

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponentInChildren<Animator>();
            target = GameObject.FindGameObjectWithTag("Player");
            weapon = GetComponentInChildren<WeaponBase>();
            search = false;
            enemySM = new StateMachine<EnemyAISystem>(this);
            enemySM.ChangeState(EnemyWaitingState.Instance);//first state

            // Setups delegates.
            OnAggro += AlertAllies;
            
        }

        void Update()
        {
            enemySM.update();//update the actual state

        }

        public virtual void Waiting() {

            if (HasSight(target.transform.position)) {

                search = true;
                enemySM.ChangeState(EnemyPursuingState.Instance);//target finded, engaging
                return;

            } 
            
            if(patrolPoints.Count>0) {
                //randon movementation
                //is in the patrol point
                if (!agent.pathPending && agent.remainingDistance < 0.5f){
                    agent.SetDestination(patrolPoints[actualPoint].transform.position);
                    actualPoint++;
                    actualPoint = (actualPoint)%patrolPoints.Count;
                }

                anim.SetBool("Walk", true);
                return;

            }

            anim.SetBool("Walk", false);

        }

        public virtual void Pursuing() {

            if(HasSight(target.transform.position))
                GoToTarget();

            anim.SetBool("Walk", true);

        }

        public virtual void Shooting() {

            if(HasSight(target.transform.position))
                LastTargetLocation = target.transform.position;

            anim.SetBool("Walk", false);
            getAim();
            if (weapon != null) {
                bool attacked = weapon.Shot();
                if(attacked)
                    anim.SetBool("Attack", true);
                else
                    anim.SetBool("Attack", false);
            }
            
        }

        public void GoToTarget() {

            LastTargetLocation = target.transform.position;
            agent.SetDestination(LastTargetLocation);

        }

        public void getAim(){

            var pos = (LastTargetLocation - transform.position).normalized;
            /*if(pos.y != 0){
                //arms movimentation
            }*/
            var rotate = Quaternion.LookRotation(pos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
        }

        public bool HasSight(Vector3 point) {

            if (Physics.Linecast(point, transform.position + Vector3.up * (agent.baseOffset + (agent.height * 0.4f)), sightMask))
                return false;

            if (Vector3.Distance(point, transform.position + Vector3.up * (agent.baseOffset + (agent.height * 0.4f))) > detectRange)
                return false;

            return true;
        }
        
        public bool InAttackRange() {

            return (HasSight(target.transform.position) && (Vector3.Distance(transform.position, target.transform.position) <= attackRange));

        }

        public bool OutAttackRange() { return !InAttackRange(); }

        public virtual void ChangeState(State<EnemyAISystem> newState) {

            enemySM.ChangeState(newState);

        }

        public bool ReachedLastPosition() {
            
            temp = Vector3.Distance(transform.position, LastTargetLocation);
            return (Vector3.Distance(transform.position, LastTargetLocation) < agent.radius * 2.5f);

        }

        public void Hitted() {
            
            if(enemySM.currentState != EnemyWaitingState.Instance)
                return;

            enemySM.ChangeState(EnemyPursuingState.Instance);
            
        }

        // Alerts close allies.
        public void AlertAllies() {

            EnemyAISystem[] allies = FindObjectsOfType<EnemyAISystem>();

            foreach (EnemyAISystem ally in allies)
            {

                if(HasSight(ally.transform.position + Vector3.up * (agent.baseOffset + (agent.height * 0.4f))))
                    ally.Hitted();

            }

        }

    # if UNITY_EDITOR

        void OnDrawGizmos() {

            if(target == null) 
                return;

            float distance = Vector3.Distance(transform.position, target.transform.position);

            if(HasSight(target.transform.position)) {

                Gizmos.color = Color.blue;

                if(distance < attackRange)
                    Gizmos.color = Color.red;

                
            } else
                Gizmos.color = Color.white;

            Gizmos.DrawLine(LastTargetLocation, transform.position + Vector3.up * (agent.baseOffset + (agent.height * 0.4f)));

        }

    # endif

    }
}
