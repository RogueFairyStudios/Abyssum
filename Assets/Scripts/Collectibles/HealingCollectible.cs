using UnityEngine;

using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public class HealingCollectible : CollectibleBase
    {
        [SerializeField] private HealType hType = HealType.Regular;
        [SerializeField] private int heal = 50;

        protected override void Collect(GameObject player) {

            // Tries healing the player.
            if(player.GetComponent<PlayerEntity>().Heal(heal, hType, collectionSound))    
                base.Collect(player);

        }

    }
}