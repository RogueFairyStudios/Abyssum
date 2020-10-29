using UnityEngine;

using DEEP.Entities;

namespace DEEP.Stage
{

    public class TrapTrigger : MonoBehaviour
    {
        [SerializeField] GameObject[] traps = new GameObject[0];


// Used for the Gizmo.
#if UNITY_EDITOR
        private BoxCollider box;
        private void Start() { box = GetComponent<BoxCollider>(); }
#endif
        private void OnTriggerEnter(Collider other)
        {

            if (other.tag != "Player")
                return;

            Debug.Log("Trap triggered.");
            foreach (var trap in traps)
                trap.GetComponentInChildren<ITrappable>()?.ActivateTrap();

            Destroy(this.gameObject);

            
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() // To visualize the ground check
        {
            Gizmos.color = Color.red;
            if (box != null)
                Gizmos.DrawWireCube(transform.position, box.size * transform.localScale.magnitude);
        }
#endif

    }
}