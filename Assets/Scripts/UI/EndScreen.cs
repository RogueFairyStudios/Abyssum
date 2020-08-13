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

        public void ShowScreen() {

            // Reference to the stage info (mainly to reduce the size of the kinda big lines below).
            StageInfo info = StageInfo.Instance;

            // Calculates and assigns the stage information to the end screen.
            completionMessage.text = info.GetStageName().ToUpper() + " COMPLETED!";

            time.text = info.GetDurationString();

            killCount.text = info.GetKillCount() + "/" + info.GetTotalEnemies() + " (" + (info.GetKillPercentage() * 100).ToString("0.0") + "%)";
            itemCount.text = info.GetCollectibleCount() + "/" + info.GetTotalCollectibles() + " (" + (info.GetCollectiblePercentage() * 100).ToString("0.0") + "%)";
            secretCount.text = info.GetSecretCount() + "/" + info.GetTotalSecrets() + " (" + (info.GetSecretPercentage() * 100).ToString("0.0") + "%)";

            // Enables this gameObject.
            gameObject.SetActive(true);

        }

    }

}
