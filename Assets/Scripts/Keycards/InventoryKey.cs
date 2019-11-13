using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.KeyCard {
	public class InventoryKey : MonoBehaviour {
		public static HashSet<KeysColors> inventory;
		// Start is called before the first frame update
		void Start() {
			inventory = new HashSet<KeysColors>();
			Debug.Log("Creating new HashSet for the KeysInventory. Inventory size = " + inventory.Count);
		}

		// Update is called once per frame
		void Update() {
			if (Input.GetButtonDown("Interact")) {	// If the player tries to interact, raycast to see if there is a door
				FindDoor();
			}
		}

		private void FindDoor() {
			int layerMask = 1 << 10;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5.0f, layerMask)) {
				try{
					hit.collider.GetComponent<ColorsDoor>().TryOpenDoor();
				} catch {
					Debug.LogWarning("Couldn't access the ColorsDoor script from the object " + hit.collider.name);
				}
			}
		}
	}

	public enum KeysColors {
		Blue,
		Red,
		Yellow,
	};

}