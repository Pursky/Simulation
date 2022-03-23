using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singelton for the start menu.
/// </summary>
public class StartMenu : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static StartMenu Instance;

    /// <summary>The generate data, that modifies the terrain generation.</summary>
    public GenerateData GenerateData;
    /// <summary>The sliders that control the generate data.</summary>
    public Slider[] Sliders;

    /// <summary>
    /// Initializes the singelton and sets multiple values.
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        if (GenerateData.ActorDensity == 0) SetDefaultData();

        Sliders[0].value = GenerateData.ActorDensity;
        Sliders[1].value = GenerateData.DetailDensity;
        Sliders[2].value = GenerateData.TerrainScale;
        Sliders[3].value = GenerateData.TerrainRoughness;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Writes default values into the generate data.
    /// </summary>
    public void SetDefaultData()
    {
        GenerateData.ActorDensity = 1;
        GenerateData.DetailDensity = 1;
        GenerateData.TerrainScale = 1;
        GenerateData.TerrainRoughness = 1;

        foreach(Slider slider in Sliders) slider.value = 1;
    }

    /// <summary>
    /// Writes the values from the sliders into the generate data.
    /// </summary>
    public void SetData()
    {
        GenerateData.ActorDensity = Sliders[0].value;
        GenerateData.DetailDensity = Sliders[1].value;
        GenerateData.TerrainScale = Sliders[2].value;
        GenerateData.TerrainRoughness = Sliders[3].value;
    }

    /// <summary>
    /// Triggers a fade out before the main scene is loaded.
    /// </summary>
    public void StartGame()
    {
        MenuUI.Instace.Starting = true;
        Cover.Instance.FadeOut();
        SetData();
        MenuUI.Instace.SwitchToMain();
    }
}