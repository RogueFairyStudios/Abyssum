using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public abstract class CollectibleBase : MonoBehaviour
    {

        [SerializeField] protected AudioClip collectionSound = null;

        protected virtual void OnTriggerEnter(Collider col)
        {

            Player player = col.GetComponent<Player>();
            if (player != null)
            {
                
                Collect(player);

            }          

        }

        protected abstract void Collect(Player player);
    }
}