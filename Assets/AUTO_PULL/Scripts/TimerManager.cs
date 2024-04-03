using UnityEngine;

public class TimerManager : MonoSingleton<TimerManager>
{
    public event System.Action OnTimerStart;
    public event System.Action OnTimerFinish;


    [Header("Config")]
    [SerializeField] private float countdownDuration = 60f; // Default countdown duration in seconds

    [Header("Debug")]
    [SerializeField] float currentTime = 0f;
    private bool isRunning = false;


    protected override void Awake()
    {
        base.Awake();

        StartTimer();
    }
    private void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                FinishTimer();
            }
        }
    }

    public void StartTimer(float durationInSeconds = -1)
    {
        if (durationInSeconds > 0)
        {
            countdownDuration = durationInSeconds;
        }
        currentTime = countdownDuration;
        isRunning = true;
        OnTimerStart?.Invoke();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void FinishTimer()
    {
        isRunning = false;
        OnTimerFinish?.Invoke();
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public bool IsTimerRunning()
    {
        return isRunning;
    }
}
