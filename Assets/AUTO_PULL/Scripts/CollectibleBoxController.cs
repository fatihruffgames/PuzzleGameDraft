using DG.Tweening;
using UnityEngine;


public class CollectibleBoxController : BaseColoredClass
{
    public bool IsLinked;
    public bool IsAboutToLinked;
    private void Awake()
    {
        SetColor();
    }
    private void Start()
    {
        CollectibleContainer.instance.AddCollectible(this);
        int index = CollectibleContainer.instance.Collectibles.IndexOf(this);
        gameObject.name = color.ToString() + "_" + index;

        GameManager.instance.LevelEndedEvent += OnLevelEnded;
    }

    private void OnLevelEnded()
    {
        // Stop the sequence if it's playing
        if (sequence != null && sequence.IsPlaying())
        {
            sequence.Kill(); // Stop the sequence
        }
    }

    public void SetIsAboutToLinked(bool _isAboutToLinked)
    {
        IsAboutToLinked = _isAboutToLinked;
    }

    public void GetLinked()
    {
        IsLinked = true;
    }
    private Sequence sequence;
    public void GetCollected(float duration, Vector3 targetCenter)
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetCenter, duration));
        sequence.Join(transform.DOScale(Vector3.zero, duration * 2));
        sequence.OnComplete(() =>
        {
            GameManager.instance.LevelEndedEvent -= OnLevelEnded;
            Destroy(gameObject, .1f);
        });
        sequence.Play();

        CollectibleContainer.instance.RemoveCollectible(this);


    }
}
