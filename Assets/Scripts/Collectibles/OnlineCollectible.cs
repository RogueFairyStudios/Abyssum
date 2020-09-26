using Mirror;

using UnityEngine;
using UnityEngine.Events;

using DEEP.Collectibles;

namespace DEEP.Online.Collectibles
{

    public class OnlineCollectible : NetworkBehaviour
    {

        protected CollectibleBase collectible;

        [System.Serializable]
        public class OnCollectEvent : UnityEvent<GameObject>{}

        // Events to be called when the item is collected.
        public OnCollectEvent OnCollectOnline;

        protected void Start() {

            // Gets the regular Collectible this Online version uses as a base.
            collectible = GetComponent<CollectibleBase>();

            // Subscribes the default online events to OnCollectOnline.
            OnCollectOnline.AddListener(DeleteCollectibleServer);

            // If not in server disables the regular Collectible so it's only collected on the server.
            if(!isServer)
                collectible.enabled = false;

        }

        [Server]
        protected void OnTriggerEnter(Collider other)
        {

            // If in server, checks for the Player and calls for OnCollect function.
            if (isServer && other.tag == "Player" && OnCollectOnline != null) {
                if(collectible.Collect(other.gameObject)) // Tries to collect the item.
                    OnCollectOnline.Invoke(other.gameObject); // Calls the collection events if collected.
            }

        }

        [Server]
        protected void DeleteCollectibleServer(GameObject collector) { NetworkServer.Destroy(gameObject); }

    }
}