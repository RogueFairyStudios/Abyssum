using UnityEngine;
using UnityEngine.AI;
using DEEP.Weapons;
using DEEP.StateMachine;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAISystem : MonoBehaviour
{
    [SerializeField]protected float radius; //search radius
    [SerializeField]private WeaponBase weapon;
    [SerializeField]public bool search{get;set;}
    protected GameObject target;
    protected  NavMeshAgent agent;
    protected Vector3 LastTagetLocation; //location to search if the target has been missed
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
        Vector3 targetDirection = target.transform.position -transform.position;
        Ray searchRay = new Ray(transform.position, targetDirection);
        RaycastHit hit;
        Debug.DrawLine(transform.position, radius * targetDirection);

        if(Physics.Raycast(searchRay, out hit, radius)){
            if (hit.transform.gameObject == target)//verify if is something between the target and the enemy
            {
                LastTagetLocation = target.transform.position;
                return true;
            }
        }
        return false;
    }

    public bool inRange(){
        if (RayCastHitTarget() && (Vector3.Distance(transform.position, target.transform.position) <= radius))
        {
            return true;
        }
        return false;
    }

    public void Pursuing(){
        agent.SetDestination(LastTagetLocation);
    }

    public void ChangeState(State<EnemyAISystem> newState){
        enemySM.ChangeState(newState);
    }
}
