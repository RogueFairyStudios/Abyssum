using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.Stage
{

    public class TrapTrigger : MonoBehaviour
    {
        [SerializeField] GameObject[] traps = new GameObject[0];
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trap triggered.");
            foreach(var trap in traps)
                trap.GetComponentInChildren<ITrappable>()?.ActivateTrap();

            Destroy(this.gameObject);
        }
    }
}