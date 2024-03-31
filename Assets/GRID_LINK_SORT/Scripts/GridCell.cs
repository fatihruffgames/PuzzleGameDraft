using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] Vector2Int _coordinates;


    [Header("Debug")]
    public bool IsAction;
    public bool IsOccupied;
    public ColoredBlock upperColoredBlock;
    [SerializeField] List<GridCell> neighbours;

    private void Start()
    {
        name = _coordinates.ToString();
        neighbours = GetNeighbors();
    }

    public void CheckNeighborsColor(bool matchedAlredy = false)
    {
        if (upperColoredBlock == null) return;

        ColorEnum myBlockColor = upperColoredBlock.ColorEnum;

        for (int i = 0; i < neighbours.Count; i++)
        {
            GridCell neighbor = neighbours[i];
            ColoredBlock targetUpperBlock = neighbor.GetUpperColoredBlock();

            if (targetUpperBlock == null) continue;

            ColorEnum neighborColor = targetUpperBlock.ColorEnum;

            if (targetUpperBlock != null)
            {

                if (myBlockColor == neighborColor)
                {
                    if (upperColoredBlock)
                    {
                        OnColorMatched();
                    }
                    SetOccupied(false);
                    neighbor.CheckNeighborsColor(true);
                }
            }
        }

        if (matchedAlredy) // There is always one cell left for last checking, it should be count as a mathced too
            OnColorMatched();
    }

    public void OnColorMatched()
    {
        if (!upperColoredBlock) return;

        upperColoredBlock.DestroySelf();
        SetOccupied(false);
        GameManager.instance.IncreaseMatchCount();
    }

    #region GETTERS & SETTERS
    public void SetOccupied(bool state, ColoredBlock coloredBlock = null)
    {
        IsOccupied = state;

        if (state && coloredBlock != null)
            upperColoredBlock = coloredBlock;
        else
            upperColoredBlock = null;
    }

    public Vector2Int GetCoordinates()
    {
        return _coordinates;
    }
    public Vector3 GetCenter()
    {
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y + .25f, transform.position.z);
        return centerPos;
    }


    public List<GridCell> GetNeighbors()
    {
        List<GridCell> gridCells = GridManager.instance.GridPlan;
        List<GridCell> neighbors = new List<GridCell>();

        int[] dx = { 1, 0, -1, 0 };
        int[] dz = { 0, 1, 0, -1 };

        for (int i = 0; i < dx.Length; i++)
        {
            Vector2Int neighborCoordinates = _coordinates + new Vector2Int(dx[i], dz[i]);
            GridCell neighbor = gridCells.Find(cell => cell._coordinates == neighborCoordinates);

            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public ColoredBlock GetUpperColoredBlock()
    {
        return upperColoredBlock;
    }
    #endregion
}
