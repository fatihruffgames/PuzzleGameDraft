using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCountManager : MonoSingleton<MatchCountManager>
{
    public List<GridCell> MatchedCells = new List<GridCell>();

    public void CheckMatchingCells(GridCell initialCell)
    {
        if (initialCell == null) return;

        ColorEnum startColor = initialCell.GetUpperColoredBlock().ColorEnum;
        AddCell(initialCell);
        CheckNeighborCells(initialCell, startColor);

        StartCoroutine(DelayedCheck());
    }

    IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(.075f);

        if (MatchedCells.Count >= 6)
        {
            for (int i = 0; i < MatchedCells.Count; i++)
            {
                GridCell cell = MatchedCells[i];
                cell.OnColorMatched();
            }
        }


        MatchedCells.Clear();
    }

    private void CheckNeighborCells(GridCell cell, ColorEnum startColor)
    {
        List<GridCell> neighbors = cell.GetNeighbors();

        for (int i = 0; i < neighbors.Count; i++)
        {
            GridCell neighbor = neighbors[i];
            if (neighbor.IsOccupied && neighbor.GetUpperColoredBlock().ColorEnum == startColor)
            {
                if (!MatchedCells.Contains(neighbor))
                {
                    AddCell(neighbor);
                    CheckNeighborCells(neighbor, startColor);
                }
            }
        }
    }

    void AddCell(GridCell gridCell)
    {
        if (!MatchedCells.Contains(gridCell))
        {
            MatchedCells.Add(gridCell);
        }
    }
}


