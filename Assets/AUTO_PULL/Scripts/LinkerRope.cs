using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LinkerRope : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float movingDelay;
    [SerializeField] int maxDistanceThreshold;

    [Header("Debug")]
    public bool PerformedMoving;
    [SerializeField] CollectibleController targetCollectible;
    bool reverseCollectingInvoked;
    Tween reverseTween;
    Tween scalingTween;
    CollectCenter myCenter;
    GameObject mesh;
    private void Start()
    {
        mesh = transform.GetChild(0).gameObject;
        GameManager.instance.LevelEndedEvent += OnLevelEnded;
    }

    private void OnLevelEnded()
    {
        KillAllAssignedTweens();
    }

    private void KillAllAssignedTweens()
    {
        if (reverseTween != null && reverseTween.IsPlaying())
        {
            reverseTween.Kill();
        }
    }

    public void SetScale(CollectibleController target, int index, CollectCenter center)
    {
        myCenter = center;
        targetCollectible = target;
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(/*index **/ movingDelay);
            float distance = Vector3.Distance(target.transform.position, transform.position);
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.up = dir;
            #region Distance Check Region
            //bool exceedsTheThreshold = distance > maxDistanceThreshold;
            //distance = Mathf.Min(distance, maxDistanceThreshold);
            #endregion

            float newScale = distance / 2;

            scalingTween = transform.DOScaleY(newScale, .5f).OnComplete(() =>
            {
                //if (!exceedsTheThreshold)
                //{
                //   target.GetLinked();
                //}
                //else
                //{
                //    targetCollectible.SetIsAboutToLinked(false);
                //    ReverseCollecting(.2f);
                //}
                if (!mesh.activeInHierarchy) return;

                target.GetLinked();
                PerformedMoving = true;

            });
        }

        StartCoroutine(Routine());
    }


    public void ReverseCollecting(float duration)
    {
        if (reverseCollectingInvoked) return;
        reverseCollectingInvoked = true;

        reverseTween = transform.DOScaleY(0, duration).OnComplete(() =>
          {
              DestroySelf();
          });
    }

    void DestroySelf()
    {
        GameManager.instance.LevelEndedEvent -= OnLevelEnded;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ObstacleController _))
        {
            if (!GameManager.instance.isLevelActive) return;

            myCenter.DisableAllRopes();
            GameManager.instance.EndGame(success: false, delayAsSeconds: 1.5f);
        }


        if (PerformedMoving) return;
        if (targetCollectible == null) return;
        if (!other.TryGetComponent(out CollectibleController collectible)) return;
        if (targetCollectible == collectible) return;

        StopTween();
    }
    public void Disable()
    {
        mesh.SetActive(false);
    }
    private void StopTween()
    {
        PerformedMoving = true;
        targetCollectible.SetIsAboutToLinked(false);
        transform.DOKill(); // Stop all ongoing tweens on this object
        ReverseCollecting(.2f);
    }

}
