using UnityEngine;

namespace DEEP.Utility {

    // Retracts the weapon to not go through walls.
    public class WeaponRetract : MonoBehaviour
    {

        Transform cameraTransform;
        Vector3 originalLocalPosition;
        [SerializeField] float safeDistance = 0.5f;
        [SerializeField] LayerMask raycastMask = new LayerMask();

        void Start() {

            cameraTransform = Camera.main.transform;
            originalLocalPosition = transform.localPosition;

        }

        void Update() {

            RaycastHit hit;
            if(Physics.Raycast(cameraTransform.position + transform.TransformDirection(originalLocalPosition), transform.forward, out hit, 2 * safeDistance, raycastMask)) {

                transform.localPosition = originalLocalPosition - transform.InverseTransformDirection(transform.forward * (safeDistance - Mathf.Clamp(hit.distance, 0, safeDistance)));

            } else {

                transform.localPosition = originalLocalPosition;
            
            }

        }

# if UNITY_EDITOR

        void OnDrawGizmos() {

            if(cameraTransform == null) return;

            Gizmos.color = Color.green;
            if(Physics.Raycast(cameraTransform.position + transform.TransformDirection(originalLocalPosition), transform.forward, safeDistance, raycastMask))
                Gizmos.color = Color.red;

            Gizmos.DrawLine(cameraTransform.position + transform.TransformDirection(originalLocalPosition), 
                            cameraTransform.position + transform.TransformDirection(originalLocalPosition) + transform.forward * safeDistance);

        }

# endif

    }
}
