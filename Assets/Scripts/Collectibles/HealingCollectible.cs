using UnityEngine;

using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public class HealingCollectible : CollectibleBase
    {
        [SerializeField] private HealType hType = HealType.Regular;
        [SerializeField] private int heal = 50;

        protected override void Collect() {

            // Tries healing the player.
            if(PlayerController.Instance.entity.Heal(heal, hType, collectionSound))    
                base.Collect();

        }

    }
}