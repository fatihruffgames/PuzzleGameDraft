using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleColor
{
    NONE,RED, GREEN, BLUE, ORANGE
}


public class BaseColoredClass : MonoBehaviour
{
    [Header("Config")]
    public CollectibleColor color;

    [Header("References")]
    public Transform mesh;
    public List<Material> colorMats;

    [Header("Debug")]
    protected Renderer _renderer;

    public void SetColor(CollectibleColor _color = CollectibleColor.NONE)
    {
        if(_color != CollectibleColor.NONE)
        {
            color = _color;
        }

        _renderer = mesh.GetComponent<Renderer>();

        switch (color)
        {
            case CollectibleColor.RED:
                _renderer.material = colorMats[0];
                break;
            case CollectibleColor.GREEN:
                _renderer.material = colorMats[1];
                break;
            case CollectibleColor.BLUE:
                _renderer.material = colorMats[2];
                break;
            case CollectibleColor.ORANGE:
                _renderer.material = colorMats[3];
                break;
        }
    }

    #region GETTERS & SETTERS

    public CollectibleColor GetColor()
    {
        return color;
    }

    #endregion
}
