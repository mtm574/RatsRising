using UnityEngine;
using UnityEngine.SceneManagement;
//Just the main menu manager. It handles the play and quit buttons. The play button loads the first cutscene, and the quit button quits the game.
public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "CutScene1";

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
