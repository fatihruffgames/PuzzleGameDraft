using System.Collections;
using Unity.VisualScripting;
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

    private void Awake()
    {
        joint = GetComponent<HingeJoint>();
    }

    IEnumerator Start()
    {
        anchorPoints = GetComponentsInChildren<AnchorPointsController>();


        yield return null;

        OnPickablePointPlaced(null);
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

                for (int i = 0; i < anchorPoints.Length; i++)
                {
                    AnchorPointsController anchor = anchorPoints[i].GetComponent<AnchorPointsController>();

                    if (anchor.isActive && anchor.closestCell == targetCell)
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
                #region Last Moved Point Assign Region
                // Start çağırıldığında lasMovedPoint null olduğu için manuel şekilde bulmam gerekli, 
                // Bu  kısma girerse sadece bir tane anchor point i var demektir.

                AnchorPointsController activeAnchor = null;
                if (lastMovedPoint == null)
                {
                    for (int i = 0; i < anchorPoints.Length; i++)
                    {
                        Transform anchor = anchorPoints[i].transform;
                        CellController closestOccupiedCell = BoardHolder.instance.GetClosestCell(anchor.position);

                        if (closestOccupiedCell.isOccupied)
                        {
                            activeAnchor = anchor.GetComponent<AnchorPointsController>();
                            lastMovedPoint = GetClosestPickablePoint(closestOccupiedCell.transform.position);
                        }


                        //float closestDistance = Mathf.Infinity;
                        //float currentDistance = Vector3.Distance(GetClosestPickablePoint(anchor.position).transform.position, anchor.position);

                        //if (closestDistance > currentDistance)
                        //{
                        //    closestDistance = currentDistance;
                        //    activeAnchor = anchor.GetComponent<AnchorPointsController>();
                        //    lastMovedPoint = GetClosestPickablePoint(anchor.transform.position);
                        //}
                    }
                }

                #endregion
                targetCell = lastMovedPoint.occupiedCell;
                if (activeAnchor == null)
                {
                    for (int i = 0; i < anchorPoints.Length; i++)
                    {
                        AnchorPointsController anchor = anchorPoints[i].GetComponent<AnchorPointsController>();

                        if (anchor.isActive && anchor.closestCell == targetCell)
                        {
                            currentAnchor = anchor.transform;
                        }
                    }
                }
                else
                    currentAnchor = activeAnchor.transform;


                SetJointActivationAndAnchor(useLimits: false, currentAnchor);
            }
            else
            {
                SetJointActivationAndAnchor(useLimits: true);
                Debug.LogWarning("Has multiple anchor points found");
            }
        }
        else
        {
            Debug.LogWarning("No anchor points");
            Destroy(joint);
        }
    }
    private void SetAnchor(Transform newAnchor)
    {
        if (activeAnchor != null) activeAnchor.GetComponent<Renderer>().sharedMaterial = defaultAnchorMaterial;

        activeAnchor = newAnchor;
        activeAnchor.GetComponent<Renderer>().sharedMaterial = activeAnchorMaterial;
        joint.connectedBody = activeAnchor.GetComponent<Rigidbody>();
        joint.anchor = activeAnchor.transform.localPosition;
    }
    void SetJointActivationAndAnchor(bool useLimits, Transform anchor = null)
    {
        if (joint == null)
        {
            transform.AddComponent<HingeJoint>();
            joint = GetComponent<HingeJoint>();
            Debug.LogError("HINGE JOINT IS NULL, BUT STILL TRYING TO ACCESS IT, ADDED NEW ONE");
        }

        if (anchor != null)
            SetAnchor(anchor);

        joint.useLimits = useLimits;

    }
    public PickablePoint GetClosestPickablePoint(Vector3 from)
    {
        Transform closestChild = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < pickablePointHolder.childCount; i++)
        {
            Transform child = pickablePointHolder.GetChild(i);
            float distance = Vector3.Distance(from, child.position);

            if (distance < closestDistance)
            {
                closestChild = child;
                closestDistance = distance;
            }
        }

        // Return the closest child's transform
        return closestChild.GetComponent<PickablePoint>();
    }
}
