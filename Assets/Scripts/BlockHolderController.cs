using System.Collections;
using UnityEngine;

public class BlockHolderController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Material defaultAnchorMaterial;
    [SerializeField] Material activeAnchorMaterial;
    [SerializeField] Transform pickablePointHolder;

    [Header("References")]
    [SerializeField] float distanceThreshold;

    [Header("Debug")]
    [SerializeField] Transform activeAnchor;
    [SerializeField] AnchorPointsController[] anchorPoints;
    HingeJoint joint;

    IEnumerator Start()
    {
        anchorPoints = GetComponentsInChildren<AnchorPointsController>();

        joint = GetComponent<HingeJoint>();

        yield return null;

        OnPickablePointPlaced(null);
        InputManager.instance.PickablePointPlacedEvent += OnPickablePointPlaced;
    }

    private void OnPickablePointPlaced(PickablePoint _)
    {
        CellController targetCell;
        Transform currentAnchor = null;
        int count = 0;

        for (int p = 0; p < pickablePointHolder.childCount; p++)
        {
            Debug.LogWarning("1");
            PickablePoint pickablePoint = pickablePointHolder.GetChild(p).GetComponent<PickablePoint>();

            if (pickablePoint.occupiedCell != null)
            {
                Debug.LogWarning("2");
                targetCell = pickablePoint.occupiedCell;

                for (int i = 0; i < anchorPoints.Length; i++)
                {
                    Debug.LogWarning("3");
                    AnchorPointsController anchor = anchorPoints[i].GetComponent<AnchorPointsController>();

                    if (anchor.closestCell == targetCell)
                    {
                        Debug.LogWarning("4");
                        count++;
                        currentAnchor = anchor.transform;
                    }
                }
            }
        }

        if (count > 0)
        {
            if (count < 2) // multiple anchor point yok ise
            {
                SetJointActivation(useLimits: false);
                SetAnchor(currentAnchor);
            }
            else
            {
                SetJointActivation(useLimits: true);
                Debug.LogWarning("Has multiple anchor points");
            }
        }
    }
    private void SetAnchor(Transform newAnchor)
    {
        Debug.LogWarning("Previous anchor: " + activeAnchor + ", New anchor: " + newAnchor);
        if (activeAnchor != null) activeAnchor.GetComponent<Renderer>().sharedMaterial = defaultAnchorMaterial;

        activeAnchor = newAnchor;
        activeAnchor.GetComponent<Renderer>().sharedMaterial = activeAnchorMaterial;
        joint.connectedBody = activeAnchor.GetComponent<Rigidbody>();
        joint.anchor = activeAnchor.transform.localPosition;
    }
    void SetJointActivation(bool useLimits)
    {
        joint.useLimits = useLimits;
    }
}
