using UnityEngine;

using TMPro;

using DEEP.Stage;

namespace DEEP.UI
{

    public class EndScreen : MonoBehaviour
    {

        [SerializeField] private TMP_Text completionMessage = null;

        [SerializeField] private TMP_Text killCount = null;
        [SerializeField] private TMP_Text itemCount = null;
        [SerializeField] private TMP_Text secretCount = null;

        [SerializeField] private TMP_Text time = null;
        [SerializeField] private TMP_Text PAR = null;

        public void ShowScreen() {

            // Reference to the stage info (mainly to reduce the size of the kinda big lines below).
            StageManager manager = StageManager.Instance;

            // Calculates and assigns the stage information to the end screen.
            completionMessage.text = manager.GetStageName().ToUpper() + " COMPLETED!";

            time.text = StageManager.GetDurationString(manager.GetDuration());
            PAR.text = StageManager.GetDurationString(manager.stageInfo.stagePAR);

            killCount.text = manager.GetKillCount() + "/" + manager.GetTotalEnemies() + " (" + (manager.GetKillPercentage() * 100).ToString("0.0") + "%)";
            itemCount.text = manager.GetCollectibleCount() + "/" + manager.GetTotalCollectibles() + " (" + (manager.GetCollectiblePercentage() * 100).ToString("0.0") + "%)";
            secretCount.text = manager.GetSecretCount() + "/" + manager.GetTotalSecrets() + " (" + (manager.GetSecretPercentage() * 100).ToString("0.0") + "%)";

            // Enables this gameObject.
            gameObject.SetActive(true);

        }

    }

}
