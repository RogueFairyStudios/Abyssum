using UnityEngine;

namespace DEEP.Entities
{

    public class DestructibleObject : EntityBase
    {

        [Tooltip("Object to be spawned when this object is destroyed.")]
        [SerializeField] protected GameObject objectToSpawn = null;

        private bool destroyed;

        protected override void Start() {

            destroyed = false;
            base.Start();

        }

        protected override void Die() {

            if(destroyed)
                return;

            destroyed = true;
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Destroy(gameObject);

        }

    }
}
