using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.Spawn
{
    public class spawnerManager : MonoBehaviour
    {
    [SerializeField] private List<spawner> spawnerList = new List<spawner>();
    
        public void createWave(int spawnerId, int enemyId, int enemyAmount){
            if (spawnerId >= 0  && spawnerId< spawnerList.Count)
                spawnerList.[spawnerId].spawnNObject(enemyId, enemyAmount);
        }

        public void createBigWave(int enemyId, int enemyAmount){
            for (int i = 0; i < spawnerList.Count; i++)
            {
                createWave(i, enemyId, enemyAmount);
            }
        }
    }
}