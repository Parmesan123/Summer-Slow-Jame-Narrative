using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float speed;

    private Transform targetTransform;

    void Start()
    {
        targetTransform = FindObjectOfType<Player>().transform;

        Vector2 targetPosition = targetTransform.position;
        transform.position = targetPosition;
    }

    void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector2 cameraFollow = Vector2.Lerp(transform.position, targetTransform.position, speed * Time.deltaTime);
        transform.position = new Vector3(cameraFollow.x, cameraFollow.y, -10);
    }
}
