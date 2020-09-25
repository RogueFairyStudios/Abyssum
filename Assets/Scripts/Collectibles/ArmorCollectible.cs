using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public class ArmorCollectible : CollectibleBase
    {

        [SerializeField] private int amount = 50;

        protected override void Collect(GameObject player) {

            // Tries giving armor to the player.
            if(player.GetComponent<PlayerEntity>().GiveArmor(amount, collectionSound))      
                base.Collect(player);

        }

    }
}