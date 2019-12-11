using UnityEngine;
using TMPro;

namespace DEEP.UI
{

    public class HUDController : MonoBehaviour {

        [SerializeField] private TMP_Text healthCounter = null;
        [SerializeField] private TMP_Text armorCounter = null;
        [SerializeField] private TMP_Text ammoCounter = null;

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

    }

}
