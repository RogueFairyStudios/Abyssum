using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.Stage;

namespace DEEP.UI { 

    public class MainMenu : MenuButtons
    {

        string menuScene;

        [SerializeField] StageInfo defaultCutsceneLevel = null;

        [SerializeField] GameObject background = null;

        [SerializeField] GameObject mainMenu = null;
        [SerializeField] GameObject loadingScreen = null;

        void Start() {

            // Sets the menu to not be destroyed.
            DontDestroyOnLoad(gameObject);         

            // Gets the name of the original menu scene.
            menuScene = SceneManager.GetActiveScene().name;

            // Loads the cutscene scene.
            SceneManager.sceneLoaded += OnCutsceneSceneLoaded;
            SceneManager.LoadSceneAsync(defaultCutsceneLevel.stageScene, LoadSceneMode.Additive);

        }

        void OnCutsceneSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            // Unloads the original menu scene.
            SceneManager.UnloadSceneAsync(menuScene);

            // Disables the background.
            background.SetActive(false);

            // Enables the main menu.
            loadingScreen.SetActive(false);
            mainMenu.SetActive(true);

            SceneManager.sceneLoaded -= OnCutsceneSceneLoaded;

        }

        public override void LoadLevel(string levelName){

            Debug.Log("Loading " + levelName + "...");
            
            Debug.Log(levelName + "/" + defaultCutsceneLevel.stageScene);
            if(levelName == defaultCutsceneLevel.stageScene) {
                
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