using UnityEngine;

namespace DEEP.DoorsAndKeycards {

	[RequireComponent(typeof(Animator))]	
	public class Door : DoorBase {

		[SerializeField] private bool needKey = false;
		[SerializeField] private KeysColors doorColor = 0;

		public override void TryOpenDoor(KeyInventory inventory) {

			if (lockState)
				return;

			# if UNITY_EDITOR
				Debug.Log("Trying to open the door");
			# endif

			// Don't open the door during an animation.
			if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Opening") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Closing"))
				return;
			
			if (!needKey || inventory.HasKey(doorColor) && !isOpen) {
				OpenDoor();
			} else if(_source != null && !_source.isPlaying) {
				_source.clip = lockedClip;
				_source.Play();
			}

		}

	}
}