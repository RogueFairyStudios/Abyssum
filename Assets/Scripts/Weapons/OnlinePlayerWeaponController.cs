using Mirror;

using UnityEngine;

using DEEP.Weapons;
using DEEP.Online.Entities.Player;

namespace DEEP.Online.Weapons {

    public class OnlinePlayerWeaponController : PlayerWeaponController
    {

        protected override void Start() {

            base.Start();

            // Ask server for default weapons.
            if((Owner as OnlinePlayerController).Identity.isLocalPlayer)
                (Owner as OnlinePlayerController).Sync.CmdGiveDefaultWeapons(); // !FIX: weapon doesn't appear for some clients until switch.

        }

        protected override void Update() {

            // Ensures the code runs on the correct player.
            if(!(Owner as OnlinePlayerController).Identity.isLocalPlayer)
                return;

            base.Update();

        }

        // Switch Weapons =====================================================================================================

        // Asks the server to switch weapons.
        protected override void SwitchWeapons(int weaponNum) {

            // If not the server, asks the server to switch weapons.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                (Owner as OnlinePlayerController).Sync.CmdSwitchWeapons(weaponNum);
            else // If in the server switchs weapon immediatly.
                ServerSwitchWeapons(weaponNum);

        }

        [Server]
        // Checks if the player can switch the weapon and syncs with clients.
        public virtual void ServerSwitchWeapons(int weaponNum) {

            // Verifies if it's a valid weapon, if it's not doesn't switch.
            if(weaponNum >= weaponInstances.Count || weaponInstances[weaponNum].Item1 == false)
                return;

            (Owner as OnlinePlayerController).Sync.RpcSyncCurrentWeapon(weaponNum);

        }

        [Server]
        // Switch the weapon immediatly and syncs with clients.
        public virtual void ServerForceSwitchWeapons(int weaponNum) {
            (Owner as OnlinePlayerController).Sync.RpcSyncCurrentWeapon(weaponNum);
        }

        // Switches weapons on the client, forces the switch to avoid having to sync weapon 
        // inventories from server to all players.
        public virtual void ClientSwitchWeapons(int weaponNum) {
            base.SetCurrentWeapon(weaponNum);
        }

        // ====================================================================================================================

        // Attempts firing the current weapon.
        protected override void FireCurrentWeapon() {

            // TODO: make work in multiplayer.

            base.FireCurrentWeapon();

        }

        // Give Weapon ========================================================================================================

        // Pick's up a weapon and enables it's use.
        public override bool GiveWeapon(int slot, int ammo, AudioClip feedbackAudio) {

            // Weapon collection is handled by the server.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                return false;

            // Removes the feedback audio before calling the base function.
            bool givenWeapon = base.GiveWeapon(slot, 0, null);

            // Syncs the weapon to all clients.
            if(givenWeapon)
                (Owner as OnlinePlayerController).Sync.SyncWeapon(slot, true);              

            // !FIX: Ammo not being given.

            // Gives the weapon ammo to the player.
            bool givenAmmo = GiveAmmo(ammo, weaponInstances[slot].Item2.ammoSource.name, null);

            // Plays the feedback audio.
            // TODO: Play feedback audio for correct player.

            return givenWeapon || givenAmmo;

        }

        public virtual bool GiveWeaponClient(int slot) {

            // Gives weapon to the client.
            bool given = base.GiveWeapon(slot, 0, null);

            // Weapon collection is handled by the server.
            if((Owner as OnlinePlayerController).Identity.isServer)
                ServerForceSwitchWeapons(slot);

            return given;

        }

        // Give Ammo ==========================================================================================================

        // Gives a certain type of ammo to the player.
        public override bool GiveAmmo(int amount, string type, AudioClip feedbackAudio) {

            // Ammo is handled by the server.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                return false;

            // Removes the feedback audio before calling the base function.
            bool given = base.GiveAmmo(amount, type, null);

            // Syncs ammo to the client if it's not the host.
            if(given && !(Owner as OnlinePlayerController).Identity.isLocalPlayer) {
                (Owner as OnlinePlayerController).Sync.SyncAmmo(amount, type);
            }

            // Plays the feedback audio.
            // TODO: Play feedback audio for correct player.

            return given;

        }

        public virtual bool ClientSyncAmmo(int amount, string type) {

            // Ignores on server because the ammo has already being received.
            if((Owner as OnlinePlayerController).Identity.isServer)
                return false;

            // Gives the ammo to the client.
            return base.GiveAmmo(amount, type, null);

        }

        // ====================================================================================================================

        // Hides the player's weapons.
        public override void DisableWeapons() { 
            
            // TODO: make work in multiplayer.

            base.DisableWeapons(); 
            
        }

    }

}
