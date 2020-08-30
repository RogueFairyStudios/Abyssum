using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class MenuCutscenes : MonoBehaviour
{
    [SerializeField] Object sceneToLoad;
    [SerializeField] GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        //! Temp for now
        SceneManager.LoadSceneAsync(sceneToLoad.name, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += FindDirector;
    }

    void FindDirector(Scene scene, LoadSceneMode mode)
    {
        background.SetActive(false);

        //! FIXME: This is utterly disgusting. Or not. I mean, it's good for performance since it's literally the first object there but it's ugly to look at.
        //! Just like that friend of yours, bob. Y'know, he's cool and all but you just feel weird when you look at his face. Poor bob. Maybe the problem is us after all.
        PlayableDirector director = scene.GetRootGameObjects()[0].GetComponent<PlayableDirector>();
        // TODO as soon as cutscene ends, select another one
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= FindDirector;
    }
}
