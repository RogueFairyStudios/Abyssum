using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.Stage;

namespace DEEP.UI { 

    public class MenuButtons : MonoBehaviour
    {

        [Tooltip("Optional reference to an options menu.")]
        [SerializeField] protected OptionsButtons optionsMenu = null;

        public virtual void QuitGame()
        {
            Debug.Log("Application.Quit()");
            Application.Quit();
        }

        public virtual void LoadLevel(string levelName){
            Debug.Log("Loading " + levelName + "...");
            SceneManager.LoadSceneAsync(levelName);
        }

        public virtual void LoadURL(string url){
            Debug.Log("Opening URL " + url + "...");
            Application.OpenURL(url);
        }

        public void NextLevel() { SceneManager.LoadSceneAsync(StageManager.Instance.GetNextStage()); }

        public void RestartGame() { SceneManager.LoadSceneAsync(0); }

        public void RestartLevel() { SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name); }
        
    }

}
