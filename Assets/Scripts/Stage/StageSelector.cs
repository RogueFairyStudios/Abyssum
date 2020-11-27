using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DEEP.UI;

namespace DEEP.Stage
{

    public class StageSelector : MonoBehaviour
    {

        [Tooltip("Reference to the MainMenu script")]
        [SerializeField] private MainMenu menu = null;

        [Tooltip("ScriptableObject with information about the stage")]
        public List<StageInfo> stages;

        [SerializeField] private Button nextButton = null;
        [SerializeField] private Button previousButton = null;

        [SerializeField] private TMP_Text sceneName = null;
        [SerializeField] private Image sceneBanner = null;

        [SerializeField] private TMP_Text bestTime = null;
        [SerializeField] private TMP_Text parTime = null;

        [Tooltip("Banner to be used for levels that are not unlocked yet or when a banner is missing.")]
        [SerializeField] private Sprite unknownBanner = null;

        private int currentStage;

        void Start() {

            if(menu == null)
                Debug.LogError("StageSelector.Start: No MainMenu!");

            if(stages.Count < 0) {
                Debug.LogWarning("StageSelector.Start: No stages available!");
            } else {

                // Sets the first stage as the default stage.
                currentStage = 0;
                SetStage(0);

            }

            // Updates the state of the next and previous buttons.
            SetButtons();

        }

        private void SetStage(int index) {

            sceneName.text = stages[index].stageName;
            // TODO: use unknown banner for stages that were not unlocked.
            sceneBanner.sprite = stages[index].banner != null ? stages[index].banner : unknownBanner;

            // TODO: save an load best times.
            bestTime.text = "Best: " + StageManager.GetDurationString(-1);;
            parTime.text = "PAR: " + StageManager.GetDurationString(stages[index].stagePAR); 

        }

        public void NextStage() {

            if(currentStage >= stages.Count - 1)
                return;

            currentStage++;
            SetStage(currentStage);
            SetButtons();


        }

        public void PreviousStage() {

            if(currentStage <= 0)
                return;

            currentStage--;
            SetStage(currentStage);
            SetButtons();

        }

        private void SetButtons() {

            // TODO: disable the play button if the previous stage hasn't been completed yet.

            // Disables the previous button if it's the last stage.
            previousButton.interactable = (currentStage > 0);

            // Disables the next button if it's the last stage.
            nextButton.interactable = (currentStage < stages.Count - 1);

        }

        public void LoadCurrentLevel() { menu.LoadLevel(stages[currentStage].stageScene); }

    }

}
