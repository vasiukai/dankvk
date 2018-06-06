using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    public void Lvl01()
    {
        SceneManager.LoadScene("Level1");
    }
    public void Lvl02()
    {
        SceneManager.LoadScene("Level2");
    }
    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
