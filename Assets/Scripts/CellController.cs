using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("Debug")]
    public bool isOccupied;

    public Vector3 GetCenter()
    {
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - .2f);
        return centerPos;
    }

    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }
}
