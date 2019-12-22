using UnityEngine;
using UnityEngine.AI;

using DEEP.Stage;

namespace DEEP.DoorsAndKeycards {

	[RequireComponent(typeof(Animator))]	
	public class Door : MonoBehaviour, ITrappable {

		[Header("Door Settings")]
		[SerializeField] private bool needKey = false;
		[SerializeField] private bool isOpen = false;
		[SerializeField] private KeysColors doorColor = 0;

		[Header("Door Audio")]
		[SerializeField] private AudioClip openClip = null;
		[SerializeField] private AudioClip lockedClip = null;

		[Header("Component References")]
		private Animator _animator = null;
		[SerializeField] private AudioSource _source = null;
		[SerializeField] private NavMeshObstacle _navObstacle = null;
		[SerializeField] private OcclusionPortal _occlusion = null;
		[SerializeField] private Collider _collider = null;

		private void Start()
		{
			_animator = GetComponent<Animator>();
			_animator.SetBool("Open", isOpen);

			if(_collider != null)
				_collider.enabled = !isOpen;

			if(_navObstacle != null)
				_navObstacle.enabled = !isOpen;

			if(_occlusion != null)
				_occlusion.open = isOpen;

		}

		public void TryOpenDoor() {

			print("Trying to open the door");

			// Dont open the door during an animation.
			if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Opening") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Closing"))
				return;
			
			if (!needKey || InventoryKey.inventory.Contains(doorColor) && !isOpen) {
				OpenDoor();
			} else if(_source != null && !_source.isPlaying) {
				_source.clip = lockedClip;
				_source.Play();
			}

		}

		private void OpenDoor() {// Activates the OpenDoor animation		

			isOpen = true;

			// Plays 
			if(_source != null) {
				_source.clip = openClip;
				_source.Play();
			}

			_animator.SetBool("Open", true);
			// The remaining of the opening process should be called by a DoorStateHandler script in the animator.	
			
		}

		public void CloseDoor() //For traps and special events
		{

			isOpen = false;

			_animator.SetBool("Open", false);
			// The remaining of the closing process should be called by a DoorStateHandler script in the animator.
		
		}

		public void OnStartOpening() {

			if(_occlusion != null)
				_occlusion.open = true;

		}

		public void OnFinishOpening() {

			if(_collider != null)
				_collider.enabled = false;

			if(_navObstacle != null)
				_navObstacle.enabled = false;
			
		}

		public void OnStartClosing() {

			if(_collider != null)
				_collider.enabled = true;

			if(_navObstacle != null)
				_navObstacle.enabled = true;

		}

		public void OnFinishClosing() {
			
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