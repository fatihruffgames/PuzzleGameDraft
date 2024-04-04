using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int desiredCollectibleCount;
    public bool IsBarFilled;

    [Header("References")]
    [SerializeField] private Slider progressBarFill;
    [SerializeField] TextMeshProUGUI countText;

    [Header("Debug")]
    private int collectedCollectibles = 0;

    void Awake()
    {
        SetText();
    }
    public void IncrementCollectedCount()
    {
        collectedCollectibles++;
        SetProgress();
        SetText();
    }


    void SetProgress()
    {
        float progress = 1;
        if (collectedCollectibles < desiredCollectibleCount)
            progress = (float)collectedCollectibles / desiredCollectibleCount;
        else
        {
            IsBarFilled = true;
            GameManager.instance.EndGame(success: true, delayAsSeconds: 1f);
        }
        progressBarFill.value = progress;
    }

    private void SetText()
    {
        if (collectedCollectibles < desiredCollectibleCount)
            countText.text = collectedCollectibles + "/" + desiredCollectibleCount;
        else
            countText.text = string.Empty;
    }
}
