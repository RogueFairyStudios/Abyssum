using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using DEEP.Utility;
using DEEP.Entities;
using UnityEditor.Experimental.GraphView;

namespace DEEP.Weapons
{

    // Base script for a simple weapons that fires common bullets.
    public class TeslaWeapon : Shotgun
    {

        [Tooltip("Base damage, dealt to the main entity when hit.")]
        [SerializeField] private int baseDamage = 10;

        [Tooltip("Damage dealt to other entities in contact with a conductor.")]
        [SerializeField] private int chainDamage = 8;

        [Tooltip("LayerMask used for the fiting raycast.")]
        [SerializeField] private LayerMask firingLayerMask = new LayerMask();

        [Tooltip("LineRenderers used for the shooting effect, one per pellet.")]
        [SerializeField] private LineRenderer[] lineRenderers = null;

        [Tooltip("The time the visual effect is on.")]
        [SerializeField] private float visualEffectDuration = 0.1f;

        [Tooltip("Offset where the effect should be placed (Used because lots of entitys have their bases on their feet).")]
        [SerializeField] private Vector3 visualEffectOffset = new Vector3(0, 0.5f, 0);

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        protected void OnEnable() {

            // Disables the effects.
            foreach (LineRenderer lineRender in lineRenderers)
                lineRender.positionCount = 0;

        }

        protected override void Fire() {

            delayTimer = 0; // Resets the delay.

            // Plays the animation.
            if (wAnimator != null) {
                wAnimator.SetBool("Fire", true);
                wAnimator.SetBool("NoAmmo", false);
            }

            // Plays the audio.
            if (wAudio != null) {
                wAudio.clip = shotClip;
                wAudio.Play();
            }

            // Fires all "pellets".
            for (int i = 0; i < pelletsPerShot; i++) {
                if (i == 0) // Ensures the first shot is always forward.
                    ShootRay(i, bulletSpawn.rotation);
                else
                    ShootRay(i, GenerateRandomRotation(bulletSpawn.rotation));
            }

            }

        private void ShootRay(int index, Quaternion direction) {

            // Shoots the ray.
            RaycastHit hitInfo;

            if (Physics.Raycast(bulletSpawn.position, direction * Vector3.forward, out hitInfo, 1000.0f, firingLayerMask)) {

                // List of conductors activated by this shot.
                List<ConductorBox> conductorsActivated = null;

                // Tries to get an entity component from the object.
                EntityBase entity = null;
                if (hitInfo.rigidbody) // Searches for the EntityBase on the object with the Rigidbody.
                    entity = hitInfo.rigidbody.GetComponent<EntityBase>();
                if (entity != null) {

                    // Spawn the blood splatter effect if avaliable and hit a player or enemy.
                    if (entity.bloodEffect != null)
                        Instantiate(entity.bloodEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                    if (entity.conductorBox != null)
                        conductorsActivated = entity.conductorBox.Electrify(chainDamage);

                    // Does the damage.
                    entity.Damage(baseDamage, 0);

                }

                // Check if the object collided have an conductor box and doesnt have any entity attached to.
                ConductorBox conductorBox = hitInfo.collider.GetComponent<ConductorBox>();
                if (hitInfo.collider.isTrigger && !(conductorBox == null || conductorBox.entity != null)) {

                    // If those requisites are satisfied, then this object must be an conductor (water). So, Electrify!
                    conductorsActivated = conductorBox.Electrify(chainDamage);

                }

                // Does the visual effect.
                lineRenderers[index].positionCount = 2;
                lineRenderers[index].SetPosition(0, bulletSpawn.transform.position);
                lineRenderers[index].SetPosition(1, hitInfo.point);

                StartCoroutine(TimedVisualEffect(index, conductorsActivated));

            }
            else { // If nothing was hit does the visual effect using a far away point.

                lineRenderers[index].positionCount = 2;
                lineRenderers[index].SetPositions(new Vector3[] { bulletSpawn.transform.position, bulletSpawn.transform.position + bulletSpawn.transform.forward * 1000.0f });

                StartCoroutine(TimedVisualEffect(index, null));

            }

        }

        private IEnumerator TimedVisualEffect(int index, List<ConductorBox> conductors) {

            // Adds the activated conductors to the effect.
            if (conductors != null) {

                lineRenderers[index].positionCount += conductors.Count;

                for(int i = 2; i < lineRenderers[index].positionCount; i++)
                    lineRenderers[index].SetPosition(i, conductors[i - 2].transform.position + visualEffectOffset);

            }

            // Waits for the duration.
            float time = 0.0f;
            while(time < visualEffectDuration) {

                // Updates the start position when the effect is on.
                lineRenderers[index].SetPosition(0, bulletSpawn.transform.position);

                time += Time.fixedDeltaTime;
                yield return waitForFixed;

            }

            // Disables the effect.
            lineRenderers[index].positionCount = 0;

        }

    }

}