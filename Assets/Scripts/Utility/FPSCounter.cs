using UnityEngine;

namespace DEEP.Utility
{

    public class FPSCounter : MonoBehaviour
    {

        private bool show = false;
        private float fps = 0;

        private float counter = 0;
        private float secFrames = 0;

        private string OSInfo;
        private string graphicsAPIInfo;
        private string processorInfo;
        private int systemMemoryInfo;
        private string graphicsCardInfo;
        private int graphicsMemoryInfo;

        void Start()
        {

            OSInfo = SystemInfo.operatingSystem;
            graphicsAPIInfo = SystemInfo.graphicsDeviceType.ToString();
            processorInfo = SystemInfo.processorType;
            systemMemoryInfo = SystemInfo.systemMemorySize;
            graphicsCardInfo = SystemInfo.graphicsDeviceName;
            graphicsMemoryInfo = SystemInfo.graphicsMemorySize;

        }

        void Update()
        {

            if(Input.GetKeyDown(KeyCode.F8)) show =! show;

            if(show) 
            {
                if(counter == 100)
                {

                    fps = 1 / (secFrames / 100);
                    secFrames = 0;
                    counter = 0;

                } 
                else
                {

                    secFrames += Time.unscaledDeltaTime;
                    counter++;

                }

            }

        }

        void OnGUI()
        {

            if(show) {
                GUI.Label(new Rect(10,10, 800, 200), "FPS: " + Mathf.Floor(fps) + "\nAPI: "+ graphicsAPIInfo +"\nOS: " + OSInfo + "\nCPU: " + processorInfo + "\nRAM: " + systemMemoryInfo + "MB\nGPU: " + graphicsCardInfo + "\nVRAM: " + graphicsMemoryInfo + "MB");
            }

        }

    }
}
