using UnityEngine;
using UnityEngine.SceneManagement; // control scene

public class MainMenu : MonoBehaviour
{
    public void Play() // functia pt play
    {
        SceneManager.LoadScene(1); // incarca jocul
    }

    public void Exit() // functia pt exit
    {
        Debug.Log("Jocul s-a închis!");
        Application.Quit(); // inchide jocul
    }
}