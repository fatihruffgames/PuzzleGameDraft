using System.Collections.Generic;
using UnityEngine;

public enum CollectibleColor
{
    NONE, RED, GREEN, BLUE, ORANGE
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
    public void SetMesh(CollectibleColor _color = CollectibleColor.NONE)
    {
        if (_color != CollectibleColor.NONE)
        {
            color = _color;
        }

        switch (color)
        {
            case CollectibleColor.RED:
                mesh.GetChild(0).gameObject.SetActive(true);
                break;
            case CollectibleColor.GREEN:
                mesh.GetChild(1).gameObject.SetActive(true);
                break;
            case CollectibleColor.BLUE:
                mesh.GetChild(2).gameObject.SetActive(true);
                break;
            case CollectibleColor.ORANGE:
                mesh.GetChild(3).gameObject.SetActive(true);
                break;
        }

        mesh.GetComponent<Renderer>().enabled = false;
    }
    public void SetColor(CollectibleColor _color = CollectibleColor.NONE)
    {
        if (_color != CollectibleColor.NONE)
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
