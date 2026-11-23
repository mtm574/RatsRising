using UnityEngine;
//literally just quits the game :/
public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
