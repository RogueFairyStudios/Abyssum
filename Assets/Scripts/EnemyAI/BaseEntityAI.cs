using UnityEngine;

namespace DEEP.AI
{
    public class BaseEntityAI : MonoBehaviour
    {
        public delegate void Reaction();
        public Reaction OnAggro, OnLoseAggro;

        [SerializeField] protected float detectRange = 40.0f;
        [SerializeField] protected LayerMask sightMask = new LayerMask();

        public float DetectRange => detectRange;
        
        protected Vector3 agentSightOffset;

        [HideInInspector] public GameObject target;
        [HideInInspector] public Vector3 lastTargetLocation; //location to search if the target has been missed

        // Checks if has sight to target.
        public bool HasTargetSight()
        {
            // Checks for sight.
            bool hasSight = HasSight(target.transform.position);

            // Stores the target location if it is seen.
            if (hasSight)
                lastTargetLocation = target.transform.position;

            return hasSight;
        }

        // Checks if AI has sight of a point.
        public bool HasSight(Vector3 point)
        {
            // Checks for visibility blocks.
            if (Physics.Linecast(point, transform.position + agentSightOffset, sightMask))
                return false;

            // Checks for detection range.
            if (Vector3.Distance(point, transform.position + agentSightOffset) > detectRange)
                return false;

            return true;
        }

        // Alerts close allies.
        public virtual void AlertAllies()
        {
            BaseEntityAI[] allies = FindObjectsOfType<BaseEntityAI>();

            foreach (BaseEntityAI ally in allies)
            {
                if (ally.gameObject != this.gameObject && HasSight(ally.transform.position + ally.agentSightOffset))
                    ally.Aggro();
            }
        }

        public virtual void Aggro() {   }
    }
}