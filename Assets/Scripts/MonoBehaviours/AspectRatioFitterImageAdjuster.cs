using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(Image))]
public class AspectRatioFitterImageAdjuster : MonoBehaviour
{
    AspectRatioFitter arf;
    private Image image;

    private void Awake()
    {
        arf = GetComponent<AspectRatioFitter>();
        image = GetComponent<Image>();

    }

    public void Adjust()
    {
        arf.aspectRatio = image.sprite.bounds.size.x / image.sprite.bounds.size.y;
    }

}
