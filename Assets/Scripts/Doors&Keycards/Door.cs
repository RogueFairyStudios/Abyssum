using UnityEngine;
using UnityEngine.AI;

using DEEP.Stage;

namespace DEEP.DoorsAndKeycards {
	
	public class Door : MonoBehaviour, ITrappable {

		[SerializeField] private bool needKey = false;
		[SerializeField] private bool isOpen = false;
		[SerializeField] private KeysColors doorColor = 0;

		[SerializeField] private AudioClip openClip = null;
		[SerializeField] private AudioClip lockedClip = null;

		[Header("Component References")]
		[SerializeField] private Animator _animator = null;
		[SerializeField] private AudioSource _source = null;
		[SerializeField] private GameObject _navLinkObj = null;
		[SerializeField] private OcclusionPortal _occlusion = null;
		[SerializeField] private Collider _collider = null;

		private void Start()
		{
			_animator.SetBool("Open", isOpen);
		}

		public void TryOpenDoor() {

			print("Trying to open the door");
			
			if (!needKey || InventoryKey.inventory.Contains(doorColor) && !isOpen) {
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
			isOpen = true;

			
			if(_source != null) {
				_source.clip = openClip;
				_source.Play();
			}

			if(_navLinkObj != null)
				_navLinkObj.SetActive(true);

			if(_occlusion != null)
				_occlusion.open = true;

			if(_collider != null)
				_collider.enabled = false;
		}

		public void CloseDoor() //For traps and special events
		{
			_animator.SetBool("Open", false);

			if(_navLinkObj != null)
				_navLinkObj.SetActive(false);

			if(_occlusion != null)
				_occlusion.open = false;
		}

		public void ActivateTrap()
		{
			if(isOpen)
				CloseDoor();
			else
				OpenDoor();
		}
	}
}