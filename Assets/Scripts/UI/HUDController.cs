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

    }

}
