using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LinkerRope : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float delay;

    [Header("Debug")]
    public bool PerformedMoving;
    [SerializeField] const int maxDistanceThreshold = 4;
    [SerializeField] CollectibleBoxController targetCollectible;

    public void SetScale(CollectibleBoxController target, int index)
    {
        targetCollectible = target;
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(index * delay);
            float distance = Vector3.Distance(target.transform.position, transform.position);
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.up = dir;

            bool exceedsTheThreshold = distance > maxDistanceThreshold;
            distance = Mathf.Min(distance, maxDistanceThreshold);

            float newScale = distance / 2;

            transform.DOScaleY(newScale, .5f).OnComplete(() =>
            {
                if (!exceedsTheThreshold)
                {
                    target.GetLinked();
                }
                else
                {
                    targetCollectible.SetIsAboutToLinked(false);
                    ReverseCollecting(.2f);
                }
                PerformedMoving = true;

            });
        }

        StartCoroutine(Routine());
    }

    bool reverseCollectingInvoked;
    public void ReverseCollecting(float duration)
    {
        if (reverseCollectingInvoked) return;
        reverseCollectingInvoked = true;

        transform.DOScaleY(0, duration).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }


    private void OnTriggerEnter(Collider other)
    {
        if (PerformedMoving) return;
        if (targetCollectible == null) return;
        if (!other.TryGetComponent(out CollectibleBoxController collectible)) return;
        if (targetCollectible == collectible) return;

        StopTween();
    }

    private void StopTween()
    {
        PerformedMoving = true;
        targetCollectible.SetIsAboutToLinked(false);
        transform.DOKill(); // Stop all ongoing tweens on this object
        ReverseCollecting(.2f);
    }

}
