using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public class ArmorCollectible : CollectibleBase
    {

        [SerializeField] private int amount = 50;

        protected override void Collect(Player player) {

            // Tries giving armor to the player and stores if the item was collected.
            bool collected = player.GiveArmor(amount, collectionSound);
            
            if(collected) {
                
                // Destroys the object if the collectible is used.
                Destroy(gameObject);

            }

        }

    }
}