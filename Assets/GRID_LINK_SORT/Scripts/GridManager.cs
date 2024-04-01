using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    public List<GridCell> GridPlan;


    public GridCell GetClosestGridCell(Vector3 from)
    {
        if (GridPlan == null || GridPlan.Count == 0)
        {
            Debug.LogWarning("GridPlan list is empty or null!");
            return null;
        }

        GridCell closestCell = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < GridPlan.Count; i++)
        {
            GridCell cell = GridPlan[i];
            float distance = Vector3.Distance(cell.transform.position, from);

            if (distance < closestDistance)
            {
                closestCell = cell;
                closestDistance = distance;
            }
        }

        return closestCell;
    }

    public GridCell GetGridCellByCoordinates(Vector2Int coordinates)
    {
        if (GridPlan == null || GridPlan.Count == 0)
        {
            Debug.LogWarning("GridPlan list is empty or null!");
            return null;
        }

        for (int i = 0; i < GridPlan.Count; i++)
        {
            GridCell cell = GridPlan[i];
            if (cell.GetCoordinates() == coordinates)
            {
                return cell;
            }
        }

        Debug.LogWarning("No GridCell found with coordinates: " + coordinates);
        return null;
    }

}
