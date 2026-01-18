using UnityEngine;
using UnityEngine.SceneManagement; // control scene

public class MainMenu : MonoBehaviour
{
    public void Play() // functia pt play
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            Debug.Log("Înc?rcare joc salvat în scena: " + PlayerPrefs.GetString("SavedScene"));
            string sceneToLoad = PlayerPrefs.GetString("SavedScene");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene("Level 1");
        }
    }

    public void Exit() // functia pt exit
    {
        Debug.Log("Jocul s-a închis!");
        Application.Quit(); // inchide jocul
    }

    public void OpenSettings()
    {
        Debug.Log("Deschidere meniul de set?ri");
        SceneManager.LoadScene("SettingsMenu");
    }
}