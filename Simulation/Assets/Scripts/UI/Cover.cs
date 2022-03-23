using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singelton for a rectangle that covers the screen and can fade in and out.
/// </summary>
public class Cover : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static Cover Instance;

    /// <summary>The speed that the cover fades at.</summary>
    public float FadingSpeed;

    /// <summary>The image of the cover.</summary>
    [HideInInspector] public Image Image;
    
    /// <summary>An enum for a fading status.</summary>
    private enum FadingStatus { Neutral, FadingIn, FadingOut }
    /// <summary>The current fading status.</summary>
    private FadingStatus fadingStatus = FadingStatus.Neutral;

    /// <summary>
    /// Initializes the singelton and the image.
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(Instance);

        Image = GetComponent<Image>();
        Image.color = Color.black;
    }

    /// <summary>
    /// Handles the fading.
    /// </summary>
    void Update()
    {
        switch (fadingStatus)
        {
            case FadingStatus.FadingIn:
                Image.color -= Color.black * FadingSpeed * Time.deltaTime;
                break;
            case FadingStatus.FadingOut:
                Image.color += Color.black * FadingSpeed * Time.deltaTime;
                break;
        }

        if (Image.color.a >= 1 && fadingStatus == FadingStatus.FadingOut)
        {
            Image.color = Color.black;
            fadingStatus = FadingStatus.Neutral;
        }

        if (Image.color.a <= 0 && fadingStatus == FadingStatus.FadingIn)
        {
            Image.color = Color.clear;
            fadingStatus = FadingStatus.Neutral;
        }
    }

    /// <summary>
    /// Starts a fade out.
    /// </summary>
    public void FadeOut()
    {
        fadingStatus = FadingStatus.FadingOut;
    }

    /// <summary>
    /// Starts a fade in.
    /// </summary>
    public void FadeIn()
    {
        fadingStatus = FadingStatus.FadingIn;
    }
}