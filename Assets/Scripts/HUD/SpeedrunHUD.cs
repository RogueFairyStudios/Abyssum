using TMPro;
using UnityEngine;

using DEEP.Stage;

namespace DEEP.HUD {

    public class SpeedrunHUD : MonoBehaviour {
        
        [SerializeField] protected GameObject panel = null;

        [SerializeField] protected TMP_Text kills = null;
        [SerializeField] protected TMP_Text items = null;
        [SerializeField] protected TMP_Text secrets = null;
        [SerializeField] protected TMP_Text time = null;

        protected void Start() {

            // Initializes the speedrun HUD
            StageManager stage = StageManager.Instance; 
            if(stage != null) {
                SetKillCount(stage.GetKillCount(), stage.GetTotalEnemies());
                SetItemCount(stage.GetCollectibleCount(), stage.GetTotalCollectibles());
                SetSecretCount(stage.GetSecretCount(), stage.GetTotalSecrets());
            }

        }

        protected void Update() {

            // Constantly updates the speedrun clock.
            if(StageManager.Instance != null)
                SetStageTime(StageManager.GetDurationString(StageManager.Instance.GetDuration()));

        }  

        public void SetEnabled(bool enabled) {
            panel.SetActive(enabled);
        }

        public void SetKillCount(int current, int total) {
            kills.text = current + "/" + total;
        }

        public void SetItemCount(int current, int total) {
            items.text = current + "/" + total;
        }

        public void SetSecretCount(int current, int total) {
            secrets.text = current + "/" + total;
        }

        public void SetStageTime(string formatedTime) {
            time.text = formatedTime;
        }

    }

}
