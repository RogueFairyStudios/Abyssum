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
    {

        [SerializeField] private float detectRange = 40.0f;

        [SerializeField] private WeaponBase weapon;
        [SerializeField] private float attackRange = 10;

        public bool search{get;set;}

        public NavMeshAgent agent;
        protected Animator anim;

        public GameObject target;

        protected Vector3 LastTargetLocation; //location to search if the target has been missed
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

            agent.SetDestination(LastTargetLocation);
            anim.SetBool("Walk", true);

        }

        public virtual void Shooting() {

            anim.SetBool("Walk", false);
            getAim();
            if (weapon != null) {
                bool attacked = weapon.Shot();
                if(attacked)
                    anim.SetBool("Attack", true);
            }
            
        }

        public void getAim(){
            var pos = (target.transform.position-transform.position).normalized;
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

            LastTargetLocation = target.transform.position;
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
            
            return (Vector3.Distance(transform.position, LastTargetLocation) < agent.radius * 1.5f);

        }

        //
        public void Hitted() {
            
            if(enemySM.currentState != EnemyWaitingState.Instance)
                return;

            LastTargetLocation = target.transform.position;
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

            Gizmos.DrawLine(target.transform.position, transform.position + Vector3.up * (agent.baseOffset + (agent.height * 0.4f)));

        }

    # endif

    }
}
