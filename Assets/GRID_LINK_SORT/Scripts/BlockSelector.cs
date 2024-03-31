using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoSingleton<BlockSelector>
{

    [Header("References")]
    [SerializeField] LayerMask coloredBlockLayer;
    [SerializeField] LayerMask defaultLayer;

    [Header("Config")]
    [SerializeField] float maxDragDistance;

    [Header("Debug")]
    [SerializeField] ColoredBlock firstPickedBlock;
    [SerializeField] bool blockPickingFirstColored;
    [SerializeField] bool stopDragging;
    [SerializeField] List<ColoredBlock> selectedBlocks = new List<ColoredBlock>();
    [SerializeField] MoveDir moveDirection;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ColoredBlock block;
            if (HitColoredBlock(out block))
            {
                if (blockPickingFirstColored) return;

                firstPickedBlock = block;
                selectedBlocks.Add(firstPickedBlock);
                blockPickingFirstColored = true;
            }
        }

        if (firstPickedBlock == null) return;

        #region DRAGGING & LINKING

        if (!stopDragging)
        {
            if (Input.GetMouseButton(0))
            {
                ColoredBlock block;
                if (HitColoredBlock(out block))
                {
                    if (!CheckIfTheBlockAddable(block)) return;

                    ColoredBlock previous = selectedBlocks[selectedBlocks.Count - 1];

                    selectedBlocks.Add(block);
                    if (selectedBlocks.Count != 1)
                        block.SetPreviousLinkedBlock(previous);

                }
                #region Trigger Moving
                else
                {
                    Vector3 hitPos;
                    if (HitDefaultLayer(out hitPos))
                    {
                        ColoredBlock lastColoredBlock = selectedBlocks[selectedBlocks.Count - 1];
                        float distanceFromLastBlock = Vector3.Distance(lastColoredBlock.transform.position, hitPos);

                        if (distanceFromLastBlock > maxDragDistance)
                        {
                            moveDirection = GetDir(hitPos, lastColoredBlock.transform.position);
                            lastColoredBlock.PerformMoving(moveDirection, selectedBlocks[0]);
                            stopDragging = true;
                        }
                    }
                }
                #endregion


            }
        }
        #endregion

        if (Input.GetMouseButtonUp(0))
        {
            ResetParams();
        }
    }

    MoveDir GetDir(Vector3 mousePos, Vector3 lastBlockPos)
    {
        MoveDir moveDir = MoveDir.None;
        float xDistance = Vector3.Distance(lastBlockPos, new Vector3(mousePos.x, lastBlockPos.y, lastBlockPos.z));
        float zDistance = Vector3.Distance(lastBlockPos, new Vector3(lastBlockPos.x, lastBlockPos.y, mousePos.z));

        if (xDistance > zDistance)
        {
            if (mousePos.x > lastBlockPos.x)
                moveDir = MoveDir.Right;
            else if (mousePos.x < lastBlockPos.x)
                moveDir = MoveDir.Left;
        }
        else
        {
            if (mousePos.z > lastBlockPos.z)
                moveDir = MoveDir.Up;
            else if (mousePos.z < lastBlockPos.z)
                moveDir = MoveDir.Down;
        }
        return moveDir;
    }

    public void ResetParams()
    {
        blockPickingFirstColored = false;
        stopDragging = false;
        firstPickedBlock = null;
        selectedBlocks.Clear();
        moveDirection = MoveDir.None;
    }

    bool CheckIfTheBlockAddable(ColoredBlock block)
    {
        bool addable = true;

        if (firstPickedBlock == block) addable = false;
        if (firstPickedBlock.ColorEnum != block.ColorEnum) addable = false;
        if (selectedBlocks.Contains(block)) addable = false;

        if (!selectedBlocks[selectedBlocks.Count - 1].GetOccupiedCell().GetNeighbors().Contains(block.GetOccupiedCell())) addable = false;

        return addable;
    }
   
    #region Raycast Region
    bool HitColoredBlock(out ColoredBlock hitBlock)
    {
        bool isHit = false;
        hitBlock = null;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 300, coloredBlockLayer))
        {
            if (hit.collider.TryGetComponent(out ColoredBlock block))
            {
                isHit = true;
                hitBlock = block;
            }
        }

        return isHit;
    }

    bool HitDefaultLayer(out Vector3 hitPos)
    {
        bool isHit = false;
        hitPos = Vector3.zero;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 300, defaultLayer))
        {
            isHit = true;
            hitPos = hit.collider.transform.position;
        }

        hitPos.y = .25f; // default colored block Y pos
        return isHit;
    }
    #endregion
}
