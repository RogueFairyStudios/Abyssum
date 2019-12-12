using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public abstract class CollectibleBase : MonoBehaviour
    {

        // Audio that can be played when a item is collected.
        [SerializeField] protected AudioClip collectionSound = null;

        protected virtual void OnTriggerEnter(Collider col)
        {

            // Checks for a Player component.
            Player player = col.GetComponent<Player>();
            if (player != null)               
                Collect(player); // Calls the collection function.

        }

        protected abstract void Collect(Player player); // Overwrite for each type of collectible.
    }
}