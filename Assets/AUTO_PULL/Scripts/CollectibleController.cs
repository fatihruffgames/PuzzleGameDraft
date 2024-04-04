using DG.Tweening;
using UnityEngine;


public class CollectibleController : BaseColoredClass
{
    public bool IsLinked;
    public bool IsAboutToLinked;
    public bool CanBeCollected;

    private void Start()
    {
        CollectibleContainer.instance.AddCollectible(this);
        int index = CollectibleContainer.instance.Collectibles.IndexOf(this);
        gameObject.name = color.ToString() + "_" + index;

        GameManager.instance.LevelEndedEvent += OnLevelEnded;
    }
    public void Initialize()
    {

        transform.DOScale(transform.localScale, .5f).From(Vector3.zero).OnComplete(() =>
        {
            CanBeCollected = true;
        });

        int maxEnum = System.Enum.GetValues(typeof(CollectibleColor)).Length;
        int randomValue = Random.Range(1, maxEnum);

        CollectibleColor randomColor = (CollectibleColor)randomValue;
        color = randomColor;

        SetColor();
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
        SpawnManager.instance.RemoveFromList(transform);
        CanvasManager.instance.SetProgress();
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

    public bool CheckIfCanCollected(CollectibleColor collecCenterColor)
    {
        bool canCollected;

        if (!CanBeCollected) canCollected = false;
        else
        {
            if (!IsAboutToLinked && !IsLinked && collecCenterColor == color)
                canCollected = true;
            else
                canCollected = false;
        }


        return canCollected;
    }
}
