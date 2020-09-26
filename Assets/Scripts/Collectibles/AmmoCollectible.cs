using UnityEngine;

using DEEP.Weapons;

namespace DEEP.Collectibles
{
    public class AmmoCollectible : CollectibleBase
    {
        [SerializeField] private string type = "Shell";
        [SerializeField] private int amount = 10;

        public override bool Collect(GameObject player) {

            // Tries giving ammo to the player.
            return player.GetComponent<PlayerWeaponController>().GiveAmmo(amount, type, collectionSound);

        }

    }
}