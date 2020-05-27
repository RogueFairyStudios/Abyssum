using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.DoorsAndKeycards {
	public class InventoryKey : MonoBehaviour {

		public HashSet<KeysColors> inventory;

		[SerializeField] LayerMask tryOpenMask = new LayerMask();

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

			inventory.Add(color);

		}

		public bool HasKey(KeysColors color) {

			return inventory.Contains(color);

		}

	}

	public enum KeysColors {
		Blue,
		Red,
		Yellow,
	};

}