using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] int spawnCount;
    [SerializeField] private CollectibleController collectiblePrefab;
    [SerializeField] private Transform spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;
    [SerializeField] private float minDistanceBetweenCollectibles;


    [SerializeField] private List<Transform> spawnedCollectibleTransforms = new List<Transform>();
    private void Start()
    {
        CollectibleContainer.instance.CollectibleListModifiedEvent += OnCollectibleListModified;

        SpawnCollectibles(spawnCount);

    }

    private void OnCollectibleListModified(int collectedCount)
    {
        SpawnCollectibles(collectedCount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnCollectibles(spawnCount);
        }
    }
    public void AddToList(Transform collectedPos)
    {
        spawnedCollectibleTransforms.Add(collectedPos);
    }

    public void RemoveFromList(Transform collectedPos)
    {
        spawnedCollectibleTransforms.Remove(collectedPos);
    }

    public void SpawnCollectibles(int count)
    {
        int iterateCount = 0;
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = GetRandomPositionInSpawnArea();

            // Check if the new position is far enough from the previous spawned positions
            bool isValidPosition = IsPositionValid(randomPosition);

            if (isValidPosition)
            {
                CollectibleController clone = Instantiate(collectiblePrefab, randomPosition, Quaternion.identity);
                clone.Initialize();
                spawnedCollectibleTransforms.Add(clone.transform);
            }
            else
            {
                // Try again to find a valid position
                iterateCount++;
                if (iterateCount >= 20)
                {
                    Debug.LogWarning("NO valid position is found, iterate count exceeds the limit.");
                    continue;
                }
                else
                    i--;
            }
        }
    }

    private Vector3 GetRandomPositionInSpawnArea()
    {
        float randomX = Random.Range(spawnAreaCenter.position.x - spawnAreaSize.x / 2f, spawnAreaCenter.position.x + spawnAreaSize.x / 2f);
        float fixedY = 0.5f;
        float randomZ = Random.Range(spawnAreaCenter.position.z - spawnAreaSize.z / 2f, spawnAreaCenter.position.z + spawnAreaSize.z / 2f);

        return new Vector3(randomX, fixedY, randomZ);
    }

    private bool IsPositionValid(Vector3 position)
    {
        for (int i = 0; i < spawnedCollectibleTransforms.Count; i++)
        {
            Vector3 spawnedPosition = spawnedCollectibleTransforms[i].position;
            if (Vector3.Distance(position, spawnedPosition) < minDistanceBetweenCollectibles)
            {
                return false;
            }
        }
        return true;
    }
}
