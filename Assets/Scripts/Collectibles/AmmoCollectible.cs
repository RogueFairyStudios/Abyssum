using UnityEngine;

using DEEP.Weapons;

namespace DEEP.Collectibles
{
    public class AmmoCollectible : CollectibleBase
    {
        [SerializeField] private string type = "Shell";
        [SerializeField] private int amount = 10;

        protected override void Collect(GameObject player) {

            // Tries giving ammo to the player.
            if(player.GetComponent<PlayerWeaponController>().GiveAmmo(amount, type, collectionSound))     
                base.Collect(player);

        }

    }
}