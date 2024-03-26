using System.Collections;
using System.Drawing;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Header("Config")]
    public float ratio;
    [SerializeField] bool canPerform;
    Transform startMesh;
    Transform endMesh;

    private void Start()
    {
        InputManager.instance.PickablePointPlacedEvent += OnPickablePointPlaced;
        InputManager.instance.PickablePointSelectedEvent += OnSelected;
        InputManager.instance.PickablePointReleaseddEvent += OnReleased;

        startMesh = startPoint.transform.GetComponent<PickablePoint>().mesh;
        endMesh = endPoint.transform.GetComponent<PickablePoint>().mesh;
    }
    private void OnReleased(PickablePoint point)
    {
        if (point.transform != startPoint && point.transform != endPoint) return;

        canPerform = false;

        float distance = Vector3.Distance(startPoint.position, endPoint.position);

        Vector3 dir = endPoint.position - startPoint.position;
        Vector3 newPosition = (startPoint.position + endPoint.transform.position) / 2f;

        transform.position = newPosition;
        transform.localScale = new Vector3(transform.localScale.x, distance * ratio, transform.localScale.z);

        transform.up = dir;

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        currentEulerAngles.x = 0f;
        currentEulerAngles.y = 0f;

        transform.rotation = Quaternion.Euler(currentEulerAngles);
    }

    private void OnSelected(PickablePoint point)
    {
        if (point.transform != startPoint && point.transform != endPoint) return;

        canPerform = true;
    }
    private void OnPickablePointPlaced(PickablePoint point)
    {
        if (point.transform != startPoint && point.transform != endPoint) return;

        UpdateScaleAndRotation();
    }

    void Update()
    {
        if (!canPerform) return;

        UpdateScaleAndRotation();
    }

    void UpdateScaleAndRotation()
    {
        float distance = Vector3.Distance(startMesh.position, endMesh.position);

        Vector3 dir = endMesh.position - startMesh.position;
        Vector3 newPosition = (startMesh.position + endMesh.position) / 2f;

        transform.position = newPosition;
        transform.localScale = new Vector3(transform.localScale.x, distance * ratio, transform.localScale.z);

        transform.up = dir;

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        currentEulerAngles.x = 0f;
        currentEulerAngles.y = 0f;

        transform.rotation = Quaternion.Euler(currentEulerAngles);
    }
}
