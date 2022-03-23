using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A singelton for the UI controller in the main scene.
/// </summary>
public class MainUI : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static MainUI Instance;

    /// <summary>The pause menu of the main scene.</summary>
    public GameObject PauseMenu;
    /// <summary>The menu explaining the controlls.</summary>
    public GameObject Controlls;

    /// <summary>Whether the game is paused.</summary>
    [HideInInspector] public bool Paused;

    /// <summary>Whether the game is currently quitting.</summary>
    private bool quitting;

    /// <summary>
    /// Initializes the singelton, sets the volume to zero and starts a fade in.
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
        
        AudioListener.volume = 0;

        Cover.Instance.FadeIn();
    }

    /// <summary>
    /// Handles the menu switching, fades the music in and handles the quitting process.
    /// </summary>
    void Update()
    {
        if (quitting)
        {
            if (Cover.Instance.Image.color.a >= 1) SceneManager.LoadScene("MainMenu");
            AudioListener.volume -= Time.deltaTime * Cover.Instance.FadingSpeed;
            return;
        }

        if (AudioListener.volume < 1) AudioListener.volume += Time.deltaTime * Cover.Instance.FadingSpeed;

        if (Input.GetButtonDown("Pause") && !quitting)
        {
            if (Paused)
            {
                if (PauseMenu.activeSelf) Unpause();
                else SwitchToPaused();
            }
            else Pause();
        }

    }

    /// <summary>
    /// Pauses the game and opens the pause menu.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        PauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;
        Paused = true;
    }

    /// <summary>
    /// Unpauses the game and closes the pause menu.
    /// </summary>
    public void Unpause()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
        Paused = false;
    }

    /// <summary>
    /// Opens the audio menu and closes the pause menu.
    /// </summary>
    public void SwitchToAudio()
    {
        AudioMenu.Instance.gameObject.SetActive(true);
        PauseMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the graphics menu and closes the pause menu.
    /// </summary>
    public void SwitchToGraphics()
    {
        GraphicsMenu.Instance.gameObject.SetActive(true);
        PauseMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the controlls menu and closes the pause menu.
    /// </summary>
    public void SwitchToControlls()
    {
        Controlls.SetActive(true);
        PauseMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the pause menu and closes all other menus.
    /// </summary>
    public void SwitchToPaused()
    {
        PauseMenu.SetActive(true);
        Controlls.SetActive(false);
        AudioMenu.Instance.gameObject.SetActive(false);
        GraphicsMenu.Instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets default save values.
    /// </summary>
    public void SetDefaultData()
    {
        GraphicsMenu.Instance.SetDefaultData();
        AudioMenu.Instance.SetDefaultData();
    }

    /// <summary>
    /// Starts the quitting process.
    /// </summary>
    public void Quit()
    {
        quitting = true;
        Unpause();
        Cover.Instance.FadeOut();
    }
}