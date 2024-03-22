using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    public event System.Action<PickablePoint> PickablePointPlacedEvent;

    [Header("References")]
    public LayerMask PickibleLayer;

    [Header("References")]
    [SerializeField] float zAxisOffset;

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

        // If dragging, move the selected object with the mouse
        if (isDragging && selectedPickable != null)
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 newPosition = new Vector3(hit.point.x, hit.point.y, zAxisOffset);
                selectedPickable.transform.position = newPosition;
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
}
