using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleContainer : MonoSingleton<CollectibleContainer>
{
    [Header("Debug")]
    public List<CollectibleBoxController> Collectibles = new List<CollectibleBoxController>();


    public void AddCollectible(CollectibleBoxController collectible)
    {
        Collectibles.Add(collectible);
    }
    public void RemoveCollectible(CollectibleBoxController collectible)
    {
        Collectibles.Remove(collectible);

        if(Collectibles.Count == 0)
        {
            Debug.Log("LEVEL SUCCEEDED");
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
}
