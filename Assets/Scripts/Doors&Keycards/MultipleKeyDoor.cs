using UnityEngine;

using DEEP.Stage;

using System.Collections.Generic;
using System;

namespace DEEP.DoorsAndKeycards
{
	[RequireComponent(typeof(Animator))]	
	public class MultipleKeyDoor : DoorBase, ITrigger
    {
        [SerializeField] private List<KeysColors> requiredKeys;
        [SerializeField] private bool triggerEvent = false;
        [SerializeField] private int numOfKeysForEventTrigger = 0;

        public event Action OnTrigger;

        public override void TryOpenDoor(KeyInventory inventory)
        {
            if (lockState)
				return;

			Debug.Log("Trying to open the door");

			// Dont open the door during an animation.
			if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Opening") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Closing"))
				return;
			
            // Sets each key animation (if there is one)
            int keyCounter = 0;
			foreach(var key in requiredKeys)
            {
                if(inventory.HasKey(key))
                {
                    _animator.SetBool(key.ToString(), true);
                    keyCounter++;
                }
            }

            if(triggerEvent && keyCounter == numOfKeysForEventTrigger)
                OnTrigger();

            if(keyCounter >= requiredKeys.Count)
                OpenDoor();
        }
	}
}