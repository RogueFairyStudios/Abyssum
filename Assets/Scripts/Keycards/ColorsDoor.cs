using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.KeyCard {
	public class ColorsDoor : MonoBehaviour {
		public KeysColors doorColor = 0;

		public void TryOpenDoor() {
			print("Trying to open the door");
			if (InventoryKey.inventory.Contains(doorColor)) {
				OpenDoor();
			}
		}

		private void OpenDoor() {// Activates the OpenDoor animation
			print("Opening the door");
			gameObject.SetActive(false);
		}
	}
}