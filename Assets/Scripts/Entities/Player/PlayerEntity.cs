using System;

using UnityEngine;

using DEEP.HUD;

namespace DEEP.Entities.Player
{

    // ========================================================================================================================
    // Entity class for the player.
    // ========================================================================================================================
    public class PlayerEntity : EntityBase
    {
    
        // PlayerController that owns this script.
        protected PlayerController ownerPlayer;
        public PlayerController Owner {
            get { return ownerPlayer; }
            set { ownerPlayer = value; }
        }

        [Header("Armor")] // ==================================================================================================

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
            OnChangeArmor(0, 0);

        }

        // Does damage to the player.
        public override void Damage(int amount, DamageType type) {

            // Saves the old value.
            int prevHealth = CurrentHealth();

            // Does damage to the armor first, them does the remaining damage to health.
            int healthDamage = amount - DamageArmor(amount, type);

            // Decreases the health.
            health -= healthDamage;

            // Handles any changes that have to be made when modifying health.
            OnChangeHealth(prevHealth, CurrentHealth());

        }

        // Does damage to the armor, returns the amount of damage remaining.
        protected virtual int DamageArmor(int amount, DamageType type) {

            // Saves the old value.
            int prevArmor = CurrentArmor();

            // If damage ignores armor, returns 0.
            if (type == DamageType.IgnoreArmor) return 0;

            // Calculates the percent of damage that should be absorbed by armor.
            float armorAbsorption = Mathf.Clamp(armor / maxArmor, minArmorAbsorption, 1f);

            // Calculates the amount of damage to armor.
            // Clamps the result to ensure if armor breaks the remaining damage will go to health.
            int armorDamage = Mathf.Clamp((int)Math.Round(armorAbsorption * amount), 0, armor); 

            // Damages the armor.
            armor -= armorDamage;

            // Handles any changes that have to be made when modifying armor.
            OnChangeArmor(prevArmor, CurrentArmor());

            return armorDamage;

        }

        // Kills the player.
        protected override void Die() {
            Debug.Log("Player died!");     
            ownerPlayer.Die();
        }

        // Heals player health.
        public virtual bool Heal(int amount, HealType type, AudioClip feedbackAudio) 
        {

            // Tries to heal the entity.
            bool healed = base.Heal(amount, type);

            // If the entity was healed plays the player feedback sound.
            if(healed && feedbackAudio != null)
                ownerPlayer.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            return healed;

        }

        // Returns the current armor of the player.
        public int CurrentArmor() { return armor; }

        // Give armor to the player.
        public virtual bool GiveArmor(int amount, AudioClip feedbackAudio) 
        {

            // Saves the old value.
            int prevArmor = CurrentArmor();

            // Checks if armor is not maxed out.
            if(armor >= maxArmor) return false;

            armor += amount; // Adds the armor.

            // Ensures not going above max armor.
            if(armor > maxArmor) armor = maxArmor;

            // Plays the player feedback sound.
            if(feedbackAudio != null)
                ownerPlayer.feedbackAudioSource.PlayOneShot(feedbackAudio, 1.0f);

            // Updates the armor counter on the HUD.
            OnChangeArmor(prevArmor, CurrentArmor());

            return true;

        }

        protected override void OnChangeHealth(int oldValue, int newValue) 
        {

            // Plays the damage screen feedback when health decreases.
            if(newValue < oldValue)
                ownerPlayer.HUD.Feedback.StartScreenFeedback(FeedbackType.Damage);

            ownerPlayer.HUD.Health.SetValue(health, maxHealth);

            base.OnChangeHealth(oldValue, newValue);

        }

        protected virtual void OnChangeArmor(int oldValue, int newValue) { 

            // Plays the damage screen feedback when armor decreases.
            if(newValue < oldValue)
                ownerPlayer.HUD.Feedback.StartScreenFeedback(FeedbackType.Damage);

            ownerPlayer.HUD.Armor.SetValue(armor, maxArmor); 

        }

        public override void SetSlow() { ownerPlayer.Movementation.SetSlow(); }

        public override void SetBaseSpeed() { ownerPlayer.Movementation.SetBaseSpeed(); }

    }

}