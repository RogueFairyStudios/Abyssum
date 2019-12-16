using System.Runtime.CompilerServices;
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
    [SerializeField]public bool search{get;set;}
    protected GameObject target;
    protected  NavMeshAgent agent;
    protected Vector3 LastTargetLocation; //location to search if the target has been missed
    protected StateMachine<EnemyAISystem> enemySM;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        weapon = GetComponentInChildren<WeaponBase>();
        search = false;
        enemySM = new StateMachine<EnemyAISystem>(this);
        enemySM.ChangeState(EnemyWaitingState.Instance);//first state
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        enemySM.update();//update the actual state
    }

    public void Shooting(){
        getAim();
        if (weapon != null)
            weapon.Shot();
    }

    public void waiting(){
        if (inRange())
        {
            search = true;
            enemySM.ChangeState(EnemyShootingState.Instance);//target finded, stating gun fight
        }
        else{
            //randon movementation
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

    public void Pursuing(){
        agent.SetDestination(LastTargetLocation);
    }

    public void ChangeState(State<EnemyAISystem> newState){
        enemySM.ChangeState(newState);
    }

    //
    public void hitted(){LastTargetLocation = target.transform.position;}
}
