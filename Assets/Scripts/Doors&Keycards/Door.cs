using System.Collections;

using UnityEngine;
using UnityEngine.AI;

using DEEP.Stage;
using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.DoorsAndKeycards {

	[RequireComponent(typeof(Animator))]	
	public class Door : MonoBehaviour, ITrappable {

		[Header("Door Settings")]

		[Tooltip("Current state of the door, will be set to the state marked here at start.")]
		[SerializeField] private bool isOpen = false;
		[Tooltip("If marked, the player won't be able to interact with the door.")]
		[SerializeField] private bool lockState = false;

		[SerializeField] private bool needKey = false;
		[SerializeField] private KeysColors doorColor = 0;

		[SerializeField] private bool autoClose = true;
		[SerializeField] private float autoCloseDelay = 2.0f;

		[Tooltip("Half height to check for entity clearance, door half height is recommended.")]
		[SerializeField] private float clearanceHalfHeight = 1.25f;
		[Tooltip("Radius to check for entity clearance, door width is recommended at minimum.")]
		[SerializeField] private float clearanceRadius = 3.5f;


		[Header("Door Audio")]
		[SerializeField] private AudioClip openClip = null;
		[SerializeField] private AudioClip lockedClip = null;

		[Header("Component References")]
		private Animator _animator = null;
		[SerializeField] private AudioSource _source = null;
		[SerializeField] private NavMeshObstacle _navObstacle = null;
		[SerializeField] private OcclusionPortal _occlusion = null;
		[SerializeField] private Collider _collider = null;

		private Coroutine autoCloseCoroutine;

		// Object used to wait in coroutines.
		private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

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

			autoCloseCoroutine = null;

		}

		public void TryOpenDoor() {

			if (lockState)
				return;

			Debug.Log("Trying to open the door");

			// Dont open the door during an animation.
			if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Opening") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Closing"))
				return;
			
			if (!needKey || PlayerController.Instance.keyInventory.HasKey(doorColor) && !isOpen) {
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

			// Closes the door after some time.
			if(autoClose && !lockState)
				StartCoroutine(AutoClose());
			
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

			if (_collider != null)
				_collider.enabled = false;

		}

		public void OnFinishOpening() {

			if(_navObstacle != null)
				_navObstacle.enabled = false;
			
		}

		public void OnStartClosing() {

			if(_navObstacle != null)
				_navObstacle.enabled = true;

		}

		public void OnFinishClosing() {

			if (_collider != null)
				_collider.enabled = true;

			if (_occlusion != null)
				_occlusion.open = false;

		}

		IEnumerator AutoClose()
		{

			float time = 0.0f;
			while(true)
			{

				if (time > autoCloseDelay)
				{

					if (CheckClearance()) // Of no entities nearby, ends the delay.
						break;
					else // Otherwise, resets the delay.
						time = 0.0f;

				}
				else
				{
					time += Time.fixedDeltaTime;
					yield return waitForFixed;
				}

				
			}

			// Closes the door.
			CloseDoor();

			// Clears the coroutine.
			autoCloseCoroutine = null;

		}

		// Checks if no entities that could get caught in the door are nearby.
		bool CheckClearance()
		{

			EntityBase[] entities = FindObjectsOfType<EntityBase>();

			foreach(EntityBase entity in entities)
			{
				float heightDiff = Mathf.Abs(entity.transform.position.y - transform.position.y);
				float distance = new Vector2(entity.transform.position.x - transform.position.x, entity.transform.position.z - transform.position.z).magnitude;
				if (heightDiff < clearanceHalfHeight && distance < clearanceRadius)
					return false;
			}

			return true;

		}

		public void ActivateTrap()
		{

			if (autoCloseCoroutine != null)
				StopCoroutine(autoCloseCoroutine);

			if(isOpen)
				CloseDoor();
			else
				OpenDoor();
		}

	}
}