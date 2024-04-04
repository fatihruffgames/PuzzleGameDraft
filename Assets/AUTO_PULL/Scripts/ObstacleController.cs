using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private void Start()
    {
        SpawnManager.instance.AddToList(transform);
    }
}
