using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DEEP.Entities;
using System.Collections.Generic;
using DEEP.DoorsAndKeycards;

namespace DEEP.UI
{

    public class HUDController : MonoBehaviour {



        [System.Serializable]
        protected class HealthHUD
        {
            public Slider slider = null;
            public Image sliderGraphic = null;

            public TMP_Text counter = null;

            public Image backgroundGraphic = null;

            public Color defaultColor = Color.green;
            public Color overloadColor = Color.cyan;

            public Color defaultColorBrigth = Color.green;
            public Color overloadColorBrigth = Color.cyan;
        }
        [Header("Health")]
        [SerializeField] protected HealthHUD healthHUD = null;

        [System.Serializable]
        protected class ArmorHUD
        {
            public Slider slider = null;
            public TMP_Text counter = null;
        }
        [Header("Armor")]
        [SerializeField] protected ArmorHUD armorHUD = null;

        [System.Serializable]
        protected class WeaponHUD
        {
            public Slider slider = null;
            public TMP_Text counter = null;

            public GameObject weaponAmmoPanel = null;

            public GameObject[] weaponIcons = null;
            [HideInInspector] public RectTransform[] weaponIconsTransform = null;

            public Vector2 weaponTabClosedOffset = Vector2.zero;
        }
        [Header("Ammo & Weapons")]
        [SerializeField] protected WeaponHUD weaponHUD = null;

        [System.Serializable]
        protected class KeyHUD
        {
            public GameObject blueKeyIcon = null;
            public GameObject redKeyIcon = null;
            public GameObject yellowKeyIcon = null;
        }
        [Header("Keycards")]
        [SerializeField] protected KeyHUD keyHUD = null;

        // Types of feedback, used to choose screen feedback color.
        public enum FeedbackType { Damage, Healing, Armor, Weapon, Keycard, Secret }

        [System.Serializable]
        protected class PlayerFeedback
        {

            [Tooltip("Animator used to play screen effects giving feedback to the player.")]
            public Image screenFeedback = null;
            public Coroutine screenFeedbackAnim = null; // Stores the current screen feedback coroutine.
            [Tooltip("Duration of the screen feedback.")]
            public float duration = 0.1f;
            [Tooltip("Color for the damage feedback.")]
            public Color damageFeedbackColor = Color.red;
            [Tooltip("Color for the healing feedback.")]
            public Color healingFeedbackColor = Color.green;
            [Tooltip("Color for the armor feedback.")]
            public Color armorFeedbackColor = Color.blue;
            [Tooltip("Color for the weapon/ammo feedback.")]
            public Color weaponAmmoFeedbackColor = Color.yellow;
            [Tooltip("Color for the keycard feedback.")]
            public Color keycardFeedbackColor = Color.cyan;
            [Tooltip("Color for the secret feedback.")]
            public Color secretFeedbackColor = Color.magenta;

        }
        [Header("Feedback")]
        [SerializeField] protected PlayerFeedback playerFeedback = null;

        public void SetHealthHUD(int current, int max) {

            // Sets the slider and counter if life is less than 0.
            if(current < 0) {
                healthHUD.slider.value = 0;
                healthHUD.counter.text = "-";
                return;
            }

            // Sets the counter text.
            healthHUD.counter.text = current.ToString();

            // Sets the slider and color for overload health.
            if(current > max) {
                healthHUD.slider.value = 1;
                healthHUD.backgroundGraphic.color = healthHUD.overloadColor;
                healthHUD.sliderGraphic.color = healthHUD.overloadColorBrigth;
                healthHUD.counter.color = healthHUD.overloadColorBrigth;
                return;
            }

            // Sets the slider otherwise.
            healthHUD.slider.value = (float)current / (float)max;

            // Uses the default colors.
            healthHUD.backgroundGraphic.color = healthHUD.defaultColor;
            healthHUD.sliderGraphic.color = healthHUD.defaultColorBrigth;
            healthHUD.counter.color = healthHUD.defaultColorBrigth;

            return;

        }

        public void SetArmorHUD(int current, int max) {

            if (current < 0) {
                armorHUD.slider.value = 0;
                armorHUD.counter.text = "-";
                return;
            }

            armorHUD.counter.text = current.ToString();

            armorHUD.slider.value = (float)current / (float)max;

        }

        public void SetAmmoCounter(int current, int max) {

            if(current < 0) {
                weaponHUD.counter.text = "-";
                weaponHUD.slider.value = 1;
                return;
            }

            weaponHUD.counter.text = current.ToString();

            weaponHUD.slider.value = (float)current / (float)max;

        }

        public void SetCurrentWeapon(int weapon) {

            // If the RectTransform of the weapon icons hasn't been obtained yet.
            if (weaponHUD.weaponIconsTransform == null || weaponHUD.weaponIconsTransform.Length != weaponHUD.weaponIcons.Length) {

                // Gets each weapon icon RectTransform.
                weaponHUD.weaponIconsTransform = new RectTransform[weaponHUD.weaponIcons.Length];
                for (int i = 0; i < weaponHUD.weaponIconsTransform.Length; i++)
                    weaponHUD.weaponIconsTransform[i] = weaponHUD.weaponIcons[i].GetComponent<RectTransform>();

            }

            // Enables the icons for the weapons the player has.
            for (int i = 0; i < weaponHUD.weaponIcons.Length; i++) {
                
                if(i == weapon) {
                    weaponHUD.weaponIconsTransform[i].anchoredPosition = Vector2.zero;
                    continue;
                }

                weaponHUD.weaponIconsTransform[i].anchoredPosition = weaponHUD.weaponTabClosedOffset;

            }

        }

        public void ShowWeaponIcons(bool[] weaponsEnabled) {

            bool hasAnyWeapons = false;

            // Enables the icons for the weapons the player has.
            for(int i = 0; i < weaponsEnabled.Length; i++)
            {
                weaponHUD.weaponIcons[i].SetActive(weaponsEnabled[i]);
                hasAnyWeapons = hasAnyWeapons || weaponsEnabled[i]; // Checks if the player has at least one weapon.
            }

            // Hides ammo counter if there are no weapons.
            weaponHUD.weaponAmmoPanel.SetActive(hasAnyWeapons);

        }

        public void SetKeyHUD() {

            keyHUD.blueKeyIcon.SetActive(Player.instance._keyInventory.HasKey(KeysColors.Blue));
            keyHUD.redKeyIcon.SetActive(Player.instance._keyInventory.HasKey(KeysColors.Red));
            keyHUD.yellowKeyIcon.SetActive(Player.instance._keyInventory.HasKey(KeysColors.Yellow));

        }


        // Starts a screen feedback effect.
        public void StartScreenFeedback(FeedbackType type) {

            // If a feedback effect is already happening stop it and start a new one.
            if(playerFeedback.screenFeedbackAnim != null)
                StopCoroutine(playerFeedback.screenFeedbackAnim);

            // Gets the correct color for the feedback.
            Color feedbackColor = Color.black;
            switch(type) {

                case FeedbackType.Damage:
                feedbackColor = playerFeedback.damageFeedbackColor;
                break;

                case FeedbackType.Healing:
                feedbackColor = playerFeedback.healingFeedbackColor;
                break;

                case FeedbackType.Armor:
                feedbackColor = playerFeedback.armorFeedbackColor;
                break;

                case FeedbackType.Weapon:
                feedbackColor = playerFeedback.weaponAmmoFeedbackColor;
                break;

                case FeedbackType.Keycard:
                feedbackColor = playerFeedback.keycardFeedbackColor;
                break;

                case FeedbackType.Secret:
                feedbackColor = playerFeedback.secretFeedbackColor;
                break;

            }

            playerFeedback.screenFeedbackAnim = StartCoroutine(ScreenFeedbackAnim(feedbackColor, playerFeedback.duration));

        }

        protected IEnumerator ScreenFeedbackAnim(Color color, float duration) {

            // Sets the feedback color and show it.
            playerFeedback.screenFeedback.color = color;
            playerFeedback.screenFeedback.enabled = true;

            // Waits for the duration.
            yield return new WaitForSeconds(duration);

            // Ends the feedback.
            playerFeedback.screenFeedback.enabled = false;
            playerFeedback.screenFeedbackAnim = null;

        }

    }

}
