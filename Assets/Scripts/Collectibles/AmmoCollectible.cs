using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public class AmmoCollectible : CollectibleBase
    {
        [SerializeField] private string type = "Shell";
        [SerializeField] private int amount = 10;

        protected override void Collect(Player player) {

            // Tries healing the player and stores if it was healed.
            bool collected = player._weaponController.GiveAmmo(amount, type, collectionSound);
            
            if(collected) {
                
                // Destroys the object if the collectible is used.
                Destroy(gameObject);

            }

        }

    }
}