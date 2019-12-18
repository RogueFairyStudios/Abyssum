using UnityEngine;

namespace DEEP.Utility
{

    public class FPSCounter : MonoBehaviour
    {

        private bool show = false;
        private float fps = 0;

        private float counter = 0;
        private float secFrames = 0;

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

            if(show)
                GUI.Label(new Rect(10,10,200,200), "FPS: " + Mathf.Floor(fps));

        }

    }
}