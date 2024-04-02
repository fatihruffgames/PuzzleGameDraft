using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredBlock : MonoBehaviour
{
    [Header("Config")]
    public ColorEnum ColorEnum;
    public bool IsMoving;

    [Header("References")]
    [SerializeField] GameObject dotCanvas;
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
        //  BlockSelector.instance.MovingTriggeredEvent += OnMovingStarted;
        BlockSelector.instance.MouseButtonUpEvent += DisableDot;
    }

    public void PerformMoving(MoveDir moveDir, ColoredBlock firstColoredElement, ColoredBlock lastColoredElement)
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

        StartCoroutine(MovingRoutine(pathCells, firstColoredElement, lastColoredElement));
    }

    public void GoToCell(List<Vector2Int> path, ColoredBlock firstColoredElement, ColoredBlock lastColoredElement)
    {
        for (int i = 0; i < path.Count; i++)
        {
            GridCell newCell = GridManager.instance.GetGridCellByCoordinates(path[i]);
            this.path.Add(newCell);
        }

        StartCoroutine(MovingRoutine(path, firstColoredElement, lastColoredElement));
    }

    IEnumerator MovingRoutine(List<Vector2Int> path, ColoredBlock firstColoredElement, ColoredBlock lastColoredElement)
    {
        // IsMoving = true;
        currentOccupiedCell.SetOccupied(false);
        GridCell previousCell = currentOccupiedCell;

        List<Vector2Int> pathForPrevious = new();
        pathForPrevious.Add(previousCell.GetCoordinates());
        pathForPrevious.AddRange(path);
        pathForPrevious.RemoveAt(pathForPrevious.Count - 1);
        bool routineEnded = false;
        yield return null;

        if (previousLinkedBlock != null)
            previousLinkedBlock.GoToCell(pathForPrevious, firstColoredElement, lastColoredElement);

        for (int i = 0; i < path.Count; i++)
        {
            yield return new WaitForSeconds(.2f);

            GridCell newCell = GridManager.instance.GetGridCellByCoordinates(path[i]);

            currentOccupiedCell.SetOccupied(false);
            currentOccupiedCell = newCell;
            currentOccupiedCell.SetOccupied(true, this);

            transform.DOMove(newCell.GetCenter(), .25f).OnComplete(() =>
            {
                if (newCell.GetCoordinates() == path[path.Count - 1])
                {
                    if (firstColoredElement == this) routineEnded = true;

                    ResetParams();
                }
            });
        }

        if (firstColoredElement == this)
        {
            yield return new WaitUntil(() => routineEnded == true);
            BlockSelector.instance.ResetParams();
            MatchCountManager.instance.CheckMatchingCells(lastColoredElement.GetOccupiedCell());
        }
    }

    public void DestroySelf()
    {
        //    BlockSelector.instance.MovingTriggeredEvent -= OnMovingStarted;
        BlockSelector.instance.MouseButtonUpEvent -= DisableDot;
        ResetParams();
        Destroy(gameObject, .1f);
    }

    void ResetParams()
    {
        // IsMoving = false;
        dotCanvas.SetActive(false);
        previousLinkedBlock = null;
        path.Clear();
    }
    
    public void DisableDot()
    {
        dotCanvas.SetActive(false);
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

    public void GetSelected()
    {
        dotCanvas.SetActive(true);
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