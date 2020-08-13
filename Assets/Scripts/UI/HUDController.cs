using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DEEP.Stage;
using DEEP.Entities;
using DEEP.DoorsAndKeycards;

namespace DEEP.UI
{

    public class HUDController : MonoBehaviour {

        [System.Serializable]
        protected class HealthHUD
        {

            public TMP_Text counter = null;

            public TMP_Text counterTitle = null;

            public Color defaultColor = Color.green;
            public Color overloadColor = Color.cyan;

        }
        [Header("Health")]
        [SerializeField] protected HealthHUD healthHUD = null;

        [System.Serializable]
        protected class ArmorHUD
        {
            public TMP_Text counter = null;
        }
        [Header("Armor")]
        [SerializeField] protected ArmorHUD armorHUD = null;

        [System.Serializable]
        protected class WeaponHUD
        {

            public TMP_Text counter = null;

            public GameObject weaponPanel = null;

            public TMP_Text[] weaponNumbers = null;

            public Image weaponIcon = null;
            public Image ammoIcon = null;

            public Color activeColor = Color.yellow;
            public Color inactiveColor = Color.gray;

        }
        [Header("Ammo & Weapons")]
        [SerializeField] protected WeaponHUD weaponHUD = null;

        [System.Serializable]
        protected class KeyHUD
        {
            public Image blueKeyIcon = null;
            public Image redKeyIcon = null;
            public Image yellowKeyIcon = null;
        }
        [Header("Keycards")]
        [SerializeField] protected KeyHUD keyHUD = null;

        [System.Serializable]
        protected class LogHUD
        {
            public TMP_Text text = null;
            public Image icon = null;
        }
        [Header("Log")]
        [SerializeField] protected LogHUD logHUD = null;

        [System.Serializable]
        protected class SpeedrunHUD
        {
            public TMP_Text kills = null;
            public TMP_Text items = null;
            public TMP_Text secrets = null;
            public TMP_Text time = null;
        }
        [Header("Speedrun")]
        [SerializeField] protected SpeedrunHUD speedrunHUD = null;

        // Types of feedback, used to choose screen feedback color.
        public enum FeedbackType { Damage, Toxic, Mud }

        [System.Serializable]
        protected class PlayerFeedback
        {

            [Tooltip("Animator used to play screen effects giving feedback to the player.")]
            public Image screenFeedback = null;
            public Coroutine screenFeedbackAnim = null; // Stores the current screen feedback coroutine.
            [Tooltip("Duration of screen flash feedbacks.")]
            public float duration = 0.1f;
            [Tooltip("Color for the damage feedback.")]
            public Color damageFeedbackColor = Color.red;
            [Tooltip("Color for the toxic feedback.")]
            public Color toxicFeedbackColor = Color.green;
            [Tooltip("Color for the mud feedback.")]
            public Color mudFeedbackColor = Color.green;
            [HideInInspector] public bool constantFeedbackActive;
            [HideInInspector] public Color currentConstantFeedbackColor;

        }
        [Header("Feedback")]
        [SerializeField] protected PlayerFeedback playerFeedback = null;

        void FixedUpdate() {

            // Constantly updates the speedrun clock.
            speedrunHUD.time.text = StageInfo.Instance.GetDurationString();

        }


        public void SetHealthHUD(int current, int max) {

            // Sets the slider and counter if life is less than 0.
            if(current < 0) {
                healthHUD.counter.text = "-";
                return;
            }

            // Sets the counter text.
            healthHUD.counter.text = current.ToString();

            // Sets the slider and color for overload health.
            if(current > max) {
                healthHUD.counter.color = healthHUD.overloadColor;
                healthHUD.counterTitle.color = healthHUD.overloadColor;
                return;
            }

            // Uses the default colors.
            healthHUD.counter.color = healthHUD.defaultColor;
            healthHUD.counterTitle.color = healthHUD.defaultColor;

            return;

        }

        public void SetArmorHUD(int current, int max) {

            if (current < 0) {
                armorHUD.counter.text = "-";
                return;
            }

            armorHUD.counter.text = current.ToString();

        }

        public void SetAmmoCounter(int current, int max) {

            if(current < 0) {
                weaponHUD.counter.text = "-";
                return;
            }

            weaponHUD.counter.text = current.ToString();

        }

        public void SetCurrentWeapon(int weapon, Sprite weaponIcon, Sprite ammoIcon) {

            // Colors the weapon numbers correctly based on the active weapon.
            for(int i = 0; i < weaponHUD.weaponNumbers.Length; i++)
                    weaponHUD.weaponNumbers[i].color = weaponHUD.inactiveColor; // Sets all numbers inactive.
            weaponHUD.weaponNumbers[weapon].color = weaponHUD.activeColor; // Sets the current weapon active.

            // Sets the correct weapons and ammo icons.
            weaponHUD.weaponIcon.sprite = weaponIcon;
            weaponHUD.ammoIcon.sprite = ammoIcon;

        }

        public void ShowWeaponIcons(bool[] weaponsEnabled) {

            bool hasAnyWeapons = false;

            // Enables the numbers for the weapons the player has.
            for(int i = 0; i < weaponsEnabled.Length; i++)
            {
                weaponHUD.weaponNumbers[i].gameObject.SetActive(weaponsEnabled[i]);
                hasAnyWeapons = hasAnyWeapons || weaponsEnabled[i]; // Checks if the player has at least one weapon.
            }

            // Hides ammo counter if there are no weapons.
            weaponHUD.weaponPanel.SetActive(hasAnyWeapons);

        }

        public void SetKeyHUD() {

            keyHUD.blueKeyIcon.enabled = Player.Instance.keyInventory.HasKey(KeysColors.Blue);
            keyHUD.redKeyIcon.enabled = Player.Instance.keyInventory.HasKey(KeysColors.Red);
            keyHUD.yellowKeyIcon.enabled = Player.Instance.keyInventory.HasKey(KeysColors.Yellow);

        }

        public void Log(string message, Sprite icon, Color color) {

            logHUD.text.color = color;
            logHUD.text.text = StageInfo.Instance.GetDurationString() + ": " + message;

            logHUD.icon.color = color;
            logHUD.icon.sprite = icon;

        }

        public void SetKillCount(int current, int total) {
            speedrunHUD.kills.text = current + "/" + total;
        }

        public void SetItemCount(int current, int total) {
            speedrunHUD.items.text = current + "/" + total;
        }

        public void SetSecretCount(int current, int total) {
            speedrunHUD.secrets.text = current + "/" + total;
        }

        // Starts a screen feedback effect.
        public void StartScreenFeedback(FeedbackType type) {

            // If a feedback effect is already happening stop it and start a new one.
            if(playerFeedback.screenFeedbackAnim != null)
                StopCoroutine(playerFeedback.screenFeedbackAnim);

            // Gets the correct color for the feedback.
            Color feedbackColor = Color.black;
            switch(type) {

                // Quick flash effects.
                case FeedbackType.Damage:
                feedbackColor = playerFeedback.damageFeedbackColor;
                playerFeedback.screenFeedbackAnim = StartCoroutine(ScreenFeedbackAnim(feedbackColor, playerFeedback.duration));
                break;

                // Constant effects.
                case FeedbackType.Toxic:
                feedbackColor = playerFeedback.toxicFeedbackColor;
                StartConstantScreenFeedback(feedbackColor);
                break;

                case FeedbackType.Mud:
                feedbackColor = playerFeedback.mudFeedbackColor;
                StartConstantScreenFeedback(feedbackColor);
                break;

            }

            
        }

        protected IEnumerator ScreenFeedbackAnim(Color color, float duration) {

            // Sets the feedback color and show it.
            playerFeedback.screenFeedback.color = color;
            playerFeedback.screenFeedback.enabled = true;

            // Waits for the duration.
            yield return new WaitForSeconds(duration);

            // If there are no constant feedbacks active
            if(!playerFeedback.constantFeedbackActive)
            {
                // Ends the feedback
                playerFeedback.screenFeedback.enabled = false;
            }
            else
            {
                // Turns the feedback back to the constant color
                playerFeedback.screenFeedback.color = playerFeedback.currentConstantFeedbackColor;
            }

            playerFeedback.screenFeedbackAnim = null;
        }

        protected void StartConstantScreenFeedback(Color color)
        {
            // Sets the feedback color and shows it
            playerFeedback.screenFeedback.color = color;
            playerFeedback.screenFeedback.enabled = true;

            // Sets up flags
            playerFeedback.constantFeedbackActive = true;                
            playerFeedback.currentConstantFeedbackColor = color;
        }

        public void StopConstantScreenFeedback()
        {
            // Sets up flags
            playerFeedback.constantFeedbackActive = false;

            // If there are no other feedbacks going on, turn the feedback image off
            if(playerFeedback.screenFeedbackAnim == null)
                playerFeedback.screenFeedback.enabled = false;
        }

    }

}
