using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public void OnRestart()
    {
        GameManager.instance.OnTapRestart();
    }

    public void OnNext()
    {
        GameManager.instance.OnTapNext();
    }
}
