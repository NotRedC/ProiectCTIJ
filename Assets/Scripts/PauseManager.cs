using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 
    private string mainMenuScene = "MainMenu";
    private static bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f;          
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f;          
        isPaused = true;
    }

    public void LoadMenu()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<GameSaver>().SaveGame();
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}