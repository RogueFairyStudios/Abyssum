using UnityEngine;

namespace DEEP.Weapons
{

    // Acts as a source of ammo for one or more weapons.
    [CreateAssetMenu(fileName = "newAmmoSource", menuName = "ScriptableObjects/Ammo Source", order = 1)]
    public class AmmoSource : ScriptableObject
    {

        [Tooltip("The id used to refer to this type of ammo.")]        
        public string id = "Bullet";

         [Tooltip("Current amount of ammo on this source.")]     
        public int ammo = 0;

         [Tooltip("Max amount of ammo this source can store.")]     
        public int maxAmmo = 200;

        // Verifies if the source has a certain amount of ammo.
        public bool HasAmmo(int amount)
        {

            if(ammo >= amount) return true;
            else return false;

        }

        // Spends a certain amount of ammo.
        public void UseAmmo(int amount)
        {

            ammo -= amount;
            if(ammo < 0) ammo = 0; // Guarantees the source doesn't goes below 0.

        }

        // Receives a certain amount of ammo.
        public void GainAmmo(int amount)
        {

            ammo += amount;
            if(ammo > maxAmmo) ammo = maxAmmo; // Guarantees the source doesn't goes above max.

        }

    }
}