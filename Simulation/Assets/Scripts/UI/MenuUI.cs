using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A singelton for the UI controller in the main menu.
/// </summary>
public class MenuUI : MonoBehaviour
{
    /// <summary>The static instance of the singeton.</summary>
    public static MenuUI Instace;

    /// <summary>The central menu on the main menu.</summary>
    public GameObject MainMenu;
    /// <summary>The text that is enabled when the main scene loads.</summary>
    public GameObject GenerateText;
    /// <summary>Whether the main game is currently starting.</summary>
    [HideInInspector] public bool Starting;

    /// <summary>
    /// Initializes the singelton, sets the volume to zero and starts a fade in.
    /// </summary>
    void Awake()
    {
        if (!Instace) Instace = this;
        else Destroy(this);

        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;
        Cover.Instance.FadeIn();
    }

    /// <summary>
    /// Handles the menu switching, fades the music in and handles the starting process.
    /// </summary>
    void Update()
    {
        if (Starting)
        {
            AudioListener.volume -= Time.deltaTime * Cover.Instance.FadingSpeed;
            if (Cover.Instance.Image.color.a >= 1)
            {
                GenerateText.SetActive(true);
                SceneManager.LoadScene("MainScene");
            }
            return;
        }

        if (AudioListener.volume < 1) AudioListener.volume += Time.deltaTime * Cover.Instance.FadingSpeed;

        if (Input.GetButtonDown("Pause")) if (!MainMenu.activeSelf) SwitchToMain();
    }

    /// <summary>
    /// Opens the start menu and closes the main menu.
    /// </summary>
    public void SwitchToStart()
    {
        if (Starting) return;
        StartMenu.Instance.gameObject.SetActive(true);
        MainMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the audio menu and closes the main menu.
    /// </summary>
    public void SwitchToAudio()
    {
        if (Starting) return;
        AudioMenu.Instance.gameObject.SetActive(true);
        MainMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the graphics menu and closes the main menu.
    /// </summary>
    public void SwitchToGraphics()
    {
        if (Starting) return;
        GraphicsMenu.Instance.gameObject.SetActive(true);
        MainMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the main menu and closes all other menus.
    /// </summary>
    public void SwitchToMain()
    {
        MainMenu.SetActive(true);
        StartMenu.Instance.SetData();
        StartMenu.Instance.gameObject.SetActive(false);
        AudioMenu.Instance.gameObject.SetActive(false);
        GraphicsMenu.Instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit()
    {
        if (Starting) return;
        Application.Quit();
    }
}