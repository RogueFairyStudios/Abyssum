using UnityEngine;

using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.AI
{
    public class BlowfishAI : BaseSwimAI
    {
        ChaseSwimState chaseState;
        Blowfish entityScript;

        protected override void Awake()
        {
            chaseState = GetComponent<ChaseSwimState>();
            entityScript = GetComponent<Blowfish>();
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            chaseState.enabled = false;
        }

        private void Update()
        {
            if(HasTargetSight() && !chaseState.enabled)
                Aggro();
        }

        public void Wander()
        {
            chaseState.enabled = false;
            wanderState.enabled = true;
        }

        public void Chase()
        {
            chaseState.enabled = true;
            wanderState.enabled = false;
        }

        public void Inflate()
        {
            entityScript.Inflate();
        }

        public void Deflate()
        {
            entityScript.Deflate();
        }

        public void Explode()
        {
            entityScript.Explode();
        }

        public override void Aggro()
        {
            if(wanderState.enabled)
            {
                OnAggro?.Invoke();
                entityScript.Growl();
                Chase();
            }
        }

        void OnDrawGizmos()
        {
            if (PlayerController.Instance == null)
                return;

            float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

            if (HasSight(PlayerController.Instance.transform.position))
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.white;

            Gizmos.DrawLine(lastTargetLocation, transform.position + agentSightOffset);
        }
    }
}