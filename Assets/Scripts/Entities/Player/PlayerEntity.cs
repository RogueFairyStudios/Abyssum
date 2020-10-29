using System;

using UnityEngine;

namespace DEEP.Entities.Player
{

    // Class that controls the Player.
    public class PlayerEntity : EntityBase
    {
        
        [Header("Armor")] // ==============================================================================

        [Tooltip("Players's current armor.")]
        [SerializeField] protected int armor = 0;

        [Tooltip("Players's max armor.")]
        [SerializeField] protected int maxArmor = 100;

        [Tooltip("What's the minimum percentage of damage that armor will absorb.")]
        [Range(0.0f, 1.0f)]
        [SerializeField] protected float minArmorAbsorption = 0.3f;

        protected override void Start()
        {

            base.Start();

            armor = 0; // Sets the initial armor to the maximum armor.
            OnChangeArmor();

        }

        public override void Damage(int amount, DamageType type) {

            // Initially all damage is to the health.
            int healthDamage = amount;

            // Calculates armor damage absorption.
            if (type != DamageType.IgnoreArmor) {

                // Calculates the percent of damage that should be absorbed by armor.
                float armorAbsorption = Mathf.Clamp(armor / maxArmor, minArmorAbsorption, 1f);

                // Calculates the amount of damage to armor.
                int armorDamage = Mathf.Clamp((int)Math.Round(armorAbsorption * amount), 0, armor); // Clamps to ensure if armor breaks the remaining damage will go to health.

                // Damages the armor.
                armor -= armorDamage;

                // Handles any changes that have to be made when modifying armor.
                OnChangeArmor();

                // Decreases the damage to health by the amount of damage that went to the armor instead.
                healthDamage -= armorDamage;

            }

            // Decreases the health.
            health -= healthDamage;

            // Handles any changes that have to be made when modifying health.
            OnChangeHealth();

            PlayerController.Instance.HUD.StartScreenFeedback(UI.HUDController.FeedbackType.Damage);

        }

        protected override void Die() {

            Debug.Log("Player died!");
            
            PlayerController.Instance.Die();

        }

        public virtual bool Heal(int amount, HealType type, AudioClip feedbackAudio) 
        {

            // Tries to heal the entity.
            bool healed = base.Heal(amount, type);

            // If the entity was healed plays the player feedback sound.
            if(healed && feedbackAudio != null)
                PlayerController.Instance.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            return healed;

        }

        // Give armor to the player.
        public virtual bool GiveArmor(int amount, AudioClip feedbackAudio) 
        {

            // Checks if armor is not maxed out.
            if(armor >= maxArmor) return false;

            armor += amount; // Adds the armor.

            // Ensures not going above max armor.
            if(armor > maxArmor) armor = maxArmor;

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                PlayerController.Instance.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Updates the armor counter on the HUD.
            OnChangeArmor();

            return true;

        }

        private void OnChangeArmor() { PlayerController.Instance.HUD.armor.SetValue(armor, maxArmor); }

        protected override void OnChangeHealth() {

            PlayerController.Instance.HUD.health.SetValue(health, maxHealth);

            base.OnChangeHealth();

        }

        public override void SetSlow() { PlayerController.Instance.movementation.SetSlow(); }

        public override void SetBaseSpeed() { PlayerController.Instance.movementation.SetBaseSpeed(); }

    }

}