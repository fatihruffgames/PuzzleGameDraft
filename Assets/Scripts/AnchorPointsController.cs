using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPointsController : MonoBehaviour
{

    public CellController closestCell;

    private void Start()
    {

        closestCell = BoardHolder.instance.GetClosestCell(transform.position);

        OnPickablePointPlaced(null);

        InputManager.instance.PickablePointPlacedEvent += OnPickablePointPlaced;
    }

    private void OnPickablePointPlaced(PickablePoint obj)
    {
        closestCell = BoardHolder.instance.GetClosestCell(transform.position);
    }

    
}
