using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

/// <summary>
/// A singelton for the graphics menu.
/// </summary>
public class GraphicsMenu : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static GraphicsMenu Instance;

    /// <summary>The toggles in the graphics menu.</summary>
    public Toggle[] GraphicsToggles;
    /// <summary>The sliders in the graphics menu.</summary>
    public Slider[] GraphicsSliders;
    public PostProcessProfile Profile;

    /// <summary>
    /// Initializes the singelton and sets multiple values.
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        if (!PlayerPrefs.HasKey("MasterVolume")) SetDefaultData();
        ApplyData();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Applies the slider and toggle values to the graphics.
    /// </summary>
    public void UpdateGraphics()
    {
        if (MainCamera.Instance)
        {
            MainCamera.Instance.RenderDistance = GraphicsSliders[0].value;
            MainCamera.Instance.UpdateRenderDistance();
        }

        QualitySettings.masterTextureLimit = (int)GraphicsSliders[1].value;

        QualitySettings.vSyncCount = GraphicsToggles[0].isOn ? 1 : 0;
        Screen.fullScreen = GraphicsToggles[1].isOn;
        Profile.GetSetting<MotionBlur>().active = GraphicsToggles[2].isOn;

        SetData();
    }

    /// <summary>
    /// Resets all the slider and toggle values back to default.
    /// </summary>
    public void ResetGraphics()
    {
        GraphicsSliders[0].value = 75;
        GraphicsSliders[1].value = 0;
        GraphicsToggles[0].isOn = true;
        GraphicsToggles[1].isOn = true;
        GraphicsToggles[2].isOn = true;

        UpdateGraphics();
    }

    /// <summary>
    /// Writes default values for the graphics into the player prefs.
    /// </summary>
    public void SetDefaultData()
    {
        PlayerPrefs.SetFloat("RenderDistance", 75);
        PlayerPrefs.SetInt("TextureQuality", 0);
        PlayerPrefs.SetInt("VSync", 1);
        PlayerPrefs.SetInt("Fullscreen", 1);
        PlayerPrefs.SetInt("MotionBlur", 1);
    }

    /// <summary>
    /// Writes the values from the sliders and toggles into the player prefs.
    /// </summary>
    public void SetData()
    {
        PlayerPrefs.SetFloat("RenderDistance", GraphicsSliders[0].value);
        PlayerPrefs.SetInt("TextureQuality", (int)GraphicsSliders[1].value);

        if (GraphicsToggles[0].isOn) PlayerPrefs.SetInt("VSync", 1);
        else PlayerPrefs.SetInt("VSync", 0);

        if (GraphicsToggles[1].isOn) PlayerPrefs.SetInt("Fullscreen", 1);
        else PlayerPrefs.SetInt("Fullscreen", 0);

        if (GraphicsToggles[2].isOn) PlayerPrefs.SetInt("MotionBlur", 1);
        else PlayerPrefs.SetInt("MotionBlur", 0);
    }

    /// <summary>
    /// Reads the player prefs and applies their values to the sliders.
    /// </summary>
    public void ApplyData()
    {
        GraphicsSliders[0].value = PlayerPrefs.GetFloat("RenderDistance");
        GraphicsSliders[1].value = PlayerPrefs.GetInt("TextureQuality");
        GraphicsToggles[0].isOn = PlayerPrefs.GetInt("VSync") == 1;
        GraphicsToggles[1].isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        GraphicsToggles[2].isOn = PlayerPrefs.GetInt("MotionBlur") == 1;

        UpdateGraphics();
    }

    /// <summary>
    /// Updates the graphics when the menu is closed.
    /// </summary>
    private void OnDisable()
    {
        UpdateGraphics();
    }
}