using Mirror;

using UnityEngine;

using DEEP.Weapons;
using DEEP.Entities.Player;

namespace DEEP.Online.Entities.Player
{

    // Class that controls the Player.
    [RequireComponent(typeof(NetworkIdentity))]

    [RequireComponent(typeof(PlayerEntity))]
    [RequireComponent(typeof(PlayerMovementation))]
    [RequireComponent(typeof(PlayerWeaponController))]
    public class PlayerOnlineController : PlayerController
    {

        NetworkIdentity identity;

        protected void Start() {

            identity = GetComponent<NetworkIdentity>();

            if(!identity.isLocalPlayer) {

                rEntity.enabled = false;
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

    }

}