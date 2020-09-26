using Mirror;

using UnityEngine;

using DEEP.Weapons;
using DEEP.Entities.Player;

namespace DEEP.Online.Entities.Player
{

    // Class that controls the Player.
    [RequireComponent(typeof(OnlinePlayerSync))]
    [RequireComponent(typeof(OnlinePlayerEntity))]
    [RequireComponent(typeof(PlayerMovementation))]
    [RequireComponent(typeof(PlayerWeaponController))]
    public class OnlinePlayerController : PlayerController
    {

        protected NetworkIdentity identity;
        public NetworkIdentity Identity { get { return identity; } }

        protected OnlinePlayerSync sync;
        public OnlinePlayerSync Sync { get { return sync; } }

        protected void Start() {

            identity = GetComponent<NetworkIdentity>();
            sync = GetComponent<OnlinePlayerSync>();

            if(!identity.isLocalPlayer) {

                Movementation.enabled = false;
                Weapons.enabled = false;
                HUD.gameObject.SetActive(false);

                Camera cam = GetComponentInChildren<Camera>();
                cam.gameObject.SetActive(false);

            }

        }

        protected override void Update() {

            if(identity.isLocalPlayer)
                base.Update();

        }

         // Pauses and un-pauses the game.
        public override void TogglePause() {

            // Confirms that this function is running on the localPlayer.
            if(!identity.isLocalPlayer) 
                return;

            base.TogglePause();


        }

        // Sets the timeScale, used by the TogglePause function.
        protected override void SetTimeScale(float scale) { /* You can't pause an online game! */ }

        public void Disconnect() {

            // Gets a reference to the game's NetworkManager.
            NetworkManager manager = FindObjectOfType<NetworkManager>();

            if(Identity.isServer)
                manager.StopServer();
            else
                manager.StopClient();

        }

    }

}