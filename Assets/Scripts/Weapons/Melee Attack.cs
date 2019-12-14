using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : weaponBase
{
    [Tooltip("Amount of time to wait between two consecutive shots.")]
    [SerializeField] protected float delayBetweenShots = 0.3f;
    private float delayTimer = 0; //Used to count the time between shots.
    [Tooltip("List of targets that can be attacked")]
    public List<GameObject> targets;

    protected virtual start(){
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
        public override void Shot()
        {

            // Verifies if the weapon can be fired.
            if(delayTimer >= delayBetweenShots)
                base.Shot();

        }

        // Fires the weapon.
        protected override void Fire(){
            
        }

        protected void OnTriggerEnter(Collider col){
            GameObject inComing = col.gameObject;
            
        }
}
