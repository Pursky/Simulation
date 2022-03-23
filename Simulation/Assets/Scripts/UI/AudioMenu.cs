using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// A singelton for the audio menu.
/// </summary>
public class AudioMenu : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static AudioMenu Instance;

    /// <summary>The sliders that control the volume.</summary>
    public Slider[] AudioSliders;
    /// <summary>The main audio mixer.</summary>
    public AudioMixer Mixer;

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
    /// Applies the slider values to thir respective audio mixer groups.
    /// </summary>
    public void UpdateAudio()
    {
        if (AudioSliders[0].value == -12) Mixer.SetFloat("MasterVolume", -80);
        else Mixer.SetFloat("MasterVolume", AudioSliders[0].value);

        if (AudioSliders[1].value == -12) Mixer.SetFloat("ActorsVolume", -80);
        else Mixer.SetFloat("ActorsVolume", AudioSliders[1].value);

        if (AudioSliders[2].value == -12) Mixer.SetFloat("AmbienceVolume", -80);
        else Mixer.SetFloat("AmbienceVolume", AudioSliders[2].value);

        if (AudioSliders[3].value == -12) Mixer.SetFloat("OtherVolume", -80);
        else Mixer.SetFloat("OtherVolume", AudioSliders[3].value);

        SetData();
    }

    /// <summary>
    /// Resets all the slider values back to default.
    /// </summary>
    public void ResetAudio()
    {
        AudioSliders[0].value = 0;
        AudioSliders[1].value = 0;
        AudioSliders[2].value = 0;
        AudioSliders[3].value = 0;

        UpdateAudio();
    }

    /// <summary>
    /// Writes default values for the volumes into the player prefs.
    /// </summary>
    public void SetDefaultData()
    {
        PlayerPrefs.SetFloat("MasterVolume", 0);
        PlayerPrefs.SetFloat("ActorsVolume", 0);
        PlayerPrefs.SetFloat("AmbienceVolume", 0);
        PlayerPrefs.SetFloat("OtherVolume", 0);
    }

    /// <summary>
    /// Writes the values from the sliders into the player prefs.
    /// </summary>
    public void SetData()
    {
        PlayerPrefs.SetFloat("MasterVolume", AudioSliders[0].value);
        PlayerPrefs.SetFloat("ActorsVolume", AudioSliders[1].value);
        PlayerPrefs.SetFloat("AmbienceVolume", AudioSliders[2].value);
        PlayerPrefs.SetFloat("OtherVolume", AudioSliders[3].value);
    }

    /// <summary>
    /// Reads the player prefs and applies their volumes to the sliders.
    /// </summary>
    public void ApplyData()
    {
        AudioSliders[0].value = PlayerPrefs.GetFloat("MasterVolume");
        AudioSliders[1].value = PlayerPrefs.GetFloat("ActorsVolume");
        AudioSliders[2].value = PlayerPrefs.GetFloat("AmbienceVolume");
        AudioSliders[3].value = PlayerPrefs.GetFloat("OtherVolume");

        UpdateAudio();
    }

    /// <summary>
    /// Updates the volumes when the menu is closed.
    /// </summary>
    private void OnDisable()
    {
        UpdateAudio();
    }
}