using UnityEngine;
using System.Collections;

namespace DEEP.AI
{
    public class BaseSwimAI : BaseEntityAI
    {
        [HideInInspector] public Animator _animator;
        [HideInInspector] public Rigidbody _rigid;
        [HideInInspector] public Collider _collider;

        [HideInInspector] public LayerMask collisionMask;

        protected WanderSwimState wanderState;

        protected override void Awake()
        {

            base.Awake();

            // Gets the main components.
            _animator = GetComponentInChildren<Animator>();
            _rigid = GetComponentInChildren<Rigidbody>();
            _collider = GetComponentInChildren<Collider>();

            // Gets the collision mask for this game object's layer
            for(int i = 0; i < 32; i++)
            {
                if(!Physics.GetIgnoreLayerCollision(gameObject.layer, i))
                    collisionMask.value = collisionMask.value | 1 << i;
            }

            wanderState = GetComponent<WanderSwimState>();
        }

        protected virtual void Start()
        {
            // Initializes with the wander state
            wanderState.enabled = true;
        }
    }
}