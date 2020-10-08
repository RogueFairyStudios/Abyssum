using UnityEngine;

using DEEP.Pooling;
using DEEP.Entities;
using DEEP.Entities.Player;

using System.Collections;
using System.Collections.Generic;

namespace DEEP.Weapons {

    public class TridentWeapon : SimpleWeapon
    {

        // PlayerController reference =========================================================================================
        private PlayerController controller;
        // Returns a reference for the PlayerController instance, 
        // tries getting it if it's not yet available.
        protected PlayerController rController {
            get { 
                if(controller != null)
                    return controller; 
                else {
                    controller = FindObjectOfType<PlayerController>();
                    return controller;
                }
            } 
        }
        // ====================================================================================================================

        [Tooltip("Amount of health used as \"tribute\" when shooting.")]
        [SerializeField] protected int healthTribute = 50;

        [Tooltip("Post-processing volume game object for the effects.")]
        [SerializeField] protected GameObject postProcessingVolume = null;

        [Tooltip("The time this weapon will be in shooting state.")]
        [SerializeField] protected float shootingDuration = 4.0f;

        [Tooltip("How many times this weapon should perform an attack on shooting stage.")]
        [SerializeField] protected int numBursts = 12;

        [Tooltip("LayerMask for the burst's visibility check.")]
        [SerializeField] protected LayerMask burstVisibilityMask = new LayerMask();

        [Tooltip("LineRenderer used for the shooting effect.")]
        [SerializeField] private LineRenderer lineRenderer = null;

        [Tooltip("The time the visual effect is on.")]
        [SerializeField] private float visualEffectDuration = 0.1f;

        [Tooltip("Offset where the effect should be placed (Used because lots of entitys have their bases on their feet).")]
        [SerializeField] private Vector3 visualEffectOffset = new Vector3(0, 0.5f, 0);

        // Object used to wait in coroutines.
        private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

        protected void OnEnable() {

            // Ensures effects are off at start.
            postProcessingVolume.SetActive(false);

            // Sets the no ammo animations.
            if (wAnimator != null) {
                if (!ammoSource.HasAmmo(1))
                    wAnimator.SetBool("NoAmmo", true);
            }

        }

        protected override void Fire() 
        {

            // Also uses player health as "ammo" for the weapon.
            PlayerEntity pEntity = rController.Entity;
            if (pEntity.CurrentHealth() > healthTribute)
                pEntity.Damage(healthTribute, DamageType.IgnoreArmor);
            else // If the player has less health than the tribute amount, fires but leaves it with 1 health.
                pEntity.Damage(pEntity.CurrentHealth() - 1, DamageType.IgnoreArmor);

            delayTimer = 0; // Resets the delay.

            // Plays the animation.
            if (wAnimator != null)  wAnimator.SetBool("Fire", true);

            // Plays the audio.
            if (wAudio != null) {
                wAudio.clip = shotClip;
                wAudio.Play();
            }

            // Enables effects.
            postProcessingVolume.SetActive(true);

            // Sets the no ammo animations.
            if (!ammoSource.HasAmmo(1))  wAnimator.SetBool("NoAmmo", true);

        }

        // Called by the animator when entering the shooting animation.
        public void OnShotAnimation() {

            StartCoroutine(Shooting());

        }

        // Shoots lightning beams on nearby entities.
        protected IEnumerator Shooting() {

            

            // Fires the weapon bursts.
            float time = 0.0f;
            int curBurst = 1;
            while(time < shootingDuration) {

                if(time > curBurst * (shootingDuration / numBursts)) {

                    StartCoroutine(Burst());
                    curBurst++;

                }

                time += Time.fixedDeltaTime;
                yield return waitForFixed;

            }

            // Disables the shooting animation.
            wAnimator.SetBool("Fire", false);

        }

        protected IEnumerator Burst() {

            // Gets a list of all entities.
            EntityBase[] allEntities = FindObjectsOfType<EntityBase>();

            // Gets all valid targets from the list.
            List<Transform> targets = new List<Transform>();
            for (int i = 0; i < allEntities.Length; i++) {

                // Gets the owner of this weapon to make a check later.
                PlayerWeaponController ownerWeaponController = FindObjectOfType<PlayerWeaponController>();

                // Checks that the entity exists and is not the player.
                if(allEntities[i] != null) {

                    // Ensures the entity is not the Player that fired the trident.
                    if(ownerWeaponController.Owner.Entity == allEntities[i])
                        continue;

                    // Checks that the entity is visible.
                    if(!Physics.Linecast(bulletSpawn.position, allEntities[i].transform.position, burstVisibilityMask)) {

                        targets.Add(allEntities[i].transform);

                    }

                }

            }

            // Fires at a random avaliable target, if there is any.
            if (targets.Count > 0) {

                // Gets the position of a random target.
                Vector3 randomTargetPos = targets[UnityEngine.Random.Range(0, targets.Count)].position;

                // Instantiates the attack at the target.
                PoolingSystem.Instance.PoolObject(bulletPrefab, randomTargetPos, new Quaternion());

                // Does the visual effect.
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new Vector3[] { bulletSpawn.transform.position, randomTargetPos + visualEffectOffset });

                // Waits for the duration.
                float time = 0.0f;
                while (time < visualEffectDuration) {

                    // Updates the start position when the effect is on.
                    lineRenderer.SetPosition(0, bulletSpawn.transform.position);

                    time += Time.fixedDeltaTime;
                    yield return waitForFixed;

                }

                // Disables the effect.
                lineRenderer.positionCount = 0;

            }

        }

        public void OnStopShotAnimation() {

            // Disables effects.
            postProcessingVolume.SetActive(false);

        }

    }

}