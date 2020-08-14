using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DEEP.Stage;
using DEEP.Entities;
using DEEP.DoorsAndKeycards;

namespace DEEP.UI
{

    public class HUDController : MonoBehaviour {

        [System.Serializable] // Health ===========================================================================
        public class HealthHUD
        {

            [SerializeField] protected TMP_Text counter = null;

            [SerializeField] protected TMP_Text counterTitle = null;

            [SerializeField] protected Color defaultColor = Color.green;
            [SerializeField] protected Color overloadColor = Color.magenta;

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

        [System.Serializable] // Armor ============================================================================
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

        [System.Serializable] // Ammo & Weapons ===================================================================
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

        [System.Serializable] // Keycards =========================================================================
        public class KeyHUD 
        {
            [SerializeField] protected Image blueKeyIcon = null;
            [SerializeField] protected Image redKeyIcon = null;
            [SerializeField] protected Image yellowKeyIcon = null;

            public void UpdateValues() {

                blueKeyIcon.enabled = Player.Instance.keyInventory.HasKey(KeysColors.Blue);
                redKeyIcon.enabled = Player.Instance.keyInventory.HasKey(KeysColors.Red);
                yellowKeyIcon.enabled = Player.Instance.keyInventory.HasKey(KeysColors.Yellow);

            }

        }
        public KeyHUD keycards = null;

        [System.Serializable] // Log ==============================================================================
        public class LogHUD
        {
            [SerializeField] protected TMP_Text text = null;
            [SerializeField] protected Image icon = null;

            public void Message(string message, Sprite icon, Color color) {

                text.color = color;
                text.text = StageInfo.Instance.GetDurationString() + ": " + message;

                this.icon.color = color;
                this.icon.sprite = icon;

            }
        }
        public LogHUD Log = null;

        [System.Serializable] // Speedrun =========================================================================
        public class SpeedrunHUD
        {

            [SerializeField] protected GameObject panel = null;

            [SerializeField] protected TMP_Text kills = null;
            [SerializeField] protected TMP_Text items = null;
            [SerializeField] protected TMP_Text secrets = null;
            [SerializeField] protected TMP_Text time = null;

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
        public SpeedrunHUD speedrun = null;

        [System.Serializable] // Statistics =======================================================================
        public class StatisticsHUD
        {

            [SerializeField] protected GameObject panel = null;

            [SerializeField] protected TMP_Text stats = null;

            // FPS
            [Tooltip("Number of frames to average for the FPS.")]
            [SerializeField] protected int FPSSampleCount = 30;

            [Tooltip("Delay between updates (in seconds).")]
            [SerializeField] protected float updateFPSDelay = 0.5f;

            protected float[] frameTimeSamples;
            protected float sampleSum;
            protected int currentSample;

            protected float lastUpdateTime;

            // Info about API nad OS version along with hardware config.
            protected string computerInfo;

            public void SetEnabled(bool enabled) {
                panel.SetActive(enabled);
            }

            public void Initialize() {

                // Creates and initializes the FPS sample buffer and sample sum.
                frameTimeSamples = new float[FPSSampleCount];
                for(int i = 0; i < FPSSampleCount; i++)
                    frameTimeSamples[i] = Time.unscaledDeltaTime;
                sampleSum = FPSSampleCount * Time.unscaledDeltaTime;
                currentSample = 0;

                lastUpdateTime = Time.time;

                // Gets the computer info.
                string OSInfo = SystemInfo.operatingSystem;
                string graphicsAPIInfo = SystemInfo.graphicsDeviceType.ToString();
                string processorInfo = SystemInfo.processorType;
                int systemMemoryInfo = SystemInfo.systemMemorySize;
                string graphicsCardInfo = SystemInfo.graphicsDeviceName;
                int graphicsMemoryInfo = SystemInfo.graphicsMemorySize;

                // Creates the string.
                computerInfo = "";
                computerInfo += "<b>API:</b> "   + graphicsAPIInfo       + "<br>";
                computerInfo += "<b>OS:</b> "    + OSInfo                + "<br>";
                computerInfo += "<b>CPU:</b> "   + processorInfo         + "<br>";
                computerInfo += "<b>RAM:</b> "   + systemMemoryInfo      + "MB<br>";
                computerInfo += "<b>GPU:</b> "   + graphicsCardInfo      + "<br>";
                computerInfo += "<b>VRAM:</b> "  + graphicsMemoryInfo    + "MB";

            }

            public void UpdateStats() {

                // Removes the oldest sample from the average and adds a new one.
                sampleSum -= frameTimeSamples[currentSample];
                frameTimeSamples[currentSample] = Time.unscaledDeltaTime;
                sampleSum += frameTimeSamples[currentSample];
                currentSample = (currentSample + 1) % FPSSampleCount;
                
                // Waits for the delay before updating.
                if(Time.time - lastUpdateTime < updateFPSDelay)
                    return;

                // Calculates the average of the frame time samples and converts to FPS.
                float fps = (FPSSampleCount / sampleSum);

                // Colors the FPS text based on how high it is.
                string fpsText = "<b>FPS:</b> ";
                if(fps < 30) {
                    fpsText += "<color=#ff0000>" + Mathf.Floor(fps) + "</color>";
                } else if (fps < 60) {
                    fpsText += "<color=#ffff00>" + Mathf.Floor(fps) + "</color>";
                } else if (fps < 120) {
                    fpsText += "<color=#00ff00>" + Mathf.Floor(fps) + "</color>";
                } else if (fps < 240) {
                    fpsText += "<color=#00ffff>" + Mathf.Floor(fps) + "</color>";
                } else {
                    fpsText += "<color=#ff00ff>" + Mathf.Floor(fps) + "</color>";
                }

                // Creates a string with screen resolution info.
                string resolutionText = "<b>Resolution:</b> ";
                int gcd = GCD(Screen.width, Screen.height);
                resolutionText += Screen.width + "x" + Screen.height + " (" + (Screen.width / gcd) + ":" + (Screen.height / gcd) + ")";

                // Creates a string with window info.
                string windowText = "<b>Window Mode:</b> ";
                if(Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) {
                    windowText += "FullScreen";
                } else if(Screen.fullScreenMode == FullScreenMode.FullScreenWindow) {
                    windowText += "Windowed FullScreen";
                } else {
                    windowText += "Windowed";
                }

                // Puts the stats on the screen.
                stats.text = fpsText + "<br>" + computerInfo + "<br>" + resolutionText + "<br>" + windowText;

                // Saves the update time.
                lastUpdateTime = Time.time;

            }

            // Are you serious that neither UnityEngine.Mathf neither System.Math has a GCD function?
            private int GCD(int a, int b)
            {
                while (a != 0 && b != 0)
                {
                    if (a > b)
                        a %= b;
                    else
                        b %= a;
                }
                return a | b;
            }

        }
        public StatisticsHUD statistics = null;

        // Feedback ===============================================================================================

        // Types of feedback, used to choose screen feedback color.
        public enum FeedbackType { Damage, Toxic, Mud }

        [System.Serializable]
        protected class PlayerFeedback
        {

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
            public Color mudFeedbackColor = Color.green;
            [HideInInspector] public bool constantFeedbackActive;
            [HideInInspector] public Color currentConstantFeedbackColor;

        }
        [SerializeField] protected PlayerFeedback playerFeedback = null;

        // Starts a screen feedback effect.
        public void StartScreenFeedback(FeedbackType type) {

            // If a feedback effect is already happening stop it and start a new one.
            if(playerFeedback.screenFeedbackAnim != null)
                StopCoroutine(playerFeedback.screenFeedbackAnim);

            // Gets the correct color for the feedback.
            Color feedbackColor = Color.black;
            switch(type) {

                // Quick flash effects.
                case FeedbackType.Damage:
                feedbackColor = playerFeedback.damageFeedbackColor;
                playerFeedback.screenFeedbackAnim = StartCoroutine(ScreenFeedbackAnim(feedbackColor, playerFeedback.duration));
                break;

                // Constant effects.
                case FeedbackType.Toxic:
                feedbackColor = playerFeedback.toxicFeedbackColor;
                StartConstantScreenFeedback(feedbackColor);
                break;

                case FeedbackType.Mud:
                feedbackColor = playerFeedback.mudFeedbackColor;
                StartConstantScreenFeedback(feedbackColor);
                break;

            }

            
        }

        protected IEnumerator ScreenFeedbackAnim(Color color, float duration) {

            // Sets the feedback color and show it.
            playerFeedback.screenFeedback.color = color;
            playerFeedback.screenFeedback.enabled = true;

            // Waits for the duration.
            yield return new WaitForSeconds(duration);

            // If there are no constant feedbacks active
            if(!playerFeedback.constantFeedbackActive)
            {
                // Ends the feedback
                playerFeedback.screenFeedback.enabled = false;
            }
            else
            {
                // Turns the feedback back to the constant color
                playerFeedback.screenFeedback.color = playerFeedback.currentConstantFeedbackColor;
            }

            playerFeedback.screenFeedbackAnim = null;
        }

        protected void StartConstantScreenFeedback(Color color)
        {
            // Sets the feedback color and shows it
            playerFeedback.screenFeedback.color = color;
            playerFeedback.screenFeedback.enabled = true;

            // Sets up flags
            playerFeedback.constantFeedbackActive = true;                
            playerFeedback.currentConstantFeedbackColor = color;
        }

        public void StopConstantScreenFeedback()
        {
            // Sets up flags
            playerFeedback.constantFeedbackActive = false;

            // If there are no other feedbacks going on, turn the feedback image off
            if(playerFeedback.screenFeedbackAnim == null)
                playerFeedback.screenFeedback.enabled = false;
        }

        // ========================================================================================================

        void Start() {

            // Gathers initial information for the statistics.
            statistics.Initialize();

            // Gets initial speedrun HUD value.
            if(!PlayerPrefs.HasKey("SpeedrunHUD"))
                PlayerPrefs.SetInt("SpeedrunHUD", 0);
            speedrun.SetEnabled(PlayerPrefs.GetInt("SpeedrunHUD") == 1);

            // Gets initial statistics HUD value.
            if(!PlayerPrefs.HasKey("StatisticsHUD"))
                PlayerPrefs.SetInt("StatisticsHUD", 0);
            statistics.SetEnabled(PlayerPrefs.GetInt("StatisticsHUD") == 1);

        }

        void Update() {

            // Constantly updates the speedrun clock.
            speedrun.SetStageTime(StageInfo.Instance.GetDurationString());

            // Constantly updates the statistics.
            statistics.UpdateStats();

        }       

    }

}
