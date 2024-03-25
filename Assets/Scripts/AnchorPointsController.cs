using System.Collections;
using UnityEngine;

public class AnchorPointsController : MonoBehaviour
{

    public CellController closestCell;
    public bool isActive;

    void Start()
    {
        AssignClosestCell();
        InputManager.instance.PickablePointPlacedEvent += OnPickablePointPlaced;
    }

    private void OnPickablePointPlaced(PickablePoint obj)
    {
        AssignClosestCell();
    }

    private void AssignClosestCell()
    {
        closestCell = BoardHolder.instance.GetClosestCell(transform.position);
        float distance = Vector3.Distance(transform.position, closestCell.transform.position);

        if (distance > 0.2f)
        {
            Debug.LogWarning("Closest cell is not near enough");
            isActive = false;
        }
        else
            isActive = true;
    }
}
