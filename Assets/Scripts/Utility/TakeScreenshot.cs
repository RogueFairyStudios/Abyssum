using UnityEngine;

namespace DEEP.Utility
{

    public class TakeScreenshot : MonoBehaviour
    {

        void Update()
        {

            if(Input.GetKeyDown(KeyCode.F9)) {

                // Gets the system date to be put on the filename.
                string date = System.DateTime.Now.ToString().Replace(':','_').Replace('/','_').Replace(' ','_');
                // Gets a random number to be put in the filename in case multiples screenshot are taken at the 
                // same time there is only a very small change one will be overwritten.
                int random = Random.Range(1000000, 10000000);
                // The format of the image.
                string format = ".png";
                ScreenCapture.CaptureScreenshot("SB_Screenshot_" + date + '_' + random + format);

            }

        }
    }

}
