using UnityEngine;
using UnityEngine.SceneManagement;


namespace DEEP.UI { 

    public class MenuButtons : MonoBehaviour
    {

        [Tooltip("Optional reference to an options menu.")]
        [SerializeField] protected OptionsButtons optionsMenu = null;

        public void QuitGame()
        {
            Debug.Log("Application.Quit()");
            Application.Quit();
        }

        public void LoadLevel(string levelName){
            Debug.Log("Loading " + levelName + "...");
            SceneManager.LoadSceneAsync(levelName);
        }

        public void LoadURL(string url){
            Debug.Log("Opening URL " + url + "...");
            Application.OpenURL(url);
        }
        
    }

}
