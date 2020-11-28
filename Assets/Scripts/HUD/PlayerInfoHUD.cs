using TMPro;
using UnityEngine;
using UnityEngine.UI;

using DEEP.Stage;
using DEEP.DoorsAndKeycards;

namespace DEEP.HUD {

    public class PlayerInfoHUD : MonoBehaviour {

        [System.Serializable] // Health =======================================================================================
        public class HealthHUD
        {

            [SerializeField] protected TMP_Text counter = null;

            [SerializeField] protected TMP_Text counterTitle = null;

            [SerializeField] protected Color defaultColor = Color.green;
            [SerializeField] protected Color overloadColor = Color.cyan;

            public void SetValue(int current, int max) {

                // Sets the slider and counter if life is less than 0.
                if(current < 0) {
                    counter.text = "-";
                    return;
                }

                // Sets the counter text.
                counter.text = current.ToString();

                // Sets the slider and color for overload health.
                if(current > max) {
                    counter.color = overloadColor;
                    counterTitle.color = overloadColor;
                    return;
                }

                // Uses the default colors.
                counter.color = defaultColor;
                counterTitle.color = defaultColor;

                return;

            }

        }
        public HealthHUD health = null;

        [System.Serializable] // Armor ========================================================================================
        public class ArmorHUD
        {
            [SerializeField] protected TMP_Text counter = null;

            public void SetValue(int current, int max) {

                if (current < 0) {
                    counter.text = "-";
                    return;
                }

                counter.text = current.ToString();

            }
        }
        public ArmorHUD armor = null;

        [System.Serializable] // Ammo & Weapons ===============================================================================
        public class AmmoAndWeaponHUD 
        {

            [SerializeField] protected TMP_Text counter = null;

            [SerializeField] protected GameObject weaponPanel = null;

            [SerializeField] protected TMP_Text[] weaponNumbers = null;

            [SerializeField] protected Image weaponIcon = null;
            [SerializeField] protected Image ammoIcon = null;

            [SerializeField] protected Color activeColor = Color.yellow;
            [SerializeField] protected Color inactiveColor = Color.gray;

            // Sets the ammo counter.
            public void SetAmmo(int current, int max) {

                if(current < 0) {
                    counter.text = "-";
                    return;
                }

                counter.text = current.ToString();

            }

            // Sets the current weapon on the HUD.
            public void SetCurrentWeapon(int weapon, Sprite weaponIcon, Sprite ammoIcon) {

                // Colors the weapon numbers correctly based on the active weapon.
                for(int i = 0; i < weaponNumbers.Length; i++)
                        weaponNumbers[i].color = inactiveColor; // Sets all numbers inactive.
                weaponNumbers[weapon].color = activeColor; // Sets the current weapon active.

                // Sets the correct weapons and ammo icons.
                this.weaponIcon.sprite = weaponIcon;
                this.ammoIcon.sprite = ammoIcon;

            }

            // Sets the numbers for the weapons to indicate which ones the player has.
            public void SetWeaponNumbers(bool[] weaponsEnabled) {

                bool hasAnyWeapons = false;

                // Enables the numbers for the weapons the player has.
                for(int i = 0; i < weaponsEnabled.Length; i++)
                {
                    weaponNumbers[i].gameObject.SetActive(weaponsEnabled[i]);
                    hasAnyWeapons = hasAnyWeapons || weaponsEnabled[i]; // Checks if the player has at least one weapon.
                }

                // Hides ammo counter if there are no weapons.
                weaponPanel.SetActive(hasAnyWeapons);

            }

        }
        public AmmoAndWeaponHUD ammoAndWeapons = null;

        [System.Serializable] // Keycards =====================================================================================
        public class KeyHUD 
        {
            [SerializeField] protected Image blueKeyIcon = null;
            [SerializeField] protected Image redKeyIcon = null;
            [SerializeField] protected Image yellowKeyIcon = null;

            public void UpdateValues(KeyInventory inventory) {

                blueKeyIcon.enabled     =   inventory.HasKey(KeysColors.Blue);
                redKeyIcon.enabled      =   inventory.HasKey(KeysColors.Red);
                yellowKeyIcon.enabled   =   inventory.HasKey(KeysColors.Yellow);

            }

        }
        public KeyHUD keycards = null;

        [System.Serializable] // Log ==========================================================================================
        public class LogHUD
        {
            [SerializeField] protected TMP_Text text = null;
            [SerializeField] protected Image icon = null;

            public void Message(string message, Sprite icon, Color color) {

                text.color = color;
                text.text = (StageManager.Instance != null) ? StageManager.GetDurationString(StageManager.Instance.GetDuration()) + ": " + message : "";

                this.icon.color = color;
                this.icon.sprite = icon;

            }
        }
        public LogHUD Log = null;

    }

}