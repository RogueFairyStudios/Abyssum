using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> entitieList = new List<GameObject>();
    public int i = 0;
    float time = 2;
    // Start is called before the first frame update
    void Start()
    {
        spawnObjects();
    }

    void Update(){
        time -= Time.deltaTime;
        if(time <= 0){
            spawnObject(i);
        
            i = (i+1)% entitieList.Count;
            time = 2;
        }

    }

    public void spawnObject(int id){

        if (id < 0 || id > entitieList.Count)
        {
            Debug.Log("Invalid Id");
            return;
        }

        Instantiate(entitieList[id],this.transform);
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
