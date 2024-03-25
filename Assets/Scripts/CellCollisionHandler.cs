using UnityEngine;

public class CellCollisionHandler : MonoBehaviour
{
    public bool evenlyAligned;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out HoleColliderHandler holeCollider))
        {
            evenlyAligned = true;
            holeCollider.TriggerEvent(evenlyAligned);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out HoleColliderHandler holeCollider))
        {
            holeCollider.TriggerEvent(evenlyAligned);
            evenlyAligned = false;
        }
    }
}
