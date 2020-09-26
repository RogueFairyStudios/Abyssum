using Mirror;

using UnityEngine;

using DEEP.Online.Weapons;

namespace DEEP.Online.Entities.Player
{

    public class OnlinePlayerSync : NetworkTransform
    {

        protected OnlinePlayerController target;

        protected void Awake() {
            target = GetComponent<OnlinePlayerController>();
        }

        [TargetRpc]
        public void SyncHealth(int oldValue, int newValue) {
            Debug.Log("OnlinePlayerSync: Syncing health [old: " + oldValue + ", new: " + newValue + "]");
            (target.Entity as OnlinePlayerEntity).OnChangeHealthClient(oldValue, newValue);
        }

        [TargetRpc]
        public void SyncArmor(int oldValue, int newValue) {
            Debug.Log("OnlinePlayerSync: Syncing armor [old: " + oldValue + ", new: " + newValue + "]");
            (target.Entity as OnlinePlayerEntity).OnChangeArmorClient(oldValue, newValue);
        }

        [TargetRpc]
        public void SyncWeapon(int index, bool available) {
            Debug.Log("OnlinePlayerSync: Syncing armor [index: " + index + ", available: " + available + "]");
            (target.Weapons as OnlinePlayerWeaponController).GiveWeaponClient(index);
        }

        [Command]
        public void CmdGiveDefaultWeapons() {
            (target.Weapons as OnlinePlayerWeaponController).GiveWeapon(0, 0, null);
        }

        [Command]
        public void CmdSwitchWeapons(int weaponNum) {
            (target.Weapons as OnlinePlayerWeaponController).ServerSwitchWeapons(weaponNum);
        }


        [ClientRpc]
        public void RpcSyncCurrentWeapon(int weaponNum) {
            
            if(isLocalPlayer)
                Debug.Log("OnlinePlayerSync: Syncing weapon [new: " + weaponNum + "]");

            (target.Weapons as OnlinePlayerWeaponController).ClientSwitchWeapons(weaponNum);

        }

        [TargetRpc]
        public void SyncAmmo(int amount, string type) {
            Debug.Log("OnlinePlayerSync: Syncing ammo [new: " + amount + ", tupe: " + type + "]");
            (target.Weapons as OnlinePlayerWeaponController).ClientSyncAmmo(amount, type);
        }

    }

}
