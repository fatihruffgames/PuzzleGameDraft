using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask coloredBlockLayer;

    [Header("Debug")]
    [SerializeField] ColoredBlock firstPickedBlock;
    [SerializeField] bool blockPickingFirstColored;
    [SerializeField] List<ColoredBlock> selectedBlocks = new List<ColoredBlock>();
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 300, coloredBlockLayer))
            {
                if (hit.collider.TryGetComponent(out ColoredBlock block))
                {
                    if (blockPickingFirstColored) return;


                    firstPickedBlock = block;
                    selectedBlocks.Add(firstPickedBlock);
                    blockPickingFirstColored = true;
                }
            }
        }

        if (firstPickedBlock == null) return;

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 300, coloredBlockLayer))
            {
                if (hit.collider.TryGetComponent(out ColoredBlock block))
                {
                    if (!CheckIfTheBlockAddable(block)) return;
                 
                    selectedBlocks.Add(block);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectedBlocks.Count > 0)
            {

                for (int i = 0; i < selectedBlocks.Count; i++)
                {
                    ColoredBlock block = selectedBlocks[i];
                    block.transform.position += Vector3.one;
                }
            }

            blockPickingFirstColored = false;
            firstPickedBlock = null;
            selectedBlocks.Clear();
        }
    }


    bool CheckIfTheBlockAddable(ColoredBlock block)
    {
        bool addable = true;

        if (firstPickedBlock == block) addable = false;
        if (firstPickedBlock.ColorEnum != block.ColorEnum) addable = false;
        if (selectedBlocks.Contains(block)) addable = false;

        // If the last selected colored block's cell is a neighbour for the new selected
        if (!selectedBlocks[selectedBlocks.Count - 1].GetOccupiedCell().GetNeighbors().Contains(block.GetOccupiedCell())) addable = false;

        return addable;
    }

}
