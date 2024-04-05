using UnityEngine;

public enum MovementDirection
{
    Vertical,
    Horizontal
}
public enum RotationAxis
{
    X,
    Y,
    Z
}
public class BladeController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] MovementDirection direction;
    [SerializeField] float moveSpeed;
    [SerializeField] float moveRange;

    [Header("Rotation")]
    [SerializeField] private RotationAxis rotationAxis;
    [SerializeField] private float rotationSpeed;

    private float initialPosition;
    private bool movingForward = true;

    void Start()
    {
        SpawnManager.instance.AddToList(transform);
        initialPosition = direction == MovementDirection.Vertical ? transform.position.z : transform.position.x;
    }

    void Update()
    {
        if (!GameManager.instance.isLevelActive) return;

        Move();
        Rotate();
    }

    private void Move()
    {
        float targetPosition = movingForward ? initialPosition + moveRange : initialPosition - moveRange;
        float newPosition = direction == MovementDirection.Vertical ? transform.position.z : transform.position.x;

        float step = moveSpeed * Time.deltaTime;
        newPosition = Mathf.MoveTowards(newPosition, targetPosition, step);

        if (direction == MovementDirection.Vertical)
            transform.position = new Vector3(transform.position.x, transform.position.y, newPosition);
        else
            transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);

        if (Mathf.Abs(newPosition - targetPosition) < 0.01f)
            movingForward = !movingForward;
    }

    void Rotate()
    {
        // Determine the rotation axis
        Vector3 axis = Vector3.zero;
        switch (rotationAxis)
        {
            case RotationAxis.X:
                axis = Vector3.right;
                break;
            case RotationAxis.Y:
                axis = Vector3.down;
                break;
            case RotationAxis.Z:
                axis = Vector3.forward;
                break;
        }

        // Rotate the object continuously
        transform.Rotate(axis, rotationSpeed * Time.deltaTime);
    }
}
