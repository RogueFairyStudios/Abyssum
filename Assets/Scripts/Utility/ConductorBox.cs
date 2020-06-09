using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using DEEP.Entities;

namespace DEEP.Utility {

	[RequireComponent(typeof(BoxCollider))]
    public class ConductorBox : MonoBehaviour {

        [Tooltip("LayarMaks to get only the layer 'Conductor'.")]
        public static LayerMask layerMask;// = LayerMask.GetMask("Conductor");
        public EntityBase entity;

        protected Vector3 centerOffset;
        protected Vector3 halfExtents;

        // Set the BoxCollider to trigger and the layer to 'Conductor', to ensure that everything is right.
        private void Reset() {
            GetComponent<BoxCollider>().isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Conductor");
        }

        private void Initialize() {
            BoxCollider box = GetComponent<BoxCollider>();
            centerOffset = box.center;
            halfExtents = box.size / 2;
            if (box.attachedRigidbody != null){
                entity = box.attachedRigidbody.GetComponent<EntityBase>();
                if (entity == null) {
                    Debug.LogWarning("ERRO!!! O objeto " + name + " de " + box.attachedRigidbody.name + " não tem um EntityBase");
                }
                else {
                    entity.conductorBox = this;
                }
            }
            layerMask = LayerMask.GetMask("Conductor");
        }

        private void Start() {
            Initialize();
        }

        public List<ConductorBox> Electrify(int chainDamage) {

            List<ConductorBox> conductorsActivated = new List<ConductorBox>();
            conductorsActivated.Add(this);

# if UNITY_EDITOR
            Debug.Log("STARTING to electrify!");
# endif
            ElectrifySubroutine(conductorsActivated, chainDamage);

            return conductorsActivated;

        }

        private void ElectrifySubroutine(List<ConductorBox> conductorsActivated, int chainDamage) {

            List<ConductorBox> conductorsInContact = GetConductorsInContact();

            foreach (ConductorBox conductor in conductorsInContact) {

                // Check if the conductor hasnt been electrified yet
                if (!conductorsActivated.Contains(conductor)) {

                    conductorsActivated.Add(conductor);

                    // Recursively calls the ElectrifySubroutine method of the conductor reached
                    conductor.ElectrifySubroutine(conductorsActivated, chainDamage);

                    // Check if this conductor is attached to an entity
                    if (conductor.entity != null)
                        conductor.entity.Damage(chainDamage, DamageType.Electric);

                }
            }

        }

        /// <summary>
        /// Creates an OverlapBox to obtain all the ConductorBoxes inside it.
        /// </summary>
        /// <returns> List containing all the contactBox in contact with the box. </returns>
        public List<ConductorBox> GetConductorsInContact(){
            List<ConductorBox> conductors = new List<ConductorBox>();

            Collider[] hits = Physics.OverlapBox(transform.position + centerOffset, halfExtents, transform.rotation, layerMask);
            //Debug.Log(hits.Length + " hits detected.");

            foreach (Collider hit in hits) {
                if (hit.GetComponent<ConductorBox>() != null){
                    conductors.Add(hit.GetComponent<ConductorBox>());
                } else {
                    Debug.LogError("ERROR! The object " + name + " has an BoxCollider " +
                        "in the layer 'Conductor', but doesnt have an conductorBox attached!");
                }
                //if (hit.attachedRigidbody.GetComponent<EntityBase>() != null) {
            }

            return conductors;
        }
    }

}