using UnityEngine;

using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public class HealingCollectible : CollectibleBase
    {

        [Tooltip("Type of the heal: Regular is limited by EntityBase.maxHealth, meanwhile Overload is limited by EntityBase.maxOverloadedHealth.")]
        [SerializeField] private HealType hType = HealType.Regular;

        [Tooltip("Health to be given.")]
        [SerializeField] private int heal = 50;

        public override bool Collect(GameObject player) {

            // Tries healing the player.
            return player.GetComponent<PlayerEntity>().Heal(heal, hType, collectionSound);

        }

    }
}