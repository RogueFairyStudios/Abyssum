using UnityEngine;

using DEEP.Entities;
using DEEP.Collectibles;

namespace DEEP.DoorsAndKeycards {

	public class ColorsKeyCard : CollectibleBase {

		public KeysColors keyColor = KeysColors.Blue;

		protected override void Collect() {

			// Gives the keycard to the player.
			Player.Instance.GiveKeyCard(keyColor, collectionSound);
			base.Collect();

		}
	}

}