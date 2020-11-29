using System.Collections.Generic;

using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.DoorsAndKeycards {
        
    // ========================================================================================================================
    // Contains the player's key inventory.
    // ========================================================================================================================
	public class KeyInventory {

		// PlayerController that owns this script.
		protected PlayerController ownerPlayer;
		public PlayerController Owner {
            get { return ownerPlayer; }
            set { ownerPlayer = value; }
        }

		// Colors on the inventory.
		public HashSet<KeysColors> inventory;

		public KeyInventory() {

			Debug.Log("Initializing KeyInventory...");

			// Creates the HashSet of Keys
			inventory = new HashSet<KeysColors>();

			# if UNITY_EDITOR
				Debug.Log("Creating new HashSet for the KeysInventory. Inventory size = " + inventory.Count);
			# endif

		}

		// Gives a keycard to the player.
        public void GiveKeyCard(KeysColors color, AudioClip feedbackAudio) {

            Debug.Log("Adding the " + color.ToString() + " key to the inventory");
			inventory.Add(color);

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                ownerPlayer.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Updates the collected keys on the HUD.
            ownerPlayer.HUD.Keycards.UpdateValues(this);

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