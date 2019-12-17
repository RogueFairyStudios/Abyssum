using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DEEP.Weapons;
using DEEP.StateMachine;
using DEEP.Entities;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAISystem : MonoBehaviour
{
    [SerializeField] private float engageRange = 10;
    [SerializeField] private float disengageRange = 20;
    [SerializeField] private float attackRange = 10;

    [SerializeField] private WeaponBase weapon;

    public bool search{get;set;}

    protected GameObject target;
    protected NavMeshAgent agent;
    protected Animator anim;

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
        
    }

    void Update()
    {
        enemySM.update();//update the actual state

    }

    public virtual void Waiting() {

        if (InRange()) {

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

    public bool HasSight() {

        return !Physics.Linecast(target.transform.position, transform.position + Vector3.up * (agent.baseOffset + (agent.height * 0.4f)), sightMask);
    }

    public bool InRange() {

        if (HasSight() && (Vector3.Distance(transform.position, target.transform.position) <= engageRange))
        {
            LastTargetLocation = target.transform.position;
            return true;
        }
        return false;

    }

    public bool OutRange() {

        if (HasSight() && (Vector3.Distance(transform.position, target.transform.position) <= disengageRange))
        {
            LastTargetLocation = target.transform.position;
            return false;
        }
        return true;

    }
    
    public bool InAttackRange() {

        return (HasSight() && (Vector3.Distance(transform.position, target.transform.position) <= attackRange));

    }

    public bool OutAttackRange() { return !InAttackRange(); }

    public virtual void ChangeState(State<EnemyAISystem> newState) {

        enemySM.ChangeState(newState);

    }

    //
    public void Hitted() {
        
        enemySM.ChangeState(EnemyPursuingState.Instance);
        
    }

# if UNITY_EDITOR

    void OnDrawGizmos() {

        if(target == null) 
            return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if(distance > Mathf.Max(engageRange, Mathf.Min(disengageRange, attackRange)))
            return;

        if(HasSight()) {

            Gizmos.color = Color.blue;

            if(distance < attackRange)
                Gizmos.color = Color.red;
            else if (distance < engageRange)
                Gizmos.color = Color.yellow;

            
        } else
            Gizmos.color = Color.white;

        Gizmos.DrawLine(target.transform.position, transform.position + Vector3.up * (agent.baseOffset + (agent.height * 0.4f)));

    }

# endif

}
