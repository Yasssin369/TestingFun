using UnityEngine;
using UnityEngine.SceneManagement;
using static GameController;

public class MainMenuUI : MonoBehaviour
{
    public void PlayEasy()
    {
        GameController.SelectedMode = GameMode.Easy_2x3;
        SceneManager.LoadScene("Main"); 
    }

    public void PlayMedium()
    {
        GameController.SelectedMode = GameMode.Medium_4x4;
        SceneManager.LoadScene("Main");
    }

    public void PlayHard()
    {
        GameController.SelectedMode = GameMode.Hard_5x6;
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
