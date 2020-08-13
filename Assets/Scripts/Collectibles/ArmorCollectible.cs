using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{
    public class ArmorCollectible : CollectibleBase
    {

        [SerializeField] private int amount = 50;

        protected override void Collect() {

            // Tries giving armor to the player.
            if(Player.Instance.GiveArmor(amount, collectionSound))      
                base.Collect();

        }

    }
}