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
    [SerializeField] List<CollectibleBoxController> sameColoredCollectibles;


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
            float distance = Vector3.Distance(sameColoredCollectibles[i].transform.position, transform.position);
            if (distance > maxDistance) continue;
            

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

    void SpawnRope(CollectibleBoxController target, int index)
    {
        LinkerRope cloneRope = Instantiate(ropePrefab, transform.position, Quaternion.identity, transform);

        cloneRope.SetScale(target, index);
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


    void TriggerReverseCollecting()
    {
        LinkerRope[] linkerRopes = GetComponentsInChildren<LinkerRope>();
        for (int i = 0; i < linkerRopes.Length; i++)
        {
            LinkerRope linkerRope = linkerRopes[i];
            linkerRope.ReverseCollecting(collectDuration);
        }

        for (int i = 0; i < sameColoredCollectibles.Count; i++)
        {
            CollectibleBoxController collectible = sameColoredCollectibles[i];
            if (collectible.IsLinked)
                collectible.GetCollected(collectDuration, transform.position);

        }

        sameColoredCollectibles.Clear();
        CollectibleContainer.instance.TriggerCollectibleListModifiedEvent();
        scalingTween = transform.DOScale(Vector3.zero, .5f).SetDelay(.5f).OnComplete(() =>
           {
               GameManager.instance.LevelEndedEvent -= OnLevelEnded;
               Destroy(gameObject, 0.1f);
           });
    }


}
