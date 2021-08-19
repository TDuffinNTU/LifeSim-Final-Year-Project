using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    GameObject AboutMenu, HelpMenu;

    // Start is called before the first frame update
    void Start()
    {
        AboutMenu = GameObject.Find("ABOUTMENU");
        HelpMenu = GameObject.Find("HELPMENU");

        HelpMenu.SetActive(false);
        AboutMenu.SetActive(false);
    }

    /// <summary>
    /// toggle menu on main menu
    /// </summary>
    /// <param name="menu"></param>
    public void ToggleMenu(int menu) 
    {
        if (menu == 0) 
        {
            AboutMenu.SetActive(!AboutMenu.activeInHierarchy);
        }
        else 
        {
            HelpMenu.SetActive(!HelpMenu.activeInHierarchy);
        }
    }

    /// <summary>
    /// load gameplay
    /// </summary>
    public void LoadGame() 
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    /// <summary>
    /// Quits
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

}

