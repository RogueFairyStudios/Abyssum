using UnityEngine;

using TMPro;

namespace DEEP.HUD {

    public class StatisticsHUD : MonoBehaviour {

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

        // Info about API and OS version along with hardware config.
        protected string computerInfo;

        public void SetEnabled(bool enabled) {
            panel.SetActive(enabled);
        }

        protected void Start() {

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

        protected void Update() {

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

        // Are you serious that neither UnityEngine.Mathf nor System.Math has a GCD function?
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

}
