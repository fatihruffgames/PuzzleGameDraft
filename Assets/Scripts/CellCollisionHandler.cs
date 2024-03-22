using UnityEngine;

public class CellCollisionHandler : MonoBehaviour
{
    public bool evenlyAligned;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Hole")
        {
            evenlyAligned = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hole")
        {
            evenlyAligned = false;
        }
    }
}
