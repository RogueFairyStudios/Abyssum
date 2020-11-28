using System.Collections.Generic;

using UnityEngine;

namespace DEEP.Decals {

    public class DecalSystem : MonoBehaviour
    {
        
        public static DecalSystem Instance;

        [SerializeField] protected GameObject baseCube = null;
        [SerializeField] protected int decalAmount = 128;
        
        protected List<Renderer> decalPool;
        protected int currentDecal;

        void Start() {

            // Ensures theres only one instance of this script by destroying the older one if necessary.
            if(Instance != null)
                Destroy(Instance.gameObject);

            Instance = this;

            decalPool = new List<Renderer>();
            for(int i = 0; i < decalAmount; i++) {
                Renderer decal = Instantiate(baseCube, transform.position, transform.rotation).GetComponent<Renderer>();
                decal.transform.SetParent(this.transform);
                decal.gameObject.SetActive(false);
                decalPool.Add(decal);
            }
            currentDecal = 0;

        }

        // Places a decal with the assigned material somewhere in the world.
        public Renderer PlaceDecal(Material material, Vector3 position, Vector3 normal, Vector2 scale) {

            // Gest decal from pool.
            Renderer decal = decalPool[currentDecal];

            // Sets the decal material.
            decal.material = material;

            // Clear any decal parent.
            decal.transform.SetParent(null); 

            // Sets the decal transform.
            decal.transform.position = position;
            decal.transform.LookAt(position - normal);
            decal.transform.localRotation *= Quaternion.Euler(0, 0, Random.Range(0, 180));
            decal.transform.localScale = new Vector3(scale.x, scale.y, 0.01f);

            // Enables decal.
            decal.gameObject.SetActive(true);

            // Updates the decal pool.
            currentDecal = (currentDecal + 1) % decalPool.Count;

            return decal;

        }

         // Places a decal with the assigned material somewhere in the world and assigns it's parent.
        public Renderer PlaceDecal(Material material, Transform parent, Vector3 position, Vector3 normal, Vector2 scale) {

            // Places a decal and them sets it's parent.
            Renderer decal = PlaceDecal(material, position, normal, scale);
            decal.transform.SetParent(parent);
            return decal;

        }

        void OnDestroy() {

            foreach(Renderer decal in decalPool) {
                if(decal != null)
                    Destroy(decal.gameObject);
            }

        }

    }

}