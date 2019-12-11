using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public class HealingCollectible : CollectibleBase
    {
        [SerializeField] private HealType hType = HealType.Regular;
        [SerializeField] private int heal = 50;

        protected override void Collect(Player player) {

            // Tries healing the player and stores if it was healed.
            bool collected = player.Heal(heal, hType, collectionSound);
            
            if(collected) {
                
                // Destroys the object if the collectible is used.
                Destroy(gameObject);

            }

        }

    }
}