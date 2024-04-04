using DG.Tweening;
using UnityEngine;

public class PopUpCanvasController : MonoBehaviour
{
    [SerializeField] GameObject premadeImage;
    [SerializeField] Vector3 desiredRot;

    private void Start()
    {
        PopTheImageUp(transform.position);
    }

    public void PopTheImageUp(Vector3 origin)
    {
        Quaternion quaternion = Quaternion.Euler(desiredRot);
        transform.rotation = quaternion;
        premadeImage.SetActive(true);

        transform.DOLocalMoveY(origin.y + 2, .25f).OnComplete(() =>
          {
              Destroy(gameObject, 0.1f);
          });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
