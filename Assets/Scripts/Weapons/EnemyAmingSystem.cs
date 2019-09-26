using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DEEP.Weapons;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAmingSystem : MonoBehaviour
{
    [SerializeField]protected float radius; //search radius
    [SerializeField]private WeaponBase weapon;
    protected float Waiting_time;
    protected float time;
    protected GameObject target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        weapon = GetComponentInChildren<WeaponBase>();

    }

    void Update()
    {
        Shooting();
    }

    public void Shooting(){
        getAim();
        if (weapon != null)
            weapon.Shot();
    }

    public void Pursuing(){
        getAim();
        if (weapon != null)
            weapon.Shot();
    }

    public void waiting(){
        if (Vector3.Distance(transform.position, target.transform.position) <= radius)
        {
            
        }
    }

    public void getAim(){

        var pos = target.transform.position -transform.position;
        pos.y = 0;// prevents the y axis rotation
        var rotate = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
    }
}
