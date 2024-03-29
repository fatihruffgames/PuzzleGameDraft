using System.Collections;
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

        // Initialize variables to keep track of the closest cell and its distance
        GridCell closestCell = null;
        float closestDistance = Mathf.Infinity;

        // Loop through each GridCell in the list
        foreach (GridCell cell in GridPlan)
        {
            // Calculate the distance between the current cell and the given position
            float distance = Vector3.Distance(cell.transform.position, from);

            // Check if this cell is closer than the previous closest cell
            if (distance < closestDistance)
            {
                closestCell = cell;
                closestDistance = distance;
            }
        }

        return closestCell;
    }
}
