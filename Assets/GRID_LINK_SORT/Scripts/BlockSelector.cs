using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoSingleton<BlockSelector>
{
    public event System.Action<List<ColoredBlock>> MovingTriggeredEvent;
    public event System.Action MouseButtonUpEvent;

    [Header("References")]
    [SerializeField] LayerMask coloredBlockLayer;
    [SerializeField] LayerMask defaultLayer;
    [SerializeField] LineRenderer lineRendererPrefab;
    [SerializeField] GameObject arrowPrefab;

    [Header("Config")]
    [SerializeField] float maxDragDistance;

    [Header("Debug")]
    [SerializeField] bool blockPickingFirstColored;
    [SerializeField] bool stopLinking;
    [SerializeField] ColoredBlock firstPickedBlock;
    [SerializeField] LineRenderer currentLineRenderer;
    [SerializeField] MoveDir moveDirection;
    [SerializeField] List<ColoredBlock> selectedBlocks = new List<ColoredBlock>();
    GameObject currentArrow;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ColoredBlock block;
            Vector3 _hitPos;
            if (HitColoredBlock(out block, out _hitPos))
            {
                if (blockPickingFirstColored) return;
                if (block.IsMoving) return;

                firstPickedBlock = block;
                selectedBlocks.Add(firstPickedBlock);
                blockPickingFirstColored = true;
                CreateLineRenderer();
                firstPickedBlock.GetSelected();
            }
        }

        if (firstPickedBlock == null) return;

        #region DRAGGING & LINKING

        if (!stopLinking)
        {
            if (Input.GetMouseButton(0))
            {
                ColoredBlock block;
                Vector3 _hitPos;
                if (HitColoredBlock(out block, out _hitPos))
                {
                    // if (block.IsMoving) return;
                    if (!CheckIfTheBlockAddable(block)) return;

                    Debug.Log("Linking");
                    ColoredBlock previous = selectedBlocks[selectedBlocks.Count - 1];

                    selectedBlocks.Add(block);
                    block.GetSelected();
                    UpdateLineRenderer(false);

                    if (selectedBlocks.Count != 1)
                        block.SetPreviousLinkedBlock(previous);

                }
                #region Trigger Moving
                else
                {
                    Vector3 hitPos;
                    if (HitDefaultLayer(out hitPos))
                    {
                        moveDirection = GetDir(hitPos, selectedBlocks[selectedBlocks.Count - 1].transform.position);
                        UpdateLineRenderer(true);
                    }
                }
                #endregion
            }
        }
        #endregion

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 hitPos;
            if (HitDefaultLayer(out hitPos))
            {
                ColoredBlock lastColoredBlock = selectedBlocks[selectedBlocks.Count - 1];
                float distanceFromLastBlock = Vector3.Distance(lastColoredBlock.transform.position, hitPos);

                if (distanceFromLastBlock > maxDragDistance)
                {
                    moveDirection = GetDir(hitPos, lastColoredBlock.transform.position);
                    lastColoredBlock.PerformMoving(moveDirection, selectedBlocks[0], lastColoredBlock);

                    stopLinking = true;
                    DestroyLineRenderer();
                    SetArrow(enable: false);
                    MovingTriggeredEvent?.Invoke(selectedBlocks);
                }
            }

            ResetParams();
            MouseButtonUpEvent?.Invoke();
        }
    }

    MoveDir GetDir(Vector3 mousePos, Vector3 lastBlockPos)
    {
        MoveDir moveDir = MoveDir.None;
        float xDistance = Mathf.Abs(Vector3.Distance(lastBlockPos, new Vector3(mousePos.x, lastBlockPos.y, lastBlockPos.z)));
        float zDistance = Mathf.Abs(Vector3.Distance(lastBlockPos, new Vector3(lastBlockPos.x, lastBlockPos.y, mousePos.z)));

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
        stopLinking = false;
        firstPickedBlock = null;
        DestroyLineRenderer();
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



    GridCell GetPossibleTargetCell()
    {
        GridCell possibletarget;
        GridCell lastPickedCell = selectedBlocks[selectedBlocks.Count - 1].GetOccupiedCell();

        int xOffset = 0;
        int zOffset = 0;

        switch (moveDirection)
        {
            case MoveDir.None:
                break;
            case MoveDir.Right:
                xOffset += 1;
                break;
            case MoveDir.Left:
                xOffset -= 1;
                break;
            case MoveDir.Up:
                zOffset += 1;
                break;
            case MoveDir.Down:
                zOffset -= 1;
                break;
        }


        Vector2Int newCellCoordinates = new Vector2Int(lastPickedCell.GetCoordinates().x + xOffset, lastPickedCell.GetCoordinates().y + zOffset);
        possibletarget = GridManager.instance.GetGridCellByCoordinates(newCellCoordinates);


        return possibletarget;
    }

    void SetArrow(bool enable, Transform cell = null)
    {
        if (currentArrow != null) Destroy(currentArrow);

        if (enable)
        {
            Quaternion rotation = Quaternion.identity;
            switch (moveDirection)
            {
                case MoveDir.None:
                    break;
                case MoveDir.Right:
                    rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case MoveDir.Left:
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case MoveDir.Up:
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    break;
                case MoveDir.Down:
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
            }

            GameObject cloneArrow = Instantiate(arrowPrefab, cell.transform);
            cloneArrow.transform.rotation = rotation;
            currentArrow = cloneArrow;

        }
        else
        {
            Destroy(currentArrow);
        }
    }

    #region LINE RENDERER

    private void CreateLineRenderer()
    {
        if (lineRendererPrefab != null)
        {
            Vector3 firstPos = new Vector3(firstPickedBlock.transform.position.x, firstPickedBlock.transform.position.y + 0.2f, firstPickedBlock.transform.position.z);
            currentLineRenderer = Instantiate(lineRendererPrefab, firstPos, Quaternion.identity);
            currentLineRenderer.positionCount = selectedBlocks.Count;


            for (int i = 0; i < selectedBlocks.Count; i++)
            {
                currentLineRenderer.SetPosition(i, firstPos);
            }
        }
    }

    private void UpdateLineRenderer(bool setArrow)
    {
        if (currentLineRenderer != null)
        {
            List<Vector3> coloredBlockPoses = new List<Vector3>();
            for (int i = 0; i < selectedBlocks.Count; i++)
            {
                Vector3 occupiedCellPos = selectedBlocks[i].GetOccupiedCell().transform.position;
                coloredBlockPoses.Add(new Vector3(occupiedCellPos.x, occupiedCellPos.y + .4f, occupiedCellPos.z));
            }

            Transform possibleTargetCell = null;
            if (GetPossibleTargetCell() != null)
            {
                possibleTargetCell = GetPossibleTargetCell().transform;
                coloredBlockPoses.Add(new Vector3(possibleTargetCell.position.x, possibleTargetCell.position.y + .4f, possibleTargetCell.position.z));
            }
            else
                setArrow = false;

            currentLineRenderer.positionCount = coloredBlockPoses.Count;

            for (int i = 0; i < coloredBlockPoses.Count; i++)
            {
                Vector3 pos = coloredBlockPoses[i];
                currentLineRenderer.SetPosition(i, pos);
            }

            if (setArrow)
                SetArrow(enable: true, possibleTargetCell);
        }
    }

    private void DestroyLineRenderer()
    {
        if (currentLineRenderer != null)
        {
            Destroy(currentLineRenderer.gameObject);
            currentLineRenderer = null;
        }

        selectedBlocks.Clear();
    }
    #endregion
    #region Raycast Region
    bool HitColoredBlock(out ColoredBlock hitBlock, out Vector3 _hitPos)
    {
        bool isHit = false;
        hitBlock = null;
        _hitPos = Vector3.zero;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 300, coloredBlockLayer))
        {
            if (hit.collider.TryGetComponent(out ColoredBlock block))
            {
                isHit = true;
                hitBlock = block;
                _hitPos = hit.point;
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
            hitPos = hit.point;
        }

        hitPos.y = .25f; // default colored block Y pos
        return isHit;
    }
    #endregion
}
