using UnityEngine;

namespace DEEP.Utility
{

    // Always faces the main camera.
    public class Billboard : MonoBehaviour
    {

        [Tooltip("Rotation offset to add to the billboard.")]
        [SerializeField] protected Vector3 rotationOffset = Vector3.zero;

        void Update()
        {

            transform.LookAt(Camera.main.transform);
            transform.rotation *= Quaternion.Euler(rotationOffset);

        }

    }

}
