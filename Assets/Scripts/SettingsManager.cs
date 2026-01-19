using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Toggle musicButton;       
    [SerializeField] private Toggle sfxButton;
    [SerializeField] private TextMeshProUGUI musicButtonText;

    private bool isMusicOn = true;
    private bool isSfxOn = true;
    void Start()
    {
        isMusicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;

        musicButton.isOn = isMusicOn;
        sfxButton.isOn = isSfxOn;

        UpdateMusicVisuals(isMusicOn);
    }
    public void OnMusicChange(bool isOn)
    {
        UpdateMusicVisuals(isOn);
    }

    void UpdateMusicVisuals(bool isOn)
    {
        if (isOn)
        {
            musicButtonText.text = "Music"; 
            musicButton.targetGraphic.color = Color.white; 
        }
        else
        {
            musicButtonText.text = "No";
            musicButton.targetGraphic.color = Color.clear; 
        }
    }

    public void OnSFXChange(bool isOn)
    {
        PlayerPrefs.SetInt("SFX", isOn ? 1 : 0);
    }

    public void BackToMenu()
    {
       
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}
