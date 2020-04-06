using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DEEP.UI
{

    public class HUDController : MonoBehaviour {

        [SerializeField] private TMP_Text healthCounter = null;
        [SerializeField] private TMP_Text armorCounter = null;
        [SerializeField] private TMP_Text ammoCounter = null;
        [SerializeField] private GameObject ammoCounterPanel = null;

        [SerializeField] private Image ammoIcon = null;

        [SerializeField] private GameObject[] weaponIcons = null;

        [System.Serializable]
        public class PlayerFeedback
        {

            // Types of feedback, used to choose screen feedback color.
            public enum Type { Damage, Healing, Armor, Weapon, Keycard, Secret }

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
        [SerializeField] private PlayerFeedback playerFeedback = null;

        public void SetHealthCounter(int value) {

            if(value < 0) {
                healthCounter.text = "---";
                return;
            }
        
            string valueStr = value.ToString();
            healthCounter.text = valueStr.PadLeft(3, '0');

        }

        public void SetArmorCounter(int value) {

            if(value < 0) {
                armorCounter.text = "---";
                return;
            }

            string valueStr = value.ToString();
            armorCounter.text = valueStr.PadLeft(3, '0');

        }

        public void SetAmmoCounter(int value) {

            if(value < 0) {
                ammoCounter.text = "---";
                return;
            }

            string valueStr = value.ToString();
            ammoCounter.text = valueStr.PadLeft(3, '0');

        }

        public void SetAmmoIcon(Sprite icon) {

            ammoIcon.sprite = icon;

        }

        public void ShowWeaponIcons(bool[] weaponsEnabled) {


            bool hasAnyWeapons = false;

            // Enables the icons for the weapons the player has.
            for(int i = 0; i < weaponsEnabled.Length; i++)
            {
                weaponIcons[i].SetActive(weaponsEnabled[i]);
                hasAnyWeapons = hasAnyWeapons || weaponsEnabled[i]; // Checks if the player has at least one weapon.
            }

            // HIdes ammo counter if there are no weapons.
            ammoCounterPanel.SetActive(hasAnyWeapons);

        }

        // Starts a screen feedback effect.
        public void StartScreenFeedback(PlayerFeedback.Type type) {

            // If a feedback effect is already happening stop it and start a new one.
            if(playerFeedback.screenFeedbackAnim != null)
                StopCoroutine(playerFeedback.screenFeedbackAnim);

            // Gets the correct color for the feedback.
            Color feedbackColor = Color.black;
            switch(type) {

                case PlayerFeedback.Type.Damage:
                feedbackColor = playerFeedback.damageFeedbackColor;
                break;

                case PlayerFeedback.Type.Healing:
                feedbackColor = playerFeedback.healingFeedbackColor;
                break;

                case PlayerFeedback.Type.Armor:
                feedbackColor = playerFeedback.armorFeedbackColor;
                break;

                case PlayerFeedback.Type.Weapon:
                feedbackColor = playerFeedback.weaponAmmoFeedbackColor;
                break;

                case PlayerFeedback.Type.Keycard:
                feedbackColor = playerFeedback.keycardFeedbackColor;
                break;

                case PlayerFeedback.Type.Secret:
                feedbackColor = playerFeedback.secretFeedbackColor;
                break;

            }

            playerFeedback.screenFeedbackAnim = StartCoroutine(ScreenFeedbackAnim(feedbackColor, playerFeedback.duration));

        }

        private IEnumerator ScreenFeedbackAnim(Color color, float duration) {

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
