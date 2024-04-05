using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public event System.Action LevelChangedEvent;
    public event System.Action LevelEndedEvent;
    public event System.Action LevelFailedEvent;

    [SerializeField] int totalSceneCount;

    [Header("Debug")] public bool isLevelActive;
    [HideInInspector] public int MatchedCount;
    int totalBlockCount;
    int fallenBlocks;

    protected override void Awake()
    {
        base.Awake();

        isLevelActive = true;
    }
    public void AddBlock()
    {
        totalBlockCount++;
    }
    void IncreaseFallenCount()
    {
        fallenBlocks++;
        if (fallenBlocks == totalBlockCount)
            ChangeLevelInfo();
    }

    void ChangeLevelInfo()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentSceneIndex + 1;

        if (nextScene >= totalSceneCount) nextScene = 0;

        SceneManager.LoadScene(nextScene);

        LevelChangedEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BlockHolderController holder))
        {
            IncreaseFallenCount();
            Destroy(holder, .1f);
        }
    }

    public void OnTapRestart()
    {
        LevelEndedEvent?.Invoke();

        isLevelActive = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void OnTapNext()
    {
        LevelEndedEvent?.Invoke();
        isLevelActive = false;

        #region  Cumulative Next Level
        /*
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextScene = currentSceneIndex + 1;

                if (nextScene >= totalSceneCount) nextScene = 0;
                SceneManager.LoadScene(nextScene);*/
        #endregion

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndGame(bool success, float delayAsSeconds = 0)
    {
        if (!isLevelActive) return;

        isLevelActive = false;        
        IEnumerator EndingRoutine()
        {
            if (!success) LevelFailedEvent?.Invoke();
            yield return new WaitForSeconds(delayAsSeconds);

            if (success)
                OnTapNext();
            else
                OnTapRestart();
        }
        StartCoroutine(EndingRoutine());
    }

    public void OnNoMoveLeft()
    {
        if (!isLevelActive) return;

        IEnumerator CheckingRoutine()
        {
            yield return new WaitForSeconds(1f);

            if (!CanvasManager.instance.progressBarManager.IsBarFilled)
                EndGame(success: false, 1f);
        }

        StartCoroutine(CheckingRoutine());
    }

    public void IncreaseMatchCount()
    {
        MatchedCount++;
    }
}

[System.Serializable]
public class BlockInfo
{
    public List<GameObject> Blocks = new List<GameObject>();
}