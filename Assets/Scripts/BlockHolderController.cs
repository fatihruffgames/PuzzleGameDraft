using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHolderController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Material defaultAnchorMaterial;
    [SerializeField] Material activeAnchorMaterial;
    [SerializeField] Transform pickablePointHolder;
    [SerializeField] List<Transform> anchorPoints;

    [Header("References")]
    [SerializeField] float distanceThreshold;

    [Header("Debug")]
    [SerializeField] HingeJoint joint;
    [SerializeField] Transform activeAnchor;
    [SerializeField] int index = 0;

    private void Start()
    {
        joint = GetComponent<HingeJoint>();
        SetAnchor();

        InputManager.instance.PickablePointPlacedEvent += OnPickablePointPlaced;

    }

    private void OnPickablePointPlaced(PickablePoint lastMovedPoint)
    {
        CellController targetCell;
        Transform currentAnchor = null;
        int count = 0;

        for (int p = 0; p < pickablePointHolder.childCount; p++)
        {
            PickablePoint pickablePoint = pickablePointHolder.GetChild(p).GetComponent<PickablePoint>();

            if (pickablePoint.occupiedCell != null)
            {
                targetCell = pickablePoint.occupiedCell;

                for (int i = 0; i < anchorPoints.Count; i++)
                {
                    AnchorPointsController anchor = anchorPoints[i].GetComponent<AnchorPointsController>();

                    if (anchor.closestCell == targetCell)
                    {
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
                Debug.LogError("Gigi: " + count);
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

    /// <summary>
    /// ///////////
    /// </summary>

    private void SetAnchor()
    {
        if (activeAnchor != null) activeAnchor.GetComponent<Renderer>().sharedMaterial = defaultAnchorMaterial;
        activeAnchor = anchorPoints[index];
        activeAnchor.GetComponent<Renderer>().sharedMaterial = activeAnchorMaterial;
        joint.connectedBody = activeAnchor.GetComponent<Rigidbody>();
        joint.anchor = activeAnchor.transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            index++;
            if (index == anchorPoints.Count) index = 0;
            SetAnchor();
        }
    }
}
