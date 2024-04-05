using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CenterPlacementManager : MonoSingleton<CenterPlacementManager>
{
    public event System.Action<CollectCenter> CollectCenterPlacedEvent;

    [Header("Layer Masks")]
    public LayerMask groundLayer;
    [SerializeField] LayerMask collectibleLayer;
    [SerializeField] LayerMask obstacleLayer;

    [Header("Config")]
    [SerializeField] bool hasLimitlessMove;
    [SerializeField] int maxMoveCount;

    [Header("References")]
    [SerializeField] CollectCenter centerPrefab;

    [Header("Debug")]
    [SerializeField] int currentPlacedCount;
    [SerializeField] CollectibleColor selectedColor;
    bool isInitialized;

    IEnumerator Start()
    {
        CanvasManager.instance.SetMoveCountText(maxMoveCount - currentPlacedCount);
        yield return null;

        isInitialized = true;
    }
    private void Update()
    {
        if (!GameManager.instance.isLevelActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!isInitialized) return;
            if (CheckUIClicked())
            {
                Debug.Log("UI clicked");
                return;
            }
            Vector3 _;
            Vector3 __;
            if (HitSpecifiedObject(collectibleLayer, out _)) return;
            if (HitSpecifiedObject(obstacleLayer, out __)) return;


            Vector3 _hitPos;
            if (HitSpecifiedObject(groundLayer, out _hitPos))
            {
                _hitPos.y = .5f;
                CollectCenter cloneCenter = Instantiate(centerPrefab, _hitPos, Quaternion.identity);
                cloneCenter.Initialize(selectedColor);
                CollectCenterPlacedEvent?.Invoke(cloneCenter);
                HandleMoveCountLogic();
            }
        }
    }

    private void HandleMoveCountLogic()
    {
        currentPlacedCount++;
        CanvasManager.instance.SetMoveCountText(maxMoveCount - currentPlacedCount);
        if (currentPlacedCount >= maxMoveCount && !hasLimitlessMove)
        {
            Debug.LogWarning("Max placement count is reached");
            GameManager.instance.OnNoMoveLeft();
        }
    }

    public void SetSelectedColor(CollectibleColor _color)
    {
        if (_color != CollectibleColor.NONE)
            selectedColor = _color;
    }

    bool CheckUIClicked()
    {
        GameObject currentSelected = EventSystem.current?.currentSelectedGameObject;

        if (currentSelected != null)
        {
            CanvasManager canvasManager = CanvasManager.instance;
            return currentSelected == canvasManager.BlockerObject ||
                currentSelected == canvasManager.RestartButton.gameObject ||
                   currentSelected == canvasManager.NextButton.gameObject;
        }

        return false;
    }


    public bool HitSpecifiedObject(LayerMask targetLayerMask, out Vector3 _hitPos)
    {
        bool isHit = false;
        _hitPos = Vector3.zero;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 300, targetLayerMask))
        {
            isHit = true;
            _hitPos = hit.point;
        }
        return isHit;
    }
}
