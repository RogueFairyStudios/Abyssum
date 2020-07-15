using UnityEngine;
using DEEP.Entities;

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
            target = GameObject.FindGameObjectWithTag("Player");
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
            if (target == null)
                return;

            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (HasSight(target.transform.position))
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.white;

            Gizmos.DrawLine(lastTargetLocation, transform.position + agentSightOffset);
        }
    }
}