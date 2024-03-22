using UnityEngine;

public class BoardHolder : MonoSingleton<BoardHolder>
{
    public CellController GetClosestCell(Vector3 from)
    {
        Transform closestChild = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float distance = Vector3.Distance(from, child.position);

            if (distance < closestDistance)
            {
                closestChild = child;
                closestDistance = distance;
            }
        }

        // Return the closest child's transform
        return closestChild.GetComponent<CellController>();
    }

}
