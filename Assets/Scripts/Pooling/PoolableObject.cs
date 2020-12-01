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
            if(origin != null)
                origin.Return(this.gameObject);
            else // !FIX
                Debug.LogWarning("FIXME - DEEP.Pooling.PoolableObject.Despawn: Origin not found! (" + transform.name + ")");

        }

    }

}
