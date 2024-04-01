using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoSingleton<BlockSelector>
{
    public event System.Action MovingTriggeredEvent;
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
                if (HitColoredBlock(out block))
                {
                    Debug.Log("Linking");
                    if (!CheckIfTheBlockAddable(block)) return;

                    ColoredBlock previous = selectedBlocks[selectedBlocks.Count - 1];

                    selectedBlocks.Add(block);
                    block.GetSelected();
                    UpdateLineRenderer();
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
                            lastColoredBlock.PerformMoving(moveDirection, selectedBlocks[0], lastColoredBlock);
                            stopLinking = true;
                            DestroyLineRenderer();
                            SetArrow(enable: true, lastColoredBlock.GetOccupiedCell());
                            MovingTriggeredEvent?.Invoke();
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
        SetArrow(enable: false);
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

    GameObject currentArrow;
    void SetArrow(bool enable, GridCell cell = null)
    {
        if (currentArrow != null) Destroy(currentArrow);

        if (enable)
        {
            GameObject cloneArrow = Instantiate(arrowPrefab, cell.transform);
            float xOffset = 0;
            float zOffset = 0;
            switch (moveDirection)
            {
                case MoveDir.None:
                    break;
                case MoveDir.Right:
                    xOffset += 1.1f;
                    cloneArrow.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case MoveDir.Left:
                    xOffset -= 1.1f;
                    cloneArrow.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case MoveDir.Up:
                    zOffset += 1.1f;
                    cloneArrow.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                    break;
                case MoveDir.Down:
                    zOffset -= 1.1f;
                    cloneArrow.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
            }

            Vector3 pos = new Vector3(cell.GetCenter().x, .45f, cell.GetCenter().y);
            //cloneArrow.transform.localPosition = pos;
            currentArrow = cloneArrow;

        }
        else
        {
            Destroy(currentArrow);
        }


    }

    #region LINE RENDERER

    private void CreateOrUpdateLineRenderer()
    {
        if (currentLineRenderer == null)
        {
            CreateLineRenderer();
        }
        else
        {
            UpdateLineRenderer();
        }
    }

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

    private void UpdateLineRenderer()
    {
        if (currentLineRenderer != null)
        {
            currentLineRenderer.positionCount = selectedBlocks.Count;

            for (int i = 0; i < selectedBlocks.Count; i++)
            {
                Vector3 pos = new Vector3(selectedBlocks[i].transform.position.x, selectedBlocks[i].transform.position.y + 0.2f, selectedBlocks[i].transform.position.z);
                currentLineRenderer.SetPosition(i, pos);
            }
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
            hitPos = hit.point;
        }

        hitPos.y = .25f; // default colored block Y pos
        return isHit;
    }
    #endregion
}
