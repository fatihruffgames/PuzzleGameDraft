using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public event System.Action LevelChangedEvent;
    public event System.Action LevelEndedEvent;

    [SerializeField] int totalSceneCount;

    [Header("Debug")]
    [SerializeField] int totalBlockCount;
    [SerializeField] int fallenBlocks;
    public int currentInfoIndex;


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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnTapNext()
    {
        LevelEndedEvent?.Invoke();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentSceneIndex + 1;

        if (nextScene >= totalSceneCount) nextScene = 0;
        SceneManager.LoadScene(nextScene);

    }
    /// <summary>
    /// //////////////
    /// </summary>
    /// 

    public int MatchedCount;
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