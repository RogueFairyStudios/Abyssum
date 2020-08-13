using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public class HealingCollectible : CollectibleBase
    {
        [SerializeField] private HealType hType = HealType.Regular;
        [SerializeField] private int heal = 50;

        protected override void Collect() {

            // Tries healing the player.
            if(Player.Instance.Heal(heal, hType, collectionSound))    
                base.Collect();

        }

    }
}