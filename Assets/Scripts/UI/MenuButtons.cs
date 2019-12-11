using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuButtons : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    public void StartGame(){
        Debug.Log("starting game");
        SceneManager.LoadScene(2);
    }

    public void Showcase()
    {
        Debug.Log("showcase");
        SceneManager.LoadScene(1);
    }
}
