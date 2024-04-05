using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorDotHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image image;

    public ColorWrapperInfo colorWrapperInfo;

    public void SetWrapperInfo(ColorWrapperInfo newWrapper)
    {
        colorWrapperInfo = newWrapper;
        SetMaterialAndEnum(  colorWrapperInfo.colorEnum, colorWrapperInfo.sprite);
    }
    void SetMaterialAndEnum( CollectibleColor _colorEnum, Sprite sprite = null)
    { 
        image.sprite = sprite;
    }

    public ColorWrapperInfo GetWrapper()
    {
        return colorWrapperInfo;
    }
}
