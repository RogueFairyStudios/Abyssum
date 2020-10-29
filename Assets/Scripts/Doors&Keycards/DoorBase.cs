using System.Collections;

using UnityEngine;
using UnityEngine.AI;

using DEEP.Stage;
using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.DoorsAndKeycards {

	[RequireComponent(typeof(Animator))]	
	public abstract class DoorBase : MonoBehaviour, ITrappable {

		[Header("Door Settings")]

		[Tooltip("Current state of the door, will be set to the state marked here at start.")]
		[SerializeField] protected bool isOpen = false;
		[Tooltip("If marked, the player won't be able to interact with the door.")]
		[SerializeField] protected bool lockState = false;

		[SerializeField] protected bool autoClose = true;
		[SerializeField] protected float autoCloseDelay = 2.0f;

		[Tooltip("Half height to check for entity clearance, door half height is recommended.")]
		[SerializeField] protected float clearanceHalfHeight = 1.25f;
		[Tooltip("Radius to check for entity clearance, door width is recommended at minimum.")]
		[SerializeField] protected float clearanceRadius = 3.5f;


		[Header("Door Audio")]
		[SerializeField] protected AudioClip openClip = null;
		[SerializeField] protected AudioClip lockedClip = null;

		[Header("Component References")]
		[SerializeField] protected AudioSource _source = null;
		[SerializeField] protected NavMeshObstacle _navObstacle = null;
		[SerializeField] protected OcclusionPortal _occlusion = null;
		[SerializeField] protected Collider _collider = null;
		protected Animator _animator = null;

		protected Coroutine autoCloseCoroutine;

		// Object used to wait in coroutines.
		protected WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

		protected void Start()
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

		public abstract void TryOpenDoor(KeyInventory inventory);

		protected void OpenDoor() {// Activates the OpenDoor animation		

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