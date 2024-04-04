using TMPro;
using UnityEngine;

public class ColorDotHandler : MonoBehaviour
{
    [SerializeField] Material _material;
    [SerializeField] TextMeshProUGUI _text;

    public ColorWrapperInfo colorWrapperInfo;

    public void SetWrapperInfo(ColorWrapperInfo newWrapper)
    {
        colorWrapperInfo = newWrapper;
        SetMaterialAndEnum(colorWrapperInfo.material, colorWrapperInfo.colorEnum);
    }
    void SetMaterialAndEnum(Material newMat, CollectibleColor _colorEnum)
    {
        _material.color = newMat.color;
        //  _text.text = colorWrapperInfo.colorEnum.ToString();
    }

    public ColorWrapperInfo GetWrapper()
    {
        return colorWrapperInfo;
    }
}
