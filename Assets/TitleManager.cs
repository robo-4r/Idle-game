using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public Button continueButton;

    void Start()
    {
        if (!PlayerPrefs.HasKey("SAVE_DATA"))
        {
            continueButton.interactable = false;
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("SAVE_DATA");
        SceneManager.LoadScene("geimu");
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("LOAD_GAME", 1);
        SceneManager.LoadScene("geimu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}