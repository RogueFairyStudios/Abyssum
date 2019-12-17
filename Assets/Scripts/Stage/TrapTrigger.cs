using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] traps;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trap triggered.");
        foreach(var trap in traps)
            trap.GetComponentInChildren<ITrappable>()?.ActivateTrap();

        Destroy(this.gameObject);
    }
}
