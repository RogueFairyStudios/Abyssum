using UnityEngine;

using DEEP.Entities;
using DEEP.Collectibles;

namespace DEEP.DoorsAndKeycards {

	public class ColorsKeyCard : CollectibleBase {

		public KeysColors keyColor = KeysColors.Blue;

		protected override void Collect(Player player) {

			player.GiveKeyCard(keyColor, collectionSound);

			Destroy(gameObject);

		}
	}

}