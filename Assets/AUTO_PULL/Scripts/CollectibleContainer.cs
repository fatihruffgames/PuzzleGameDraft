using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleContainer : MonoSingleton<CollectibleContainer>
{
    public event System.Action CollectibleListModifiedEvent;

    [Header("Debug")]
    public List<CollectibleBoxController> Collectibles = new List<CollectibleBoxController>();
    public bool NoMoreCollectibleLeft;

    public void AddCollectible(CollectibleBoxController collectible)
    {
        Collectibles.Add(collectible);
    }
    public void RemoveCollectible(CollectibleBoxController collectible)
    {
        Collectibles.Remove(collectible);

        if (Collectibles.Count == 0)
        {
            NoMoreCollectibleLeft = true;
            IEnumerator EndRoutine()
            {
                yield return new WaitForSeconds(1);
                GameManager.instance.OnTapNext();
            }
            StartCoroutine(EndRoutine());
        }

    }
    public List<CollectibleBoxController> GetSameColorCollectibles(CollectibleColor color)
    {
        List<CollectibleBoxController> closestCollectibles = new List<CollectibleBoxController>();

        if (Collectibles.Count == 0)
        {
            Debug.LogWarning("No collectibles in the container.");
            return closestCollectibles;
        }

        for (int i = 0; i < Collectibles.Count; i++)
        {
            CollectibleBoxController collectible = Collectibles[i];
            if (collectible.GetColor() == color && !collectible.IsAboutToLinked && !collectible.IsLinked)
            {
                closestCollectibles.Add(collectible);
            }
        }

        return closestCollectibles;
    }

    public List<CollectibleColor> GetColorEnumPool()
    {
        List<CollectibleColor> pool = new List<CollectibleColor>();
        for (int i = 0; i < Collectibles.Count; i++)
        {
            CollectibleBoxController collectible = Collectibles[i];
            if (!pool.Contains(collectible.GetColor()))
            {
                pool.Add(collectible.GetColor());
            }
        }

        return pool;
    }

    public void TriggerCollectibleListModifiedEvent()
    {
        CollectibleListModifiedEvent?.Invoke();
    }
}
