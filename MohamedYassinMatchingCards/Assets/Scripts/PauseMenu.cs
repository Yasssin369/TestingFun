using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public void OpenPauseMenu()
    {
        Time.timeScale = 0;
        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuPanel.SetActive(false);
    }

    public void SaveGame()
    {
        GameController.Instance.SaveGame();
    }

    public void LoadGame()
    {
        GameController.Instance.LoadGame();
    }
}
