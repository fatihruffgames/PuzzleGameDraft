using UnityEngine;

public enum DistanceType
{
    Short,
    Long
}

public class PickablePoint : MonoBehaviour
{
    [Header("References")]
    public Transform siblingPoint;
    public Transform mesh;
    public Renderer outline;

    [Header("Configuration")]
    [SerializeField] DistanceType distanceType;
    [SerializeField] LayerMask cellLayer;

    [Header("Debug")]
    public bool IsPicked;
    public bool IsPlaced;
    public float maxDistance;
    public CellController occupiedCell;
    Vector3 _currrentPos;
    float initDistance;

    private void Awake()
    {
        _currrentPos = transform.position;
        occupiedCell = null;

        maxDistance = distanceType == DistanceType.Short ? 0.87f : 1.25f;
    }
    private void Start()
    {
        InitOccupation();

    }
    private void InitOccupation()
    {
        CellController closestCell = BoardHolder.instance.GetClosestCell(transform.position);

        closestCell.SetOccupied(true);
        occupiedCell = closestCell;
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

        float distance = Vector3.Distance(mesh.transform.position, siblingPoint.position);
        float normalizedDistance = (distance - initDistance) / maxDistance;
        outline.material.SetFloat("_Width", normalizedDistance);
    }

    void GoToCell()
    {
        CellController targetCell = GetCellFront();
        Vector3 cellPos = targetCell.transform.position;
        float dist = Vector3.Distance(siblingPoint.position, mesh.position);

        if (maxDistance < dist)
        {
            Debug.LogWarning("Distance is greater than max distance: " + dist);
            GetReleased();
            return;
        }

        if (occupiedCell)
        {
            occupiedCell.SetOccupied(state: false);
            occupiedCell = null;
        }


        cellPos.z = transform.position.z;
        InputManager.instance.SetBlockPicking(shouldBlock: false);
        targetCell.SetOccupied(true);
        transform.position = cellPos;
        _currrentPos = transform.position;
        occupiedCell = targetCell;
        IsPicked = false;
        mesh.transform.localPosition = Vector3.zero;
        SetMeshes(true);

        // Trigger related placed event
        InputManager.instance.TriggerPickablePlacedEvent(this);
    }

    #region GETTERS

    CellController GetCellFront()
    {
        RaycastHit hit;
        Ray ray = new Ray(mesh.position + (Vector3.back), mesh.forward);

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

    void SetMeshes(bool flag)
    {
        transform.GetComponent<MeshRenderer>().enabled = flag;
        mesh.gameObject.SetActive(!flag);
    }

    public void GetPicked()
    {
        IsPicked = true;
        SetMeshes(false);

        initDistance = Vector3.Distance(mesh.transform.position, siblingPoint.position);
        InputManager.instance.TriggerPickableSelected(this);
    }

    public void GetReleased()
    {
        IsPicked = false;
        InputManager.instance.SetBlockPicking(false);
        transform.position = _currrentPos;
        mesh.transform.localPosition = Vector3.zero;
        SetMeshes(true);
        InputManager.instance.TriggerPickableReleased(this);

    }
    #endregion
}
