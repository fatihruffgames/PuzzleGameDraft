using UnityEngine;
using UnityEngine.UIElements;

public enum DistanceType
{
    Short,
    Long
}

public class PickablePoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform siblingPoint;

    [Header("Configuration")]
    [SerializeField] DistanceType distanceType;
    [SerializeField] LayerMask cellLayer;

    [Header("Debug")]
    public bool IsPicked;
    public bool IsPlaced;
    [SerializeField] float maxDistance;
    public CellController occupiedCell;
    Vector3 _currrentPos;


    private void Awake()
    {
        _currrentPos = transform.position;
        occupiedCell = null;

        maxDistance = distanceType == DistanceType.Short ? 0.87f : 1.55f;
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
                GoToCell();
            }
            else
            {
                GetReleased();
            }
        }

        if (!IsPicked) return;

        GetCellFront();
    }

    void GoToCell()
    {
        CellController targetCell = GetCellFront();
        Vector3 cellPos = targetCell.transform.position;
        float dist = Vector3.Distance(siblingPoint.position, transform.position);

        if (maxDistance < dist)
        {
            Debug.LogWarning("Distance is greater than max distance: " + dist);
            GetReleased();
            return;
        }

        cellPos.z -= .1f;
        InputManager.instance.SetBlockPicking(shouldBlock: false);
        targetCell.SetOccupied(true);
        transform.position = cellPos;
        _currrentPos = transform.position;
        occupiedCell = targetCell;
        IsPicked = false;

        // Trigger related placed event
        InputManager.instance.TriggerPickablePlacedEvent(this);
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
                else
                {
                    if (cell.isBlocked) return null;
                }
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
        transform.position = _currrentPos;
    }
    public void GetPlaced(Vector3 cellPosition)
    {
        if (!GetCellFront()) return;


        cellPosition.z -= -0.125f;
        transform.position = cellPosition;
        IsPicked = false;
    }

    #endregion
}
