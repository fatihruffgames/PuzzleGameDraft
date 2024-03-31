using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ColoredBlock : MonoBehaviour
{
    [Header("Config")]
    public ColorEnum ColorEnum;

    [Header("References")]
    public List<Material> colorMats;

    [Header("Debug")]
    [SerializeField] GridCell currentOccupiedCell;
    [SerializeField] ColoredBlock previousLinkedBlock;
    [SerializeField] List<GridCell> path = new();
    Renderer _renderer;
    Transform mesh;

    private void Awake()
    {
        mesh = transform.GetChild(0);
        _renderer = mesh.GetComponent<Renderer>();

        switch (ColorEnum)
        {
            case ColorEnum.RED:
                _renderer.material = colorMats[0];
                break;
            case ColorEnum.GREEN:
                _renderer.material = colorMats[1];
                break;
            case ColorEnum.BLUE:
                _renderer.material = colorMats[2];
                break;
            case ColorEnum.ORANGE:
                _renderer.material = colorMats[3];
                break;
        }


    }

    private void Start()
    {
        currentOccupiedCell = GridManager.instance.GetClosestGridCell(from: transform.position);
        currentOccupiedCell.SetOccupied(state: true, this);
    }
    public void PerformMoving(MoveDir moveDir, ColoredBlock firstColoredElement)
    {
        List<Vector2Int> pathCells = new();

        Vector2Int newCoordinates = currentOccupiedCell.GetCoordinates();

        bool canMove = true;
        while (canMove)
        {
            if (pathCells.Count != 0)
                newCoordinates = pathCells[pathCells.Count - 1];

            // Update the new coordinates based on the primary movement direction
            switch (moveDir)
            {
                case MoveDir.None:
                    Debug.LogWarning("Move direction does not assign correctly");
                    break;
                case MoveDir.Right:
                    newCoordinates.x += 1;
                    break;
                case MoveDir.Left:
                    newCoordinates.x -= 1;
                    break;
                case MoveDir.Up:
                    newCoordinates.y += 1;
                    break;
                case MoveDir.Down:
                    newCoordinates.y -= 1;
                    break;
            }

            GridCell newCell = GridManager.instance.GetGridCellByCoordinates(newCoordinates);

            if (newCell != null && !newCell.IsOccupied)
            {
                pathCells.Add(newCoordinates);
            }
            else
            {
                // Stop moving if the new cell is occupied or null
                canMove = false;
            }
        }

        StartCoroutine(MovingRoutine(pathCells, firstColoredElement));
        BlockSelector.instance.ResetParams();
    }

    public void GoToCell(List<Vector2Int> path, ColoredBlock firstColoredElement)
    {
        for (int i = 0; i < path.Count; i++)
        {
            GridCell newCell = GridManager.instance.GetGridCellByCoordinates(path[i]);
            this.path.Add(newCell);
        }

        StartCoroutine(MovingRoutine(path, firstColoredElement));
    }

    IEnumerator MovingRoutine(List<Vector2Int> path, ColoredBlock firstColoredElement)
    {
        currentOccupiedCell.SetOccupied(false);
        GridCell previousCell = currentOccupiedCell;

        List<Vector2Int> pathForPrevious = new();
        pathForPrevious.Add(previousCell.GetCoordinates());
        pathForPrevious.AddRange(path);
        pathForPrevious.RemoveAt(pathForPrevious.Count - 1);
        bool routineEnded = false;
        yield return null;

        if (previousLinkedBlock != null)
            previousLinkedBlock.GoToCell(pathForPrevious, firstColoredElement);

        for (int i = 0; i < path.Count; i++)
        {
            yield return new WaitForSeconds(.2f);

            GridCell newCell = GridManager.instance.GetGridCellByCoordinates(path[i]);

            currentOccupiedCell.SetOccupied(false);
            currentOccupiedCell = newCell;
            currentOccupiedCell.SetOccupied(true, this);

            transform.DOMove(newCell.GetCenter(), .25f).OnComplete(() =>
            {
                if (i == path.Count - 1)
                {
                    ResetParams();

                    if (firstColoredElement == this)
                    {
                        routineEnded = true;
                    }

                }
            });
        }

        if (routineEnded)
        {
            yield return new WaitForSeconds(.2f);
            currentOccupiedCell.CheckNeighborsColor();
        }
    }

    public void DestroySelf()
    {
        ResetParams();
        Destroy(gameObject, .1f);
    }

    void ResetParams()
    {
        previousLinkedBlock = null;
        path.Clear();
    }
    
    #region GETTERS & SETTERS

    public void SetPreviousLinkedBlock(ColoredBlock linked)
    {
        previousLinkedBlock = linked;
    }

    public GridCell GetOccupiedCell()
    {
        return currentOccupiedCell;
    }

    #endregion
}
public enum ColorEnum
{
    RED, GREEN, BLUE, ORANGE
}
public enum MoveDir
{
    None, Right, Left, Up, Down
}