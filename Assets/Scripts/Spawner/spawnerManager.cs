using System.Collections.Generic;
using UnityEngine;

namespace DEEP.Spawn
{
    public class SpawnerManager : MonoBehaviour
    {

        [SerializeField] private List<Spawner> spawnerList = new List<Spawner>();
    
        public void CreateWave(int spawnerId, int enemyId, int enemyAmount){
            if (spawnerId >= 0  && spawnerId< spawnerList.Count)
                spawnerList[spawnerId].SpawnMultiple(enemyId, enemyAmount);
        }

        public void CreateBigWave(int enemyId, int enemyAmount){
            for (int i = 0; i < spawnerList.Count; i++)
            {
                CreateWave(i, enemyId, enemyAmount);
            }
        }
    }
}