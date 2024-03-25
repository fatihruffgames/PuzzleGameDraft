using UnityEngine;

public class SingleBlock : MonoBehaviour
{
    HoleColliderHandler holeCollider;
    string nonInteractableBlock = "NonInteractableBlock"; // The name of the layer you want to change to

    void Start()
    {
        holeCollider = GetComponentInChildren<HoleColliderHandler>();
        holeCollider.AlignmentChangedEvent += OnAlignmentChanged;

    }

    private void OnAlignmentChanged(bool evenlyAligned)
    {
        CellController closestCell = BoardHolder.instance.GetClosestCell(transform.position);
        if (evenlyAligned && closestCell.isOccupied)
        {
            int layerIndex = LayerMask.NameToLayer(nonInteractableBlock);

            // Check if the layer index is valid (-1 indicates the layer name was not found)
            if (layerIndex != -1)
            {
                // Change the object's layer to the specified layer
                gameObject.layer = layerIndex;
                holeCollider.gameObject.layer = layerIndex;
            }
            else
            {
                Debug.LogError("Layer '" + nonInteractableBlock + "' not found!");
            }
        }

        else
        {
            // Default layer

            int layerIndex = LayerMask.NameToLayer("DefaultBlock");
            gameObject.layer = layerIndex;
            holeCollider.gameObject.layer = layerIndex;
        }

        GetComponent<Collider>().enabled = true;
    }
}
