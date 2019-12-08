using UnityEngine;
using UnityEngine.AI;

namespace DEEP.DoorsAndKeycards {
	public class Door : MonoBehaviour {

		[SerializeField] private bool needKey = false;
		[SerializeField] private KeysColors doorColor = 0;

		[SerializeField] private AudioClip openClip = null;
		[SerializeField] private AudioClip lockedClip = null;

		[Header("Component References")]
		[SerializeField] private Animator _animator = null;
		[SerializeField] private AudioSource _source = null;
		[SerializeField] private NavMeshObstacle _aiObstacle = null;
		[SerializeField] private OcclusionPortal _occlusion = null;

		public void TryOpenDoor() {

			print("Trying to open the door");
			
			if (!needKey || InventoryKey.inventory.Contains(doorColor)) {
				OpenDoor();
			} else if(_source != null && !_source.isPlaying) {
				_source.clip = lockedClip;
				_source.Play();
			}

		}

		private void OpenDoor() {// Activates the OpenDoor animation

			print("Opening the door");			

			if(_animator == null) {
				gameObject.SetActive(false);
				return;
			}

			_animator.SetBool("Open", true);

			
			if(_source != null) {
				_source.clip = openClip;
				_source.Play();
			}

			if(_aiObstacle != null)
				_aiObstacle.enabled = false;

			if(_occlusion != null)
				_occlusion.open = true;

			Destroy(this);

		}
	}
}