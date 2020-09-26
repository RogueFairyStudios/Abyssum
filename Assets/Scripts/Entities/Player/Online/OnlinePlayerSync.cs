using Mirror;

using UnityEngine;

namespace DEEP.Online.Entities.Player
{

    public class OnlinePlayerSync : NetworkTransform
    {

        protected OnlinePlayerEntity targetEntity;

        protected void Awake() {
            targetEntity = GetComponent<OnlinePlayerEntity>();
        }

        [TargetRpc]
        public void SyncHealth(int oldValue, int newValue) {
            Debug.Log("OnlinePlayerSync: Syncing health [old: " + oldValue + ", new: " + newValue + "]");
            targetEntity.OnChangeHealthClient(oldValue, newValue);
        }

        [TargetRpc]
        public void SyncArmor(int oldValue, int newValue) {
            Debug.Log("OnlinePlayerSync: Syncing armor [old: " + oldValue + ", new: " + newValue + "]");
            targetEntity.OnChangeArmorClient(oldValue, newValue);
        }

    }

}
