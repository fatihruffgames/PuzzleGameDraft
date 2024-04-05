using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("References")]
    public GameObject BlockerObject;
    public Button RestartButton;
    public Button NextButton;
    [SerializeField] CanvasGroup failPanel;
    [SerializeField] TextMeshProUGUI moveCountText;
    [SerializeField] List<ColorDotHandler> colorDots;

    [Header("Config")]
    [SerializeField] List<ColorWrapperInfo> colorWrappers;

    [Header("Debug")]
    [SerializeField] CollectibleColor currentColorEnum;
    [HideInInspector] public ProgressBarManager progressBarManager;

    IEnumerator Start()
    {
       // CollectibleContainer.instance.CollectibleListModifiedEvent += OnCollectibleListModifiedEvent;
        GameManager.instance.LevelFailedEvent += OnLevelFailed;
        progressBarManager = GetComponentInChildren<ProgressBarManager>();
        yield return null;

        ColorAssign(isInit: true);
    }

    private void OnLevelFailed()
    {
        failPanel.DOFade(1, .5f);
    }

    public void SetProgress()
    {
        progressBarManager.IncrementCollectedCount();
    }
    public void SetMoveCountText(int leftMoveCount)
    {
        moveCountText.text = "Move Count: " + leftMoveCount.ToString();
    }
    public void ColorAssign(bool isInit)
    {
        if (CollectibleContainer.instance.NoMoreCollectibleLeft) return;

        List<CollectibleColor> colorPool = CollectibleContainer.instance.GetColorEnumPool();
        ColorWrapperInfo currentWrapper;

        if (!isInit)
        {
            currentWrapper = colorDots[1].GetWrapper();
            /*  while (!colorPool.Contains(currentWrapper.colorEnum))
              {
                  currentWrapper = GetRandomAvailableWrapper();
              }*/

        }
        else
            currentWrapper = GetFullyRandomWrapper();

        ColorWrapperInfo nextWrapper = GetFullyRandomWrapper();

        colorDots[0].SetWrapperInfo(currentWrapper);
        colorDots[1].SetWrapperInfo(nextWrapper);

        currentColorEnum = currentWrapper.colorEnum;
        CenterPlacementManager.instance.SetSelectedColor(currentColorEnum);

    }

    public void AssignDotsColor()
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

    public ColorWrapperInfo GetFullyRandomWrapper()
    {
        int randomIndex = Random.Range(0, colorWrappers.Count); // Generating random 
        return colorWrappers[randomIndex];
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
    public Sprite sprite; 
}