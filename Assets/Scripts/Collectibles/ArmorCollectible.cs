using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public class ArmorCollectible : CollectibleBase
    {

        [Tooltip("Armor to be given.")]
        [SerializeField] private int amount = 50;

        public override bool Collect(GameObject player) {

            // Tries giving armor to the player.
            return player.GetComponent<PlayerEntity>().GiveArmor(amount, collectionSound);     

        }

    }
}