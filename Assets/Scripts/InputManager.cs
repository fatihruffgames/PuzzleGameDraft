using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    public event System.Action<PickablePoint> PickablePointPlacedEvent;


    public event System.Action<PickablePoint> PickablePointSelectedEvent;
    public event System.Action<PickablePoint> PickablePointReleaseddEvent;


    [Header("References")]
    public LayerMask PickibleLayer;

    [Header("Config")]
    [SerializeField] float zDefaultPos;
    [SerializeField] float yAxisOffset;

    [Header("Debug")]
    [SerializeField] GameObject selectedObject;
    [SerializeField] PickablePoint selectedPickablePoint;
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

                    selectedObject = pickable.gameObject;
                    pickable.GetPicked();

                    blockPicking = true;
                    isDragging = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedObject != null)
            {
                selectedObject = null;
                isDragging = false;
            }
        }

        #region Old Implementation
        /* if (isDragging && selectedObject != null)
         {
             if (selectedPickablePoint != null)
             {
                 RaycastHit hit;
                 Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                 if (Physics.Raycast(ray, out hit))
                 {
                     Vector3 newPosition = new Vector3(hit.point.x, hit.point.y + yAxisOffset, zAxisOffset);
                     Vector3 direction = newPosition - selectedPickablePoint.siblingPoint.position;

                     if (direction.magnitude > selectedPickablePoint.maxDistance)
                     {
                         direction = direction.normalized * selectedPickablePoint.maxDistance;
                     }
                     selectedPickablePoint.mesh.position = selectedPickablePoint.siblingPoint.position + direction;
                 }
             }
         }*/
        #endregion


        if (isDragging && selectedObject != null && selectedPickablePoint != null)
        {
            // Convert mouse position to world position
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDefaultPos));

            // Calculate the new position based on the mouse position
            Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y + yAxisOffset, zDefaultPos);

            // Calculate the direction from the sibling point to the new position
            Vector3 direction = newPosition - selectedPickablePoint.siblingPoint.position;

            // If the direction magnitude exceeds the max distance, limit it
            if (direction.magnitude > selectedPickablePoint.maxDistance)   
            {
                direction = direction.normalized * selectedPickablePoint.maxDistance;
            }

            // Set the new position of the mesh
            selectedPickablePoint.mesh.position = selectedPickablePoint.siblingPoint.position + direction;
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

    public void SetSelectedPickablePoint(PickablePoint point)
    {
        selectedPickablePoint = point;
    }
}

