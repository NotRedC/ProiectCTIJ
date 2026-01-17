using UnityEngine;
using UnityEngine.SceneManagement; // control scene

public class MainMenu : MonoBehaviour
{
    public void Play() // functia pt play
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            string sceneToLoad = PlayerPrefs.GetString("SavedScene");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene("Level_1");
        }
    }

    public void Exit() // functia pt exit
    {
        Debug.Log("Jocul s-a închis!");
        Application.Quit(); // inchide jocul
    }
}