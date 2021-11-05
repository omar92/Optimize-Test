using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(Image))]
public class AspectRatioFitterImageAdjuster : MonoBehaviour
{
    //this component is used to keep the image aspect ration after image is changed 
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
