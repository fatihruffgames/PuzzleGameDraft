using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CenterPlacementManager : MonoSingleton<CenterPlacementManager>
{
    [Header("Config")]
    [SerializeField] bool isTestinglevel;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int maxPlacementCount;

    [Header("References")]
    [SerializeField] CollectCenter centerPrefab;

    [Header("Debug")]
    [SerializeField] int currentPlacedCount;
    [SerializeField] CollectibleColor selectedColor;
    bool isInitialized;

    IEnumerator Start()
    {
        yield return null;

        isInitialized = true;   
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isInitialized) return;
            if (CheckUIClicked())
            {
                Debug.Log("UI clicked");
                return;
            }
            if (currentPlacedCount >= maxPlacementCount && !isTestinglevel)
            {
                Debug.LogWarning("Max placement count is reached");

                GameManager.instance.OnTapRestart();
                return;
            }

            Collider hitCollider;
            Vector3 _hitPos;
            if (HitDesiredObject(groundLayer, out hitCollider, out _hitPos))
            {
                CollectCenter cloneCenter = Instantiate(centerPrefab, _hitPos, Quaternion.identity);
                cloneCenter.Initialize(selectedColor);
                currentPlacedCount++;
            }
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
            return currentSelected == canvasManager.changeColorButton.gameObject ||
                   currentSelected == canvasManager.RestartButton.gameObject ||
                   currentSelected == canvasManager.NextButton.gameObject;
        }

        return false;
    }

    bool HitDesiredObject(LayerMask targetLayerMask, out Collider hitCollider, out Vector3 _hitPos)
    {
        bool isHit = false;
        hitCollider = null;
        _hitPos = Vector3.zero;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 300, targetLayerMask))
        {
            isHit = true;
            hitCollider = hit.collider;
            _hitPos = hit.point;
        }
        return isHit;
    }
}
