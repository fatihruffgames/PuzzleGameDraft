using UnityEngine;

public class MouseFollowForHand : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Vector3 _hit;
        if (CenterPlacementManager.instance.HitSpecifiedObject(CenterPlacementManager.instance.groundLayer, out _hit))
        {
            _hit.y = 1.5f;
            _hit.x -= .21f;
            transform.position = Vector3.Lerp(transform.position, _hit, 5f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _animator.Play("handanim");
        }
    }
}