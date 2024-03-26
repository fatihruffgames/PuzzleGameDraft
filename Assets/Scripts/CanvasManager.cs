using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public void OnRestart()
    {
        GameManager.instance.OnTapRestart();
    }
}
