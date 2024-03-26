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


    private void Start()
    {
        InputManager.instance.PickablePointPlacedEvent += OnPickablePointPlaced;
        InputManager.instance.PickablePointSelectedEvent += OnSelected;
        InputManager.instance.PickablePointReleaseddEvent += OnReleased;
    }
    private void OnReleased(PickablePoint point)
    {
        if (point.transform != startPoint && point.transform != endPoint) return;

        canPerform = false;

        float distance = Vector3.Distance(startPoint.position, endPoint.position);

        Vector3 dir = endPoint.position - startPoint.position;
        Vector3 newPosition = (startPoint.position + endPoint.transform.position) / 2f;

        // Set the position of the capsule to the new calculated position
        transform.position = newPosition;
        transform.localScale = new Vector3(transform.localScale.x, distance * ratio, transform.localScale.z);

        // Set the rotation of the object
        transform.up = dir;

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        currentEulerAngles.x = 0f;
        currentEulerAngles.y = 0f;

        // Apply the modified rotation
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

        //// modify scale
        //IEnumerator ScaleRoutine()
        //{
        //    yield return null;
        //    float distance = Vector3.Distance(startPoint.position, endPoint.position);

        //    Vector3 dir = endPoint.position - startPoint.position;
        //    Vector3 newPosition = (startPoint.position + endPoint.position) / 2f;

        //    // Set the position of the capsule to the new calculated position
        //    transform.position = newPosition;
        //    transform.localScale = new Vector3(transform.localScale.x, distance * ratio, transform.localScale.z);

        //    // Set the rotation of the object
        //    transform.up = dir;

        //    Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        //    currentEulerAngles.x = 0f;
        //    currentEulerAngles.y = 0f;

        //    // Apply the modified rotation
        //    transform.rotation = Quaternion.Euler(currentEulerAngles);
        //}

        //StartCoroutine(ScaleRoutine());
    }



    void Update()
    {
        if (!canPerform) return;

        UpdateScaleAndRotation();
    }

    void UpdateScaleAndRotation()
    {
        Debug.Log("Released");
        float distance = Vector3.Distance(startPoint.transform.GetComponent<PickablePoint>().mesh.position, endPoint.transform.GetComponent<PickablePoint>().mesh.position);

        Vector3 dir = endPoint.transform.GetComponent<PickablePoint>().mesh.position - startPoint.transform.GetComponent<PickablePoint>().mesh.position;
        Vector3 newPosition = (startPoint.transform.GetComponent<PickablePoint>().mesh.position + endPoint.transform.GetComponent<PickablePoint>().mesh.position) / 2f;

        // Set the position of the capsule to the new calculated position
        transform.position = newPosition;
        transform.localScale = new Vector3(transform.localScale.x, distance * ratio, transform.localScale.z);

        // Set the rotation of the object
        transform.up = dir;

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        currentEulerAngles.x = 0f;
        currentEulerAngles.y = 0f;

        // Apply the modified rotation
        transform.rotation = Quaternion.Euler(currentEulerAngles);
    }



}
