using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DEEP.AI;

public class spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> entitieList = new List<GameObject>();
    public GameObject startingPoint;
    public int i = 0;
    float time = 15f;
    
    //test
    void Start()
    {
       // spawnObjects();
    }

    void Update(){
        time -= Time.deltaTime;
        if(time <= 0){
            spawnObject(i);
            i++;
            i = i% entitieList.Count;
            time = 3f;
        }

    }

    public void spawnObject(int id){

        if (id < 0 || id > entitieList.Count)
        {
            Debug.Log("Invalid Id");
            return;
        }

        GameObject instance = Instantiate(entitieList[id],this.transform);
        instance.GetComponent<EnemyAISystem>().addPatrolPoint(startingPoint);
        
    }

    public void spawnNObject(int id, int n){

        if (id < 0 || id > entitieList.Count)
        {
            Debug.Log("Invalid Id");
            return;
        }

        for (int i = 0; i < n; i++)
        {
            spawnObject(id);    
        }
    }

    public void spawnObjects(){
        for (int i = 0; i < entitieList.Count; i++)
        {
            spawnObject(i);
        }
    }

}
    
