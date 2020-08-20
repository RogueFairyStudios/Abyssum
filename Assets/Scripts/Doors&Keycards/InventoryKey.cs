using System.Collections.Generic;
using UnityEngine;

namespace DEEP.DoorsAndKeycards {
	public class InventoryKey : MonoBehaviour {

		public HashSet<KeysColors> inventory;

		[SerializeField] LayerMask tryOpenMask = new LayerMask();

		// If this script has been initialzed by the main Player script.
        private bool initialized = false;

		// Start is called before the first frame update
		public void Initialize() {

			Debug.Log("Initializing InventoryKey...");

			inventory = new HashSet<KeysColors>();
			Debug.Log("Creating new HashSet for the KeysInventory. Inventory size = " + inventory.Count);

			initialized = true;

		}

		// Update is called once per frame
		void Update() {
			
			// Returns if not initialized.
            if(!initialized) return;

			if (Input.GetButtonDown("Interact")) {	// If the player tries to interact, raycast to see if there is a door
				FindDoor();
			}

		}

		void FindDoor() {

			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5.0f, tryOpenMask)) {
				try{
					hit.collider.GetComponent<Door>().TryOpenDoor();
				} catch {
					Debug.LogWarning("Couldn't access the ColorsDoor script from the object " + hit.collider.name);
				}
			}
		}

		public void AddKey(KeysColors color) {

			if(!initialized) { 
				Debug.LogError("AddKey: You need to initialize the script first!"); 
				return;
			}

			inventory.Add(color);

		}

		public bool HasKey(KeysColors color) {

			if(!initialized) { 
				Debug.LogError("HasKey: You need to initialize the script first!"); 
				return false;
			}

			return inventory.Contains(color);

		}

	}

	public enum KeysColors {
		Blue,
		Red,
		Yellow,
	};

}