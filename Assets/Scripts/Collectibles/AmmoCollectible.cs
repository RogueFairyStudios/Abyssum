using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Collectibles
{
    public class AmmoCollectible : CollectibleBase
    {
        [SerializeField] private string type = "Shell";
        [SerializeField] private int amount = 10;

        protected override void Collect() {

            // Tries giving ammo to the player.
            if(PlayerController.Instance.weaponController.GiveAmmo(amount, type, collectionSound))     
                base.Collect();

        }

    }
}