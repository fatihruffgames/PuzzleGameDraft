using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("References")]
    public GameObject BlockerObject;
    public Button RestartButton;
    public Button NextButton;
    [SerializeField] TextMeshProUGUI colorText;
    [SerializeField] List<ColorDotHandler> colorDots;

    [Header("Config")]
    [SerializeField] List<ColorWrapperInfo> colorWrappers;

    [Header("Debug")]
    [SerializeField] CollectibleColor currentColorEnum;


    IEnumerator Start()
    {
        CollectibleContainer.instance.CollectibleListModifiedEvent += OnCollectibleListModifiedEvent;

        yield return null;

        ColorAssign(isInit: true);
    }



    public void ColorAssign(bool isInit)
    {
        if (CollectibleContainer.instance.NoMoreCollectibleLeft) return;

        List<CollectibleColor> colorPool = CollectibleContainer.instance.GetColorEnumPool();
        ColorWrapperInfo currentWrapper;

        if (!isInit)
        {
            currentWrapper = colorDots[1].GetWrapper();
            while (!colorPool.Contains(currentWrapper.colorEnum))
            {
                currentWrapper = GetRandomAvailableWrapper();
            }
        }
        else
            currentWrapper = GetRandomAvailableWrapper();

        ColorWrapperInfo nextWrapper = GetRandomAvailableWrapper();

        colorDots[0].SetWrapperInfo(currentWrapper);
        colorDots[1].SetWrapperInfo(nextWrapper);

        currentColorEnum = currentWrapper.colorEnum;
        CenterPlacementManager.instance.SetSelectedColor(currentColorEnum);

    }

    private void OnCollectibleListModifiedEvent()
    {
        ColorAssign(isInit: false);
    }

    #region COMPLETED REGION

    public ColorWrapperInfo GetRandomAvailableWrapper()
    {
        List<CollectibleColor> colorPool = CollectibleContainer.instance.GetColorEnumPool();

        int randomIndex = Random.Range(0, colorPool.Count); // Generating random index
        CollectibleColor targetColorEnum = colorPool[randomIndex];

        for (int i = 0; i < colorWrappers.Count; i++)
        {
            if (targetColorEnum == colorWrappers[i].colorEnum)
                return colorWrappers[i];
        }
        return null;
    }
    public void OnRestart()
    {
        GameManager.instance.OnTapRestart();
    }

    public void OnNext()
    {
        GameManager.instance.OnTapNext();
    }

    #endregion
}

[System.Serializable]
public class ColorWrapperInfo
{
    public CollectibleColor colorEnum;
    public Material material;
}
//// Example usage:
//currentColorEnum = CollectibleColor.RED;
//Material assignedMaterial = colorMap[currentColorEnum];
//// Use assignedMaterial as needed