using UnityEngine;

using DEEP.Weapons;

namespace DEEP.Collectibles
{

    public class WeaponCollectible : CollectibleBase
    {
        
        [Tooltip("The slot that contains the weapon to be given.")]
        [SerializeField] private int weaponSlot = 0;

        [Tooltip("Ammo to be given.")]
        [SerializeField] private int ammoAmount = 5;

        protected override void Collect(GameObject player) {

            // Tries giving the weapon to the player.
            if(player.GetComponent<PlayerWeaponController>().GiveWeapon(weaponSlot - 1, ammoAmount, collectionSound))
                base.Collect(player);
            
        }
        
    }

}
