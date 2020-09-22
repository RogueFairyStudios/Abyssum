using UnityEngine;

using DEEP.Collectibles;
using DEEP.Entities.Player;

namespace DEEP.DoorsAndKeycards {

	public class ColorsKeyCard : CollectibleBase {

		public KeysColors keyColor = KeysColors.Blue;

		protected override void Collect() {

			// Gives the keycard to the player.
			PlayerController.Instance.keyInventory.GiveKeyCard(keyColor, collectionSound);
			base.Collect();

		}
	}

}