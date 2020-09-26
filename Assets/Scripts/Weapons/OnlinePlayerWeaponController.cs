using Mirror;

using UnityEngine;

using DEEP.Weapons;
using DEEP.Online.Entities.Player;

namespace DEEP.Online.Weapons {

    public class OnlinePlayerWeaponController : PlayerWeaponController
    {

        protected override void Start() {

            // TODO: make work in multiplayer.

            base.Start();

            if((Owner as OnlinePlayerController).Identity.isLocalPlayer)
                (Owner as OnlinePlayerController).Sync.CmdGiveDefaultWeapons();

        }

        protected override void Update() {

            // Ensures the code runs on the correct player.
            if(!(Owner as OnlinePlayerController).Identity.isLocalPlayer)
                return;

            // TODO: make work in multiplayer.

            base.Update();

        }

        // Switch Weapons =====================================================================================================

        // Asks the server to switch weapons.
        protected override void SwitchWeapons(int weaponNum) {
            (Owner as OnlinePlayerController).Sync.CmdSwitchWeapons(weaponNum);
        }

        [Server]
        // Checks if the player can switch the weapon and syncs with clients.
        public virtual void ServerSwitchWeapons(int weaponNum) {

            // Verifies if it's a valid weapon, if it's not doesn't switch.
            if(weaponNum >= weaponInstances.Count || weaponInstances[weaponNum].Item1 == false)
                return;

            (Owner as OnlinePlayerController).Sync.RpcSyncCurrentWeapon(weaponNum);

        }

        // Switches weapons on the client, forces the switch to avoid having to sync weapon 
        // inventories from server to all players.
        public virtual void ClientSwitchWeapons(int weaponNum) {
            base.ForceSwitchWeapons(weaponNum);
        }

        // ====================================================================================================================

        // Attempts firing the current weapon.
        protected override void FireCurrentWeapon() {

            // TODO: make work in multiplayer.

            base.FireCurrentWeapon();

        }

        // Returns the index of the current weapon.
        public override int GetCurrentWeaponIndex() {

            // TODO: make work in multiplayer.

            return base.GetCurrentWeaponIndex();

        }

        // Returns the index of the next enabled weapon (rolls around if no weapon with higher index is enabled).
        public override int GetNextEnabledWeaponIndex() {

            // TODO: make work in multiplayer.

            return base.GetNextEnabledWeaponIndex();

        }

        // Returns the index of the previous enabled weapon (rolls around if no weapon with lower index is enabled).
        public override int GetPreviousEnabledWeaponIndex() {
           
            // TODO: make work in multiplayer.

            return base.GetPreviousEnabledWeaponIndex();

        }

        // Give Weapon ========================================================================================================

        // Pick's up a weapon and enables it's use.
        public override bool GiveWeapon(int slot, int ammo, AudioClip feedbackAudio) {

            // Weapon collection is handled by the server.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                return false;

            // Removes the feedback audio before calling the base function.
            bool given = base.GiveWeapon(slot, ammo, null);

            // Syncs the weapon to all clients.
            if(given)
                (Owner as OnlinePlayerController).Sync.SyncWeapon(slot, true);

            // Plays the feedback audio.
            // TODO: Play feedback audio for correct player.

            return given;

        }

        public virtual bool GiveWeaponClient(int slot, int ammo, AudioClip feedbackAudio) {
            
            // Ignores on server because the weapon has already being received.
            if((Owner as OnlinePlayerController).Identity.isServer)
                return false;


            // Gives weapon to the client.
            base.GiveWeapon(slot, 0, feedbackAudio);

            return true;

        }

        // Give Ammo ==========================================================================================================

        // Gives a certain type of ammo to the player.
        public override bool GiveAmmo(int amount, string type, AudioClip feedbackAudio) {

            // TODO: make work in multiplayer.

            return base.GiveAmmo(amount, type, feedbackAudio);

        }

        // ====================================================================================================================

        // Hides the player's weapons.
        public override void DisableWeapons() { 
            
            // TODO: make work in multiplayer.

            DisableWeapons(); 
            
        }

    }

}
