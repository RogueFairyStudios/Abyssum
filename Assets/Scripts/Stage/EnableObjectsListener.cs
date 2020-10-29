using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Stage;

public class EnableObjectsListener : MonoBehaviour
{
    [SerializeField] GameObject[] itemsToEnable;

    [Tooltip("Script with ITrigger interface")]
    [SerializeField] Object trigger;
 
    private void OnEnable()
    {
        ((ITrigger)trigger).OnTrigger += EnableObjects;
    }

    void EnableObjects()
    {
        foreach(var gameObject in itemsToEnable)
            gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        ((ITrigger)trigger).OnTrigger -= EnableObjects;
    }
}
