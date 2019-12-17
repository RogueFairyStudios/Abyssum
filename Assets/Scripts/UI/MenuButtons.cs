using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace DEEP.UI { 

    public class MenuButtons : MonoBehaviour
    {
        public void QuitGame()
        {
            Debug.Log("Application.Quit()");
            Application.Quit();
        }

        public void StartGame(){
            Debug.Log("Starting game...");
            SceneManager.LoadScene(2);
        }

        public void Showcase()
        {
            Debug.Log("Starting showcase...");
            SceneManager.LoadScene(1);
        }
    }

}
