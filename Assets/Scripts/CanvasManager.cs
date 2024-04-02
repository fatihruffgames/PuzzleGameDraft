using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("References")]
    public Button changeColorButton;
    public Button RestartButton;
    public Button NextButton;
    [SerializeField] Image buttonBG;
    [SerializeField] TextMeshProUGUI colorText;

    [Header("Config")]
    [SerializeField] List<Color> colors;
    [SerializeField] List<Material> colorMats;

    [Header("Debug")]
    [SerializeField] CollectibleColor currentColor;

    private void Start()
    {
        OnChangeColor();
    }

    public void OnChangeColor()
    {
        int numColors = System.Enum.GetValues(typeof(CollectibleColor)).Length;
        currentColor = (CollectibleColor)(((int)currentColor + 1) % numColors);

        if (currentColor == CollectibleColor.NONE)
        {
            currentColor = (CollectibleColor)(((int)currentColor + 1) % numColors);
        }

        UpdateColorText();
        UpdateButtonColor();

        CenterPlacementManager.instance.SetSelectedColor(currentColor);
    }

    private void UpdateColorText()
    {
        colorText.text = "CURRENT: " + currentColor.ToString();
    }

    private void UpdateButtonColor()
    {
        int colorIndex = (int)currentColor;
        buttonBG.material.color = colors[colorIndex - 1];
    }


    public void OnRestart()
    {
        GameManager.instance.OnTapRestart();
    }

    public void OnNext()
    {
        GameManager.instance.OnTapNext();
    }

}
