using ScriptableSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetSoSpriteToImage : MonoBehaviour
{
    [SerializeField] SpriteSO spriteSO;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImage()
    {
        image.sprite = spriteSO.Value;
    }

}
