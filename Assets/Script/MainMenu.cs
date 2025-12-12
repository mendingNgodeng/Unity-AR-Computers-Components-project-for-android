using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void Exit(){
        Application.Quit();
        Debug.Log("Game Telah Keluar");
    }

    public void About(){
        SceneManager.LoadScene("About");
    }

    public void Play(){
        SceneManager.LoadScene("Start");
    }
}
