using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    public event System.Action<PickablePoint> PickablePointPlacedEvent;


    public event System.Action<PickablePoint> PickablePointSelectedEvent;
    public event System.Action<PickablePoint> PickablePointReleaseddEvent;


    [Header("References")]
    public LayerMask PickibleLayer;

    [Header("References")]
    [SerializeField] float zAxisOffset;
    [SerializeField] float yAxisOffset;

    [Header("Debug")]
    [SerializeField] GameObject selectedPickable;
    [SerializeField] bool blockPicking;
    [SerializeField] bool isDragging = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 300, PickibleLayer))
            {
                if (hit.collider.TryGetComponent(out PickablePoint pickable))
                {
                    if (blockPicking) return;
                    if (pickable.IsPicked) return;

                    selectedPickable = pickable.gameObject;
                    pickable.GetPicked();

                    blockPicking = true;
                    isDragging = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedPickable != null)
            {
                selectedPickable = null;
                isDragging = false;
            }
        }


        if (isDragging && selectedPickable != null)
        {

            PickablePoint pickablePoint = selectedPickable.transform.GetComponent<PickablePoint>();
            bool canMove = true;
            RaycastHit hitt;
            Ray rayy = mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 mousePosition = Vector3.zero;

            if (Physics.Raycast(rayy, out hitt))
            {
                mousePosition = new Vector3(hitt.point.x, hitt.point.y , hitt.point.z);
            }

            float distance = Vector3.Distance(mousePosition, pickablePoint.siblingPoint.position);

            if (distance > pickablePoint.maxDistance)
            {
                canMove = false;
            }


            if (canMove)
            {
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 newPosition = new Vector3(hit.point.x, hit.point.y + yAxisOffset, zAxisOffset);
                    pickablePoint.mesh.position = newPosition;
                }
            }
        }
    }



    public void SetBlockPicking(bool shouldBlock)
    {
        blockPicking = shouldBlock;
    }

    public void TriggerPickablePlacedEvent(PickablePoint point)
    {
        PickablePointPlacedEvent?.Invoke(point);
    }

    public void TriggerPickableSelected(PickablePoint point)
    {
        PickablePointSelectedEvent?.Invoke(point);
    }

    public void TriggerPickableReleased(PickablePoint point)
    {
        PickablePointReleaseddEvent?.Invoke(point);
    }
}

