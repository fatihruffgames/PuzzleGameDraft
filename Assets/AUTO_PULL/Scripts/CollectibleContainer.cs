using System.Collections.Generic;
using UnityEngine;

public class CollectibleContainer : MonoSingleton<CollectibleContainer>
{
    public event System.Action<int> CollectibleListModifiedEvent;

    [Header("Debug")]
    public List<CollectibleController> Collectibles = new List<CollectibleController>();
    public bool NoMoreCollectibleLeft;

    public void AddCollectible(CollectibleController collectible)
    {
        Collectibles.Add(collectible);
    }
    public void RemoveCollectible(CollectibleController collectible)
    {
        Collectibles.Remove(collectible);

        /* if (Collectibles.Count == 0)
         {
             NoMoreCollectibleLeft = true;
             GameManager.instance.EndGame(success: true, delayAsSeconds: 1);
         }*/

    }
    public List<CollectibleController> GetSameColorCollectibles(CollectibleColor color)
    {
        List<CollectibleController> closestCollectibles = new List<CollectibleController>();

        if (Collectibles.Count == 0)
        {
            Debug.LogWarning("No collectibles in the container.");
            return closestCollectibles;
        }

        for (int i = 0; i < Collectibles.Count; i++)
        {
            CollectibleController collectible = Collectibles[i];
            if (collectible.CheckIfCanCollected(color))
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
            CollectibleController collectible = Collectibles[i];
            if (!pool.Contains(collectible.GetColor()))
            {
                pool.Add(collectible.GetColor());
            }
        }

        return pool;
    }

    public void TriggerCollectibleListModifiedEvent(int collectedCount)
    {
        CollectibleListModifiedEvent?.Invoke(collectedCount);
    }
}
