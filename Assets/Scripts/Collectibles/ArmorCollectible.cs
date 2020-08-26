using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public class ArmorCollectible : CollectibleBase
    {

        [SerializeField] private int amount = 50;

        protected override void Collect() {

            // Tries giving armor to the player.
            if(PlayerController.Instance.entity.GiveArmor(amount, collectionSound))      
                base.Collect();

        }

    }
}