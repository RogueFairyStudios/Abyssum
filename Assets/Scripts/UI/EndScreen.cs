using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using DEEP.Stage;

namespace DEEP.UI
{

    public class EndScreen : MonoBehaviour
    {
        
        [SerializeField] private GameObject endScreen = null;

        [SerializeField] private TMP_Text completionMessage = null;

        [SerializeField] private TMP_Text killCount = null;
        [SerializeField] private TMP_Text itemCount = null;
        [SerializeField] private TMP_Text secretCount = null;

        [SerializeField] private TMP_Text time = null;

        public void ShowScreen() {

            StageInfo info = StageInfo.current;

            completionMessage.text = info.GetStageName().ToUpper() + " COMPLETED!";

            killCount.text = info.GetKillCount() + "/" + info.GetTotalEnemies() + " (" + (info.GetKillPercentage() * 100).ToString("0.0") + "%)";
            itemCount.text = info.GetCollectibleCount() + "/" + info.GetTotalCollectibles() + " (" + (info.GetCollectiblePercentage() * 100).ToString("0.0") + "%)";
            secretCount.text = info.GetSecretCount() + "/" + info.GetTotalSecrets() + " (" + (info.GetSecretPercentage() * 100).ToString("0.0") + "%)";

            float completionTime = info.GetDuration();

            float minutes = (int)Mathf.Floor(completionTime / 60);
            float seconds = (int)Mathf.Floor(completionTime - minutes * 60);
            float secondFraction = (int)Mathf.Round((completionTime - seconds - (minutes * 60)) * 10);

            time.text = minutes + ":" + seconds.ToString().PadLeft(2, '0') + "." + secondFraction;

            endScreen.SetActive(true);

        }

    }

}
