using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCenter : BaseColoredClass
{
    [Header("Config")]
    [SerializeField] LinkerRope ropePrefab;
    [SerializeField] float collectDuration;
    Tween scalingTween;
    public float maxDistance;

    [Header("Debug")]
    [SerializeField] List<CollectibleController> sameColoredCollectibles;


    private void Start()
    {
        GameManager.instance.LevelEndedEvent += OnLevelEnded;
    }

    private void OnLevelEnded()
    {
        if (scalingTween != null && scalingTween.IsPlaying())
        {
            scalingTween.Kill();
        }
    }

    public void Initialize(CollectibleColor collectibleColor)
    {
        SetColor(collectibleColor);
        PerformCollecting();
    }

    void PerformCollecting()
    {
        sameColoredCollectibles.AddRange(CollectibleContainer.instance.GetSameColorCollectibles(color));
        for (int i = 0; i < sameColoredCollectibles.Count; i++)
        {
            #region Distance Cheking Region
            //float distance = Vector3.Distance(sameColoredCollectibles[i].transform.position, transform.position);
            //if (distance > maxDistance) continue;
            #endregion

            sameColoredCollectibles[i].SetIsAboutToLinked(true);
            SpawnRope(sameColoredCollectibles[i], index: i);
        }

        IEnumerator CheckingRoutine()
        {
            LinkerRope[] linkerRopes = GetComponentsInChildren<LinkerRope>();
            while (!AllRopesPerformedMoving(linkerRopes))
            {
                yield return null;
            }

            TriggerReverseCollecting();
        }

        StartCoroutine(CheckingRoutine());
    }

    void SpawnRope(CollectibleController target, int index)
    {
        LinkerRope cloneRope = Instantiate(ropePrefab, transform.position, Quaternion.identity, transform);

        cloneRope.SetScale(target, index, this);
    }

    bool AllRopesPerformedMoving(LinkerRope[] linkerRopes)
    {
        for (int i = 0; i < linkerRopes.Length; i++)
        {
            if (!linkerRopes[i].PerformedMoving)
                return false;
        }
        return true;
    }

    public void DisableAllRopes()
    {
        LinkerRope[] linkerRopes = GetComponentsInChildren<LinkerRope>();
        for (int i = 0; i < linkerRopes.Length; i++)
        {
            LinkerRope linkerRope = linkerRopes[i];
            if (linkerRope != null && linkerRope.gameObject.activeInHierarchy)
                linkerRope.Disable();
        }
    }
    void TriggerReverseCollecting()
    {


        LinkerRope[] linkerRopes = GetComponentsInChildren<LinkerRope>();
        int succeededRopeCount = linkerRopes.Length;
        for (int i = 0; i < succeededRopeCount; i++)
        {
            LinkerRope linkerRope = linkerRopes[i];
            linkerRope.ReverseCollecting(collectDuration);
        }

        for (int i = 0; i < sameColoredCollectibles.Count; i++)
        {
            CollectibleController collectible = sameColoredCollectibles[i];
            if (collectible.IsLinked)
                collectible.GetCollected(collectDuration, transform.position);

        }
        CanvasManager.instance.AssignDotsColor();
        sameColoredCollectibles.Clear();
        scalingTween = transform.DOScale(Vector3.zero, .5f).SetDelay(.5f).OnComplete(() =>
           {
               if (GameManager.instance.isLevelActive)
                   CollectibleContainer.instance.TriggerCollectibleListModifiedEvent(collectedCount: succeededRopeCount);

               GameManager.instance.LevelEndedEvent -= OnLevelEnded;
               Destroy(gameObject, 0.1f);
           });
    }


}
