using System.Collections;
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

        public void PlaceDecal(Material material, Vector3 position, Vector3 normal, Vector2 scale) {

            Renderer decal = decalPool[currentDecal];

            decal.material = material;
            decal.transform.position = position;
            decal.transform.LookAt(position - normal);
            decal.transform.localRotation *= Quaternion.Euler(0, 0, Random.Range(0, 180));
            decal.transform.localScale = new Vector3(scale.x, scale.y, 0.01f);
            decal.gameObject.SetActive(true);

            currentDecal = (currentDecal + 1) % decalPool.Count;

        }

        void OnDestroy() {

            foreach(Renderer decal in decalPool) {
                if(decal != null)
                    Destroy(decal.gameObject);
            }

        }

    }

}