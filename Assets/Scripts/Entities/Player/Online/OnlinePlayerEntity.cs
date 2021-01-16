using UnityEngine;

using DEEP.HUD;
using DEEP.Entities;
using DEEP.Entities.Player;

namespace DEEP.Online.Entities.Player
{

    // Class that controls the Player.
    public class OnlinePlayerEntity : PlayerEntity
    {
    
        protected override void Start()
        {

            // Entity is handled by the server.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                return;

            // Initializes health and armor and syncs through the network.
            health = maxHealth;
            (Owner as OnlinePlayerController).Sync.SyncHealth(health, health);
            armor = 0;
            (Owner as OnlinePlayerController).Sync.SyncArmor(armor, armor);

            isDead = false;

        }

        public override void Damage(int amount, DamageType type) {

            // Damage is handled by the server.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                return;

            // Calls base function.
            base.Damage(amount, type);

        }

        // Asks for health to be synced between server and client.
        protected override void OnChangeHealth(int oldValue, int newValue) {
            if(oldValue != newValue)
                (Owner as OnlinePlayerController).Sync.SyncHealth(oldValue, newValue);
        }

        // Changes the health on the client after a sync from the server.
        public virtual void OnChangeHealthClient(int oldValue, int newValue) {

            // Plays the damage screen feedback when health decreases.
            if(newValue < oldValue)
                ownerPlayer.HUD.Feedback.StartScreenFeedback(FeedbackType.Damage);

            ownerPlayer.HUD.Health.SetValue(newValue, maxHealth);

        }

        // Asks for armor to be synced between server and client.
        protected override void OnChangeArmor(int oldValue, int newValue) { 
            if(oldValue != newValue)
                (Owner as OnlinePlayerController).Sync.SyncArmor(oldValue, newValue);
        }

        // Changes the armor on the client after a sync from the server.
        public virtual void OnChangeArmorClient(int oldValue, int newValue) {

            // Plays the damage screen feedback when armor decreases.
            if(newValue < oldValue)
                ownerPlayer.HUD.Feedback.StartScreenFeedback(FeedbackType.Damage);

            ownerPlayer.HUD.Armor.SetValue(newValue, maxArmor); 

        }


        protected override void Die() {

            Debug.Log("Player died!");
            // TODO: kill player in server.

        }

        public override bool Heal(int amount, HealType type, AudioClip feedbackAudio) 
        {

            // Healing is handled by the server.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                return false;

            // Removes the feedback audio before calling the base function.
            bool healed = base.Heal(amount, type, null);

            // Plays the feedback audio.
            // TODO: Play feedback audio for correct player.

            return healed;

        }

        // Give armor to the player.
        public override bool GiveArmor(int amount, AudioClip feedbackAudio) 
        {

            // Giving armor is handled by the server.
            if(!(Owner as OnlinePlayerController).Identity.isServer)
                return false;

            // Removes the feedback audio before calling the base function.
            bool receivedArmor = base.GiveArmor(amount, null);

            // Plays the feedback audio.
            // TODO: Play feedback audio for correct player.

            return receivedArmor;

        }

        public override void SetSlow() { 

            // TODO: Set speed and syncs to client 

        }

        public override void SetBaseSpeed() { 
            
            // TODO: Set speed and syncs to client 
        }

    }

}