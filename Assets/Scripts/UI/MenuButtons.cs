using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace DEEP.UI { 

    public class MenuButtons : MonoBehaviour
    {

        [Tooltip("Optional reference to an options menu.")]
        [SerializeField] protected OptionsButtons optionsMenu = null;

        void Start() {

            if(optionsMenu != null)
                optionsMenu.Initialize();

        }

        public void QuitGame()
        {
            Debug.Log("Application.Quit()");
            Application.Quit();
        }

        public void LoadLevel(string levelName){
            Debug.Log("Loading " + levelName + "...");
            SceneManager.LoadScene(levelName);
        }

        public void LoadURL(string url){
            Debug.Log("Opening URL " + url + "...");
            Application.OpenURL(url);
        }
    }

}
