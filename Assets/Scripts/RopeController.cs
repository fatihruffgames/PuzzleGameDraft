using System.Collections;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Header("Config")]
    public float ratio;

    private void Start()
    {
        InputManager.instance.PickablePointPlacedEvent += OnPickablePointPlaced;
    }

    private void OnPickablePointPlaced(PickablePoint point)
    {
        if (point.transform != startPoint && point.transform != endPoint) return;

        // modify scale
        IEnumerator ScaleRoutine()
        {
            yield return null;
            float distance = Vector3.Distance(startPoint.position, endPoint.position);

            Vector3 dir = endPoint.position - startPoint.position;
            Vector3 newPosition = (startPoint.position + endPoint.position) / 2f;

            // Set the position of the capsule to the new calculated position
            transform.position = newPosition;
            transform.localScale = new Vector3(transform.localScale.x, distance * ratio, transform.localScale.z);

            transform.up = dir;
        }

        StartCoroutine(ScaleRoutine());
    }
}
