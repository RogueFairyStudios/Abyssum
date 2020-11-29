using System;
using System.Collections.Generic;

using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Weapons {
 
    // ========================================================================================================================
    // Class that manages the player's weapons.
    // ========================================================================================================================
    public class PlayerWeaponController : MonoBehaviour
    {

        // PlayerController that owns this script.
        protected PlayerController ownerPlayer;
        public PlayerController Owner {
            get { return ownerPlayer; }
            set { ownerPlayer = value; }
        }

        [Tooltip("All of the player weapons.")]
        public List<PlayerWeapon> weapons;

        // Stores the weapons instances with their info.
        protected List<Tuple<bool, WeaponBase>> weaponInstances;

        [Tooltip("Where Player weapons should be.")]
        public Transform weaponPosition;

        // Stores the current weapon.
        [SerializeField] public WeaponBase currentWeapon;

        [Tooltip("Ammo sources carried by the player.")]
        public List<AmmoSource> ammoTypes;
        // Stores a dictionary with the AmmoSource instances.
        private Dictionary<string, AmmoSource> ammoDict;

        protected virtual void Start() {

            Debug.Log("Initializing PlayerWeaponController...");
            
            // Creates a dictionary with the ammo sources.
            foreach(AmmoSource source in ammoTypes)
                CreateAmmo(source);

            // Creates the the weapons.
            foreach (PlayerWeapon weapon in weapons)
                CreateWeapon(weapon, weaponPosition);

            // Shows current weapons on the HUD.
            UpdateWeaponHUD();

        }

        protected virtual void Update() {

            // Equiping weapons ===================================================================================

            // Verifies the number keys.
            if(Input.GetKeyDown("0")) // Checks for 0.
                SwitchWeapons(9); // 0 is the rightmost key so it's actually the last weapon at index 9.
            else {
                for(int i = 1; i <= 9; i++) // Checks for the other keys.
                    if(Input.GetKeyDown(i.ToString()))
                        SwitchWeapons(i - 1);  // Converts the key into the weapon index of the list.
            }

            // Check if player has a weapon
            if (currentWeapon != null)
            {

                // Change weapon using mouse scrollwheel =========================================================
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) // Scroll Up
                    SwitchWeapons(GetNextEnabledWeaponIndex());
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // Scroll Down
                    SwitchWeapons(GetPreviousEnabledWeaponIndex());

                // Try firing weapons ============================================================================
                if (Input.GetButton("Fire1"))
                    FireCurrentWeapon();
            }

        }

        protected virtual void CreateAmmo(AmmoSource source) {

            // Creates ammo dictionary if necessary.
            if(ammoDict == null) {
                ammoDict = new Dictionary<string, AmmoSource>();
            // Checks if the ammo type is not on the dictionary already.
            } else if(ammoDict.ContainsKey(source.id)) {
                Debug.LogWarning("DEEP.Weapons.PlayerWeaponController.CreateAmmo: Ammo type already exists!");
                return;
            }

            // Adds ammo type to dictionary.
            ammoDict.Add(source.id, Instantiate(source));

        }

        // Creates a new weapon parented to a certain spawn position, returns the WeaponBase for the new weapon.
        protected virtual void CreateWeapon(PlayerWeapon weapon, Transform spawn) {

            // Creates weapon instances list if necessary.
            if(weaponInstances == null)
                weaponInstances = new List<Tuple<bool, WeaponBase>>();

            // Creates the weapons at the weapon spawn position.
            GameObject weaponObj = Instantiate(weapon.prefab, spawn.position, spawn.rotation);
            weaponObj.transform.SetParent(spawn);

            // Disables the weapon at start.
            weaponObj.SetActive(false);

            // Gets the weapon script.
            WeaponBase weaponScript = weaponObj.GetComponent<WeaponBase>();
            if(weaponScript == null) {
                Debug.LogError("DEEP.Weapons.PlayerWeaponController.CreateWeapon: Weapon has no weapon script!");
                return;
            }

            // Sets the ammo source of the weapon.
            if(weapon.ammoId != null && weapon.ammoId != "")
                if(ammoDict.ContainsKey(weapon.ammoId))
                    weaponScript.ammoSource = ammoDict[weapon.ammoId];
                else
                    Debug.LogError("DEEP.Weapons.PlayerWeaponController.CreateWeapon: Ammo type not found!");

            // Adds the weapon to the list
            weaponInstances.Add(new Tuple<bool, WeaponBase>(weapon.enabled, weaponScript));

        }

        // Updates the weapons list on the HUD.
        protected virtual void UpdateWeaponHUD() {

            bool[] weaponsEnabled = new bool[weaponInstances.Count];
            for(int i = 0; i < weaponInstances.Count; i++)
                weaponsEnabled[i] = weaponInstances[i].Item1;
            ownerPlayer.HUD.AmmoAndWeapons.SetWeaponNumbers(weaponsEnabled);

            // Updates the current weapon icon on the HUD.
            if(currentWeapon != null) {
                int curWeaponIndex = GetCurrentWeaponIndex();
                ownerPlayer.HUD.AmmoAndWeapons.SetCurrentWeapon(curWeaponIndex, weapons[curWeaponIndex].icon, ammoDict[currentWeapon.ammoSource.id].icon);
            }

        }

        // Updates the ammo counter on the HUD.
        protected virtual void UpdateAmmoHUD() {
            
            if(currentWeapon != null)
               ownerPlayer.HUD.AmmoAndWeapons.SetAmmo(ammoDict[currentWeapon.ammoSource.id].ammo, ammoDict[currentWeapon.ammoSource.id].maxAmmo);

        }

        // Switches between the Player weapons.
        protected virtual void SwitchWeapons(int weaponNum) {

            // Verifies if it's a valid weapon, if it's not doesn't switch.
            if(weaponNum >= weaponInstances.Count || weaponInstances[weaponNum].Item1 == false)
                return;

            SetCurrentWeapon(weaponNum);

        }

        // Sets the current weapon.
        protected virtual void SetCurrentWeapon(int weaponNum) {

            // Disables the current weapon object.
            if(currentWeapon != null) currentWeapon.gameObject.SetActive(false);

            // Assigns the new weapon as current weapon.
            currentWeapon = weaponInstances[weaponNum].Item2;

            // Enables the current weapon.
            currentWeapon.gameObject.SetActive(true);

            // Updates the HUD.
            UpdateAmmoHUD();
            UpdateWeaponHUD();        

        }

        // Attempts firing the current weapon.
        protected virtual void FireCurrentWeapon() {

            if(currentWeapon == null)
                return;

            currentWeapon.Shot();
            // Updates the ammo counter on the HUD.
            ownerPlayer.HUD.AmmoAndWeapons.SetAmmo(ammoDict[currentWeapon.ammoSource.id].ammo, ammoDict[currentWeapon.ammoSource.id].maxAmmo);

        }

        // Returns the index of the current weapon.
        public virtual int GetCurrentWeaponIndex() {

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
        public virtual int GetNextEnabledWeaponIndex() {

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
        public virtual int GetPreviousEnabledWeaponIndex() {
           
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
        public virtual bool GiveWeapon(int slot, int ammo, AudioClip feedbackAudio) {

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
                SetCurrentWeapon(slot);

                // Updates the weapons on the HUD.
                UpdateWeaponHUD();

                // Give the initial ammo to the player.
                GiveAmmo(ammo, weaponInstances[slot].Item2.ammoSource.id, feedbackAudio);

                // If collected, plays the player feedback sound.
                if(feedbackAudio != null)
                    ownerPlayer.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

                return true;

            }

            // Tries giving ammo to the player if it already has the weapon.
            return GiveAmmo(ammo, weaponInstances[slot].Item2.ammoSource.id, feedbackAudio);

        }

        // Gives a certain type of ammo to the player.
        public virtual bool GiveAmmo(int amount, string type, AudioClip feedbackAudio) {

            // Checks if the ammo type is valid.
            if(!ammoDict.ContainsKey(type)) return false;
            
            // Checks if ammo is not maxed out.
            if(ammoDict[type].ammo >= ammoDict[type].maxAmmo) return false;

            // Adds ammo to the source.
            ammoDict[type].GainAmmo(amount);

            // Updates the HUD
            UpdateAmmoHUD();

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                ownerPlayer.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            return true;

        }

        // Hides the player's weapons.
        public virtual void DisableWeapons() { weaponPosition.gameObject.SetActive(false); }

    }

}
