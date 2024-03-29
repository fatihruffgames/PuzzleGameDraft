using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorEnum
{
    RED, GREEN, BLUE, ORANGE
}

public class ColoredBlock : MonoBehaviour
{
    [Header("Config")]
    public ColorEnum ColorEnum;
    [Header("References")]
    public List<Material> colorMats;
    [Header("Debug")]
    [SerializeField] GridCell occupiedCell;
    Renderer _renderer;
    Transform mesh;

    private void Awake()
    {
        mesh = transform.GetChild(0);
        _renderer = mesh.GetComponent<Renderer>();

        switch (ColorEnum)
        {
            case ColorEnum.RED:
                _renderer.material = colorMats[0];
                break;
            case ColorEnum.GREEN:
                _renderer.material = colorMats[1];
                break;
            case ColorEnum.BLUE:
                _renderer.material = colorMats[2];
                break;
            case ColorEnum.ORANGE:
                _renderer.material = colorMats[3];
                break;
        }


    }

    private void Start()
    {
        occupiedCell = GridManager.instance.GetClosestGridCell(from: transform.position);
        occupiedCell.SetOccupied(state: true, this);
    }

    public GridCell GetOccupiedCell()
    {
        return occupiedCell;    
    }
}
