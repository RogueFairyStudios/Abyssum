using Mirror;

using UnityEngine;

using DEEP.Collectibles;

namespace DEEP.Online.Collectibles
{

    public class OnlineCollectible : NetworkBehaviour
    {

        protected CollectibleBase collectible;

        protected void Start() {

            // Gets the regular Collectible this Online version uses as a base.
            collectible = GetComponent<CollectibleBase>();

            // If not in server disables the regular Collectible so it's only collected on the server.
            if(!isServer)
                collectible.enabled = false;

        }

        protected void OnTriggerEnter(Collider other)
        {

            // If in server, checks for the Player.
            if (isServer && other.tag == "Player") {

                // Tries to collect the item.
                if(collectible.Collect(other.gameObject)) { 

                    NetworkServer.Destroy(gameObject);

                }
                    
            }

        }
    }
}