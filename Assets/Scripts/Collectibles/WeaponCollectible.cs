using UnityEngine;
using DEEP.Entities;

namespace DEEP.Collectibles
{

    public class WeaponCollectible : CollectibleBase
    {
        
        // The slot that contains the weapon to be given. 
        [SerializeField] private int weaponSlot = 0;

        // Ammo to be given. 
        [SerializeField] private int ammoAmount = 5;

        protected override void Collect(Player player) {

            // Tries giving the weapon to the player and stores if it was given.
            bool collected = player._weaponController.GiveWeapon(weaponSlot - 1, ammoAmount, collectionSound);

            if(collected) {
                
                // Destroys the object if the collectible is used.
                Destroy(gameObject);

            }
            
        }
        
    }

}
