using UnityEngine;

namespace DEEP.Weapons {

    // Retracts the weapon to not go through walls.
    public class WeaponRetract : MonoBehaviour
    {

        Transform cameraTransform;
        Vector3 originalLocalPosition;
        [SerializeField] float safeDistance = 0.5f;
        [SerializeField] LayerMask raycastMask = new LayerMask();

        void Start() {

            cameraTransform = Camera.main.transform;

        }

        void Update() {

            // Gets a point far away into the horizon.
            RaycastHit rayHit;
            if (Physics.Linecast(cameraTransform.position + transform.TransformDirection(originalLocalPosition), cameraTransform.position + transform.TransformDirection(originalLocalPosition) + transform.forward * 5.0f, out rayHit, raycastMask, QueryTriggerInteraction.Ignore)) {
                transform.localPosition = originalLocalPosition - transform.InverseTransformDirection(transform.forward * (safeDistance - Mathf.Clamp(rayHit.distance, 0, safeDistance)));
            } else {
                transform.localPosition = originalLocalPosition;
            }

        }

        public void SetOrigin(Vector3 position) { originalLocalPosition = position; }

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
