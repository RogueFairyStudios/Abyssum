using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.DoorsAndKeycards {
	public class ColorsKeyCard : MonoBehaviour {
		public KeysColors keyColor = 0;

		void OnTriggerEnter(Collider collider) {
			if (collider.tag == "Player") {
				print("Adding the " + keyColor.ToString() + " key to the inventory");
				InventoryKey.inventory.Add(keyColor);
				Destroy(gameObject);
			}
		}
	}
}