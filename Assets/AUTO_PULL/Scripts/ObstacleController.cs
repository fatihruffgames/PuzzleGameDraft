using UnityEngine;

public enum MovementDirection
{
    Vertical,
    Horizontal
}


public class ObstacleController : MonoBehaviour
{

    public MovementDirection direction;
    public float moveSpeed;
    public float moveRange;

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
}
