using UnityEngine;

using DEEP.Collectibles;
using DEEP.Entities.Player;

namespace DEEP.DoorsAndKeycards {

	public class ColorsKeyCard : CollectibleBase {

		public KeysColors keyColor = KeysColors.Blue;

		public override bool Collect(GameObject player) {

			// Gives the keycard to the player.
			player.GetComponent<PlayerController>().Keys.GiveKeyCard(keyColor, collectionSound);
			return true;

		}
	}

}