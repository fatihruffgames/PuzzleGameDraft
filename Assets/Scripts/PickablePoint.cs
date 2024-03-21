using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class PickablePoint : MonoBehaviour
{
    [Header("Configuration")]
    public bool IsPicked;
    public bool IsPlaced;
    [SerializeField] LayerMask cellLayer;

    [Header("Debug")]
    [SerializeField] CellController occupiedCell;
    Vector3 _startPos;

    private void Awake()
    {
        _startPos = transform.position;
        occupiedCell = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!IsPicked) return;
            IsPicked = false;

            CellController cell = GetCellFront();

            if (cell != null)
            {
                Debug.Log("GetPlaced(); invoked");
                GoToCell();
            }
            else
            {
                GetReleased();
            }
        }
    }

    void GoToCell()
    {
        CellController targetCell = GetCellFront();
        Vector3 cellPos = targetCell.transform.position;

        InputManager.instance.SetBlockPicking(shouldBlock: false);
        targetCell.SetOccupied(true);
        transform.position = cellPos;
        occupiedCell = targetCell;
    }

    #region GETTERS

    CellController GetCellFront()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + (Vector3.back), transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

        if (Physics.Raycast(ray, out hit, 100, cellLayer))
        {
            if (hit.collider.transform.TryGetComponent(out CellController cell))
            {
                if (cell.isOccupied) return null;
                return cell;
            }
        }

        return null;
    }
    public void GetPicked()
    {
        IsPicked = true;
        if (occupiedCell)
        {
            occupiedCell.SetOccupied(state: false);
            occupiedCell = null;
        }
    }

    public void GetReleased()
    {
        IsPicked = false;
        InputManager.instance.SetBlockPicking(false);
        transform.position = _startPos;
    }
    public void GetPlaced(Vector3 cellPosition)
    {
        if (!GetCellFront()) return;

        Debug.Log("GetPlaced(); invoked");
        //  cellPosition.z -= 0.2f;
        transform.position = cellPosition;
        IsPicked = false;
    }

    #endregion
}