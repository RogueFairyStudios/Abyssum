using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DEEP.Entities;

namespace DEEP.Weapons{
    public class MeleeAttack : WeaponBase
    {
        [Tooltip("Amount of time to wait between two consecutive shots.")]
        [SerializeField] protected float delayBetweenShots = 0.3f;
        private float delayTimer = 0; //Used to count the time between shots.
        [Tooltip("List of targets that can be attacked")]
        public List<GameObject> targets;
        [Tooltip("Damage inflicted by the attack.")]
        public int damage;
        public float knockbackForce; // when used by enemy multiply by the player mass
        [Tooltip("this is the owner of the attack")]
        public GameObject Attacker;

        protected virtual void start(){
            // Allows the weapon to be fired at start.
            delayTimer = delayBetweenShots;

        }


        protected virtual void Update()
        {

            // Waits for the delay between shots.
            if(delayTimer < delayBetweenShots)
                delayTimer += Time.deltaTime;

        }

        // Attempts to fire the weapon.
        public override bool Shot()
        {
            // Verifies if the weapon can be fired.
            if(delayTimer >= delayBetweenShots)
                return base.Shot();

            return false;

        }

        // Attacks all the targets
        protected override void Fire(){
                
                delayTimer = 0;
                Vector3 dir;//knockback direction

                for(int i=0; i<targets.Count; i++){
                    //target i knockback
                    dir = targets[i].transform.position -  Attacker.transform.position;
                    dir.y = 0.1f;
                    targets[i].GetComponent<Rigidbody>().AddForce(dir.normalized * knockbackForce);
                    if(targets[i].GetComponent(typeof(EntityBase)) != null){
                        EntityBase entity = targets[i].GetComponent<EntityBase>();
                        entity.Damage(damage,0);//applying the damage
                    }
                }
        }

        protected void OnTriggerEnter(Collider col){
            GameObject inComing = col.gameObject;
                
            //the gameObject can be punched ?
            if(inComing.GetComponent(typeof(Rigidbody)) != null){
                if(!targets.Contains(inComing))
                    targets.Add(inComing);
            }
        }

        protected void OnTriggerExit(Collider col){
            GameObject exited = col.gameObject;
            if(targets.Contains(exited))
                targets.Remove(exited);
            
        }

        protected override void NoAmmo(){}

    }
}