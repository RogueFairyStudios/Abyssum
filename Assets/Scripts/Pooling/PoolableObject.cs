using UnityEngine;

namespace DEEP.Pooling {

    public class PoolableObject : MonoBehaviour
    {
    
        protected ObjectPool origin;

        public void SetOrigin(ObjectPool origin) {

            this.origin = origin;

        }

        public void Despawn() {

            this.gameObject.SetActive(false);
            origin.Return(this.gameObject);

        }

    }

}
