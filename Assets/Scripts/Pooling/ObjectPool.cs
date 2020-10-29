using System.Collections.Generic;

using UnityEngine;

namespace DEEP.Pooling {

    /// <summary>Pool of a certain prefab, used by the PoolingSystem to manage it's
    /// prefab pools.</summary>
    public class ObjectPool {

        /// <summary>Total number of instances of this pool's prefab,
        /// considers both prefabs on the avaliable Stack and the ones
        /// that are currently active.</summary>
        private int totalInstances;

        /// <summary>Stores the prefab that was given on the constructor,
        /// and will be used to create this pool's instances.</summary>
        private GameObject prefab;
        
        /// <summary>Transform of an object created to store this pool's
        /// instances.</summary>
        private Transform parent;

        /// <summary>Stack containing currently avaliable prefab instances.</summary>
        private Stack<GameObject> avaliable;

        /// <summary>Default constructor for the class,
        /// initializes an empty pool of the given prefab.
        ///    the given coordinates.</summary>
        /// <param name="prefab">Prefab to be used by the pool.</param>
        public ObjectPool(GameObject prefab) {

            this.totalInstances = 0;
            this.prefab = prefab;
            avaliable = new Stack<GameObject>();

            CreateParent();

        }

        /// <summary>This constructor creates a pool of the given
        /// prefab and initializes a certain number of instances.</summary>
        /// <param name="prefab">Prefab to be used by the pool.</param>
        /// <param startingInstances="prefab">Number of instances to be initially available.</param>
        public ObjectPool(GameObject prefab, int startingInstances) {

            this.totalInstances = 0;
            this.prefab = prefab;
            avaliable = new Stack<GameObject>();

            CreateParent();

            for(int i = 0; i < startingInstances; i++)
                CreateNewInstance();

        }

        /// <summary>Creates the parent object that will be used to stores
        /// the prefab instances. Mainly to make the Inspector on the
        /// UnityEditor cleaner.</summary>
        private void CreateParent() {

            parent = new GameObject().transform;
            parent.name = "Pool_" + prefab.name;
            parent.SetParent(PoolingSystem.Instance.transform);

        }

        /// <summary>Creates a new instance of this pools prefab.</summary>
        private void CreateNewInstance() {

            GameObject newInstance = GameObject.Instantiate(prefab);
            
            PoolableObject obj = newInstance.GetComponent<PoolableObject>();
            if(obj == null)
                obj = newInstance.AddComponent(typeof(PoolableObject)) as PoolableObject;
            obj.SetOrigin(this);

            newInstance.transform.SetParent(parent);
            newInstance.SetActive(false);
            avaliable.Push(newInstance);
            totalInstances++;

        }

        /// <summary>Gets an instance of this pool's prefab. Will first
        /// check for currently avaliable instances. If there's none, a
        /// new one will be created.</summary>
        /// <returns>A prefab instance.</returns>
        public GameObject Get() {

            if(avaliable.Count < 1)
                CreateNewInstance();

            return avaliable.Pop();

        }

        /// <summary>Used to return an instance to the pool.</summary>
        public void Return(GameObject instance) {
            avaliable.Push(instance);
        }

    }

}