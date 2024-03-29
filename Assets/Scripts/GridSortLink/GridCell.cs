using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GridCell : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] Vector2Int _coordinates;


    [Header("Debug")]
    public bool IsAction;
    public bool isOccupied;
    public ColoredBlock upperColoredBlock;
    [SerializeField] List<GridCell> neighbours;

    private void Start()
    {
        name = _coordinates.ToString();
        neighbours = GetNeighbors();
    }


    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }
    public Vector3 GetCenter()
    {
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y + .2f, transform.position.z);
        return centerPos;
    }

    public void SetOccupied(bool state, ColoredBlock coloredBlock = null)
    {
        isOccupied = state;
        if (coloredBlock != null)
            upperColoredBlock = coloredBlock;
    }

    public List<GridCell> GetNeighbors()
    {
        List<GridCell> gridCells = GridManager.instance.GridPlan;
        List<GridCell> neighbors = new List<GridCell>();

        // Define the directions for 4-way connectivity (right, up, left, down)
        int[] dx = { 1, 0, -1, 0 };
        int[] dz = { 0, 1, 0, -1 };

        // Loop through each direction
        for (int i = 0; i < dx.Length; i++)
        {
            // Calculate the neighbor's coordinates
            Vector2Int neighborCoordinates = _coordinates + new Vector2Int(dx[i], dz[i]);

            // Find the neighbor in the list of grid cells
            GridCell neighbor = gridCells.Find(cell => cell._coordinates == neighborCoordinates);

            // If the neighbor exists, add it to the list
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

}
