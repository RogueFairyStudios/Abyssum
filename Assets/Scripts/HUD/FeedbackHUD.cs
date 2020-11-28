using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace DEEP.HUD {

    // Types of feedback, used to choose screen feedback color.
    public enum FeedbackType { Damage, Toxic, Mud }

    public class FeedbackHUD : MonoBehaviour {

        [Tooltip("Animator used to play screen effects giving feedback to the player.")]
        public Image screenFeedback = null;

        public Coroutine screenFeedbackAnim = null; // Stores the current screen feedback coroutine.
        [Tooltip("Duration of screen flash feedbacks.")]

        public float duration = 0.1f;
        [Tooltip("Color for the damage feedback.")]

        public Color damageFeedbackColor = Color.red;
        [Tooltip("Color for the toxic feedback.")]

        public Color toxicFeedbackColor = Color.green;
        [Tooltip("Color for the mud feedback.")]

        public Color mudFeedbackColor = Color.yellow;

        private bool constantFeedbackActive;

        private Color currentConstantFeedbackColor;

        // Starts a screen feedback effect.
        public void StartScreenFeedback(FeedbackType type) {

            // If a feedback effect is already happening stop it and start a new one.
            if(screenFeedbackAnim != null)
                StopCoroutine(screenFeedbackAnim);

            // Gets the correct color for the feedback.
            Color feedbackColor = Color.black;
            switch(type) {

                // Quick flash effects.
                case FeedbackType.Damage:
                feedbackColor = damageFeedbackColor;
                screenFeedbackAnim = StartCoroutine(ScreenFeedbackAnim(feedbackColor, duration));
                break;

                // Constant effects.
                case FeedbackType.Toxic:
                feedbackColor = toxicFeedbackColor;
                StartConstantScreenFeedback(feedbackColor);
                break;

                case FeedbackType.Mud:
                feedbackColor = mudFeedbackColor;
                StartConstantScreenFeedback(feedbackColor);
                break;

            }

            
        }

        protected IEnumerator ScreenFeedbackAnim(Color color, float duration) {

            // Sets the feedback color and show it.
            screenFeedback.color = color;
            screenFeedback.enabled = true;

            // Waits for the duration.
            yield return new WaitForSeconds(duration);

            // If there are no constant feedbacks active
            if(!constantFeedbackActive)
            {
                // Ends the feedback
                screenFeedback.enabled = false;
            }
            else
            {
                // Turns the feedback back to the constant color
                screenFeedback.color = currentConstantFeedbackColor;
            }

            screenFeedbackAnim = null;
        }

        protected void StartConstantScreenFeedback(Color color)
        {

            // Sets the feedback color and shows it
            screenFeedback.color = color;
            screenFeedback.enabled = true;

            // Sets up flags
            constantFeedbackActive = true;                
            currentConstantFeedbackColor = color;
        }

        public void StopConstantScreenFeedback()
        {

            // Sets up flags
            constantFeedbackActive = false;

            // If there are no other feedbacks going on, turn the feedback image off
            if(screenFeedbackAnim == null && screenFeedback != null)
                screenFeedback.enabled = false;
        }

    }
}
