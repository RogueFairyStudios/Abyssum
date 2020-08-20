using System;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Entities;

namespace DEEP.Weapons {
    public class PlayerWeaponController : MonoBehaviour
    {

        [Tooltip("All of the player weapons.")]
        public List<PlayerWeapon> weapons;

        // Stores the weapons instances with their info.
        List<Tuple<bool, WeaponBase>> weaponInstances;

        [Tooltip("Where Player weapons should be.")]
        public Transform weaponPosition;

        // Stores the current weapon.
        [SerializeField] public WeaponBase currentWeapon;

        [Tooltip("Ammo sources carried by the player.")]
        public List<AmmoSource> ammoTypes;
        // Stores a dictionary with the AmmoSource instances.
        private Dictionary<string, AmmoSource> ammoDict;

         // If this script has been initialzed by the main Player script.
        private bool initialized = false;

        public void Initialize() {

            Debug.Log("Initializing PlayerWeaponController...");
            
            // Creates a dictionary with the ammo sources.
            ammoDict = new Dictionary<string, AmmoSource>();
            foreach(AmmoSource source in ammoTypes)
                if(!ammoDict.ContainsKey(source.id))
                    ammoDict.Add(source.id, Instantiate(source));

            // Weapon setup =============================================================================================

            // Instantiates the weapons.
            weaponInstances = new List<Tuple<bool, WeaponBase>>();
            foreach (PlayerWeapon weapon in weapons)
            {

                // Creates the weapons inside the weapon position.
                GameObject weaponObj = Instantiate(weapon.prefab, weaponPosition.position, weaponPosition.rotation);
                weaponObj.transform.SetParent(weaponPosition);
                // Disables the weapon at start.
                weaponObj.SetActive(false);

                // Gets the weapon script.
                WeaponBase weaponScript = weaponObj.GetComponent<WeaponBase>();
                if(weaponScript == null) Debug.LogError("DEEP.Entities.Player.Start: Weapon has no weapon script!");

                // Sets the ammo source of the weapon.
                if(weapon.ammoId != null && weapon.ammoId != "")
                    if(ammoDict.ContainsKey(weapon.ammoId))
                        weaponScript.ammoSource = ammoDict[weapon.ammoId];
                    else
                        Debug.LogError("DEEP.Entities.Player.Start: Ammo type not found!");

                // Adds the weapon to the list
                weaponInstances.Add(new Tuple<bool, WeaponBase>(weapon.enabled, weaponScript));

                // Shows current weapons on the HUD.
                bool[] weaponsEnabled = new bool[weaponInstances.Count];
                for(int i = 0; i < weaponInstances.Count; i++)
                    weaponsEnabled[i] = weaponInstances[i].Item1;
                Player.Instance.HUD.ammoAndWeapons.SetWeaponNumbers(weaponsEnabled);

            }

            initialized = true;

        }

        // Switches between the Player weapons.
        public void SwitchWeapons(int weaponNum) {

            if(!initialized) { 
				Debug.LogError("SwitchWeapons: You need to initialize the script first!"); 
				return;
			}

            // Verifies if it's a valid weapon, if it's not doesn't switch.
            if(weaponNum >= weaponInstances.Count || weaponInstances[weaponNum].Item1 == false)
                return;

            // Disables the current weapon object.
            if(currentWeapon != null) currentWeapon.gameObject.SetActive(false);

            // Assigns the new weapon as current weapon.
            currentWeapon = weaponInstances[weaponNum].Item2;

            // Enables the current weapon.
            currentWeapon.gameObject.SetActive(true);

            // Updates the ammo counter on the HUD.
            Player.Instance.HUD.ammoAndWeapons.SetAmmo(ammoDict[currentWeapon.ammoSource.id].ammo, ammoDict[currentWeapon.ammoSource.id].maxAmmo);

            // Updates the current weapon icon on the HUD.
            Player.Instance.HUD.ammoAndWeapons.SetCurrentWeapon(weaponNum, weapons[GetCurrentWeaponIndex()].icon, ammoDict[currentWeapon.ammoSource.id].icon);

        }

        // Attempts firing the current weapon.
        public void FireCurrentWeapon() {

            if(!initialized) { 
				Debug.LogError("FireCurrentWeapon: You need to initialize the script first!"); 
				return;
			}

            if(currentWeapon == null)
                return;

            currentWeapon.Shot();
            // Updates the ammo counter on the HUD.
            Player.Instance.HUD.ammoAndWeapons.SetAmmo(ammoDict[currentWeapon.ammoSource.id].ammo, ammoDict[currentWeapon.ammoSource.id].maxAmmo);

        }

        // Returns the index of the current weapon.
        public int GetCurrentWeaponIndex() {

            if(!initialized) { 
				Debug.LogError("GetCurrentWeaponIndex: You need to initialize the script first!"); 
				return -1;
			}

            // Searches for the current weapon index.
            int curWeaponIndex = -1;
            for(int i = 0; i < weaponInstances.Count; i++)
            {
                if(Equals(weaponInstances[i].Item2.name, currentWeapon.name))
                    curWeaponIndex = i;
            }

            // Returns the index or -1 if there's no weapon.
            return curWeaponIndex;

        }

        // Returns the index of the next enabled weapon (rolls around if no weapon with higher index is enabled).
        public int GetNextEnabledWeaponIndex() {

            if(!initialized) { 
				Debug.LogError("GetNextEnabledWeaponIndex: You need to initialize the script first!"); 
				return -1;
			}

            // Gets the current weapon index.
            int curWeaponIndex = GetCurrentWeaponIndex();

            // Checks if there is no weapons.
            if(curWeaponIndex == -1)
                return -1;

            for(int i = curWeaponIndex + 1; i < weaponInstances.Count; i++) // Checks for the weapons with higher indexes.
            {
                if(weaponInstances[i].Item1)
                    return i;
            }

            for(int i = 0; i < curWeaponIndex; i++) // Rolls around the list.
            {
                if(weaponInstances[i].Item1)
                    return i;
            }

            return curWeaponIndex;

        }

        // Returns the index of the previous enabled weapon (rolls around if no weapon with lower index is enabled).
        public int GetPreviousEnabledWeaponIndex() {

            if(!initialized) { 
				Debug.LogError("GetPreviousEnabledWeaponIndex: You need to initialize the script first!"); 
				return -1;
			}
           
            // Gets the current weapon index.
            int curWeaponIndex = GetCurrentWeaponIndex();

            // Checks if there is no weapons.
            if(curWeaponIndex == -1)
                return -1;

            for(int i = curWeaponIndex - 1; i >= 0; i--) // Checks for the weapons with lower indexes.
            {
                if(weaponInstances[i].Item1)
                    return i;
            }

            for(int i = weaponInstances.Count - 1; i > curWeaponIndex; i--) // Rolls around the list.
            {
                if(weaponInstances[i].Item1)
                    return i;
            }

            return curWeaponIndex;

        }

         // Pick's up a weapon and enables it's use.
        public bool GiveWeapon(int slot, int ammo, AudioClip feedbackAudio) {

            if(!initialized) { 
				Debug.LogError("GiveWeapon: You need to initialize the script first!"); 
				return false;
			}

            // Collects the weapon if the player doesn't have it yet.
            if(!weaponInstances[slot].Item1) {

                // Gets the old instance from the list.
                Tuple<bool, WeaponBase> weaponInstance;
                weaponInstance = weaponInstances[slot];
                weaponInstances.RemoveAt(slot);
                
                // Creates a new instance that is enabled and re-adds it to the list.
                Tuple<bool, WeaponBase> enabledInstance = new Tuple<bool, WeaponBase>(true, weaponInstance.Item2);
                weaponInstances.Insert(slot, enabledInstance);

                Debug.Log("Player.GiveWeapon: " + weaponInstance.Item2.name + " has been collected!");

                // Equips the weapon.
                SwitchWeapons(slot);

                // Updates the weapons on the HUD.
                bool[] weaponsEnabled = new bool[weaponInstances.Count];
                for(int i = 0; i < weaponInstances.Count; i++)
                    weaponsEnabled[i] = weaponInstances[i].Item1;
                Player.Instance.HUD.ammoAndWeapons.SetWeaponNumbers(weaponsEnabled);

                // Give the initial ammo to the player.
                GiveAmmo(ammo, weaponInstances[slot].Item2.ammoSource.id, feedbackAudio);

                // If collected, plays the player feedback sound.
                if(feedbackAudio != null)
                    Player.Instance.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

                return true;

            }

            // Tries giving ammo to the player if it already has the weapon.
            return GiveAmmo(ammo, weaponInstances[slot].Item2.ammoSource.id, feedbackAudio);

        }

        // Gives a certain type of ammo to the player.
        public bool GiveAmmo(int amount, string type, AudioClip feedbackAudio) {

            if(!initialized) { 
				Debug.LogError("GiveAmmo: You need to initialize the script first!"); 
				return false;
			}

            // Checks if the ammo type is valid.
            if(!ammoDict.ContainsKey(type)) return false;
            
            // Checks if ammo is not maxed out.
            if(ammoDict[type].ammo >= ammoDict[type].maxAmmo) return false;

            // Adds ammo to the source.
            ammoDict[type].GainAmmo(amount);

            // Updates the ammo counter on the HUD.
            if(currentWeapon != null)
                Player.Instance.HUD.ammoAndWeapons.SetAmmo(ammoDict[currentWeapon.ammoSource.id].ammo, ammoDict[currentWeapon.ammoSource.id].maxAmmo);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                Player.Instance.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            return true;

        }


    }

}
