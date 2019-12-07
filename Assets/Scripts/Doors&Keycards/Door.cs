using UnityEngine;

namespace DEEP.DoorsAndKeycards {
	public class Door : MonoBehaviour {

		public bool needKey = false;
		public KeysColors doorColor = 0;

		public AudioClip openClip;
		public AudioClip lockedClip;

		private Animator _animator;
		private AudioSource _source;

		private void Start() {

			_animator = GetComponentInChildren<Animator>();
			_source = GetComponentInChildren<AudioSource>();

		}

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

			this.enabled = false;

		}
	}
}