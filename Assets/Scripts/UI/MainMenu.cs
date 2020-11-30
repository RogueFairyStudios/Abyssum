using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using DEEP.Stage;

namespace DEEP.UI { 

    public class MainMenu : MenuButtons
    {

        Scene menuScene;
        Scene cutScene;

        [SerializeField] StageInfo defaultCutsceneStage = null;

        [SerializeField] GameObject mainMenu = null;

        [SerializeField] GameObject loadingScreen = null;

        [SerializeField] GameObject background = null;


        void Start() {

            // Sets the menu to not be destroyed.
            DontDestroyOnLoad(gameObject);         

            // Gets the name of the original menu scene.
            menuScene = SceneManager.GetActiveScene();

            // Loads the cutscene scene.
            SceneManager.sceneLoaded += OnCutsceneSceneLoaded;
            SceneManager.LoadSceneAsync(defaultCutsceneStage.stageScene, LoadSceneMode.Additive);

        }

        void OnCutsceneSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            // Saves a cutscene reference.
            cutScene = scene;

            // Unloads the original menu scene.
            SceneManager.UnloadSceneAsync(menuScene);

            // Disables the background.
            background.SetActive(false);

            // Shows the main menu.
            mainMenu.SetActive(true);
            loadingScreen.SetActive(false);

            SceneManager.sceneLoaded -= OnCutsceneSceneLoaded;

        }

        public override void LoadLevel(string sceneName){

            Debug.Log("Loading " + sceneName + "...");
            
            Debug.Log(sceneName + "/" + defaultCutsceneStage.stageScene);
            if(sceneName == defaultCutsceneStage.stageScene) {
                
                Debug.Log("Scene already loaded!");
                OnSceneAlreadyLoaded();

            } else { StartCoroutine(SwapScene(sceneName)); }

        }

        void OnSceneAlreadyLoaded() {

            StageManager.Instance.EnableLevelStart();
            Destroy(gameObject);

        }

        IEnumerator SwapScene(string sceneName) { 

            AsyncOperation loadAsync = null;
            AsyncOperation unloadAsync = null; 
            
            // Loads the target scene and waits.
            loadAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while(!loadAsync.isDone) {

                // Waits for when scene activation stage is close.
                if(loadAsync.progress >= 0.85f) {

                    // Enables background.
                    background.SetActive(true);

                }
                yield return null;

            }

            // Unloads the cutscene and waits.
            unloadAsync = SceneManager.UnloadSceneAsync(cutScene);
            while(!unloadAsync.isDone) yield return null;

            OnSceneAlreadyLoaded(); 

        }

    }

}