using UnityEngine;

using DEEP.AI;

namespace DEEP.Entities{

    public class Blowfish : Enemy
    {
        [SerializeField] GameObject explosionPrefab;

        // Will be a loop instead
        protected override void Growl()
        {
            
        }

        public void Explode()
        {
            Transform currentPosition = this.transform;
            Destroy(this.gameObject);
            Instantiate(explosionPrefab, currentPosition.position, currentPosition.rotation);
        }
    }
}
