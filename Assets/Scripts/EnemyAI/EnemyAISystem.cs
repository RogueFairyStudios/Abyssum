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
    [SerializeField]private float radius; //search radius
    [SerializeField]private WeaponBase weapon;
    public bool search{get;set;}
    protected GameObject target;
    protected  NavMeshAgent agent;
    protected Vector3 LastTargetLocation; //location to search if the target has been missed
    protected StateMachine<EnemyAISystem> enemySM;
    [Tooltip("random movimentation settings")]
    [SerializeField]private List<GameObject> patrolPoints;
    [SerializeField] private int actualPoint =0;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

    public virtual void Shooting(){
        getAim();
        if (weapon != null)
            weapon.Shot();
    }

    public virtual void waiting(){
        if (inRange())
        {
            search = true;
            enemySM.ChangeState(EnemyShootingState.Instance);//target finded, stating gun fight
        }
        else if(patrolPoints.Count>0){
            //randon movementation
            //is in the patrol point
            if (!agent.pathPending && agent.remainingDistance < 0.5f){
                agent.SetDestination(patrolPoints[actualPoint].transform.position);
                actualPoint++;
                actualPoint = (actualPoint)%patrolPoints.Count;
            }

        }
    }

    public void getAim(){
        var pos = (target.transform.position -transform.position).normalized;
        /*if(pos.y != 0){
            //arms movimentation
        }*/
        var rotate = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
    }

    public bool RayCastHitTarget(){
        Vector3 targetDirection = (target.transform.position - (transform.position + Vector3.up)).normalized;
        Ray searchRay = new Ray(transform.position + Vector3.up, targetDirection);
        RaycastHit hit;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + radius * targetDirection);

        if(Physics.Raycast(searchRay, out hit, radius)){
            if (hit.transform.gameObject == target)//verify if is something between the target and the enemy
            {
                return true;
            }
        }
        return false;
    }

    public bool inRange(){
        if (RayCastHitTarget() && (Vector3.Distance(transform.position, target.transform.position) <= radius))
        {
            LastTargetLocation = target.transform.position;
            return true;
        }
        return false;
    }

    public bool outRange(){
        if (RayCastHitTarget() && (Vector3.Distance(transform.position, target.transform.position) >= radius * 2))
        {
            LastTargetLocation = target.transform.position;
            return true;
        }
        return false;
    }

    public virtual void Pursuing(){
        agent.SetDestination(LastTargetLocation);
    }

    public virtual void ChangeState(State<EnemyAISystem> newState){
        enemySM.ChangeState(newState);
    }

    //
    public void hitted(){LastTargetLocation = target.transform.position;}
}
