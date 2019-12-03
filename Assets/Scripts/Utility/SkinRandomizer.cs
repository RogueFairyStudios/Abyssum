using UnityEngine;

namespace DEEP.Utilty
{

    // Picks a random skin from an array for an object.
    public class SkinRandomizer : MonoBehaviour
    {

        [SerializeField] Material[] randomMaterials = new Material[0];

        void Start()
        {
            
            SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            renderer.material = randomMaterials[Random.Range(0,randomMaterials.Length)];

        }

    }
}
