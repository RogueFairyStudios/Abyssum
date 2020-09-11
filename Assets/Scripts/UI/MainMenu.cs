using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.Stage;

namespace DEEP.UI { 

    public class MainMenu : MenuButtons
    {

        string menuScene;

        [SerializeField] string defaultCutsceneScene;

        [SerializeField] GameObject background;

        void Start() {

            // Sets the menu to not be destroyed.
            DontDestroyOnLoad(gameObject);         

            // Gets the name of the original menu scene.
            menuScene = SceneManager.GetActiveScene().name;

            // Loads the cutscene scene.
            SceneManager.sceneLoaded += OnCutsceneSceneLoaded;
            SceneManager.LoadSceneAsync(defaultCutsceneScene, LoadSceneMode.Additive);

        }

        void OnCutsceneSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            // UNlaods the original menu scene.
            SceneManager.UnloadSceneAsync(menuScene);

            // Disables the background.
            background.SetActive(false);

            SceneManager.sceneLoaded -= OnCutsceneSceneLoaded;

        }

        public override void LoadLevel(string levelName){

            Debug.Log("Loading " + levelName + "...");
            
            Debug.Log(levelName + "/" + defaultCutsceneScene);
            if(levelName == defaultCutsceneScene) {
                
                Debug.Log("Scene already loaded!");
                OnSceneAlreadyLoaded();

            } else {

                SceneManager.LoadScene(levelName, LoadSceneMode.Single);
                SceneManager.sceneLoaded += OnSceneLoaded;

            }

        }

        void OnSceneAlreadyLoaded() {

            StageManager.Instance.EnableLevelStart();
            Destroy(gameObject);

        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) { 

            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnSceneAlreadyLoaded(); 

        }

    }

}