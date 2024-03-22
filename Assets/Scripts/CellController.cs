using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("Debug")]
    public bool isOccupied;
    public bool isBlocked;

    [Header("References")]
    [SerializeField] CellCollisionHandler collisionHandler;

    public Vector3 GetCenter()
    {
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - .2f);
        return centerPos;
    }

    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }

    private void OnTriggerStay(Collider other)
    {
        if (collisionHandler.evenlyAligned) 
        {
            isBlocked = false;
            return;
        }

        if(other.transform.TryGetComponent(out SingleBlock block))
        {
            isBlocked = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent(out SingleBlock block))
        {
            isBlocked = false;
        }

    }
}
