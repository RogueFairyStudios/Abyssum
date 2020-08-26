using System.Collections.Generic;
using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.DoorsAndKeycards {

	public class KeyInventory {

		public HashSet<KeysColors> inventory;

		// Start is called before the first frame update
		public KeyInventory() {

			Debug.Log("Initializing InventoryKey...");

			inventory = new HashSet<KeysColors>();
			Debug.Log("Creating new HashSet for the KeysInventory. Inventory size = " + inventory.Count);

		}


		// Gives a keycard to the player.
        public void GiveKeyCard(KeysColors color, AudioClip feedbackAudio) {

            Debug.Log("Adding the " + color.ToString() + " key to the inventory");
			inventory.Add(color);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                PlayerController.Instance.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Updates the collected keys on the HUD.
            PlayerController.Instance.HUD.keycards.UpdateValues();

        }

		// Checks if the player has a certain keycard.
		public bool HasKey(KeysColors color) { return inventory.Contains(color); }

	}

	public enum KeysColors {
		Blue,
		Red,
		Yellow,
	};

}