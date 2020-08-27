using System.Collections.Generic;

using UnityEngine;

namespace DEEP.Pooling {

    public class PoolingSystem : MonoBehaviour
    {

        public static PoolingSystem Instance;

        [SerializeField] List<PrefabWarmup> prefabWarmups = new List<PrefabWarmup>();

        Dictionary<GameObject, ObjectPool> pools;

        public void Awake() {

            Instance = this;

            pools = new Dictionary<GameObject, ObjectPool>();

            foreach(PrefabWarmup warmup in prefabWarmups)
                pools.Add(warmup.prefab, new ObjectPool(warmup.prefab, warmup.amount));

        }

        public GameObject PoolObject(GameObject prefab, Vector3 position, Quaternion rotation) {

            if(!pools.ContainsKey(prefab))
                pools.Add(prefab, new ObjectPool(prefab));

            GameObject instance = pools[prefab].Get();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.SetActive(true);

            return instance;

        }

    }

}
