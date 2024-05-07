using UnityEngine;

public class ShieldBearer : Enemy
{
    [SerializeField] private float speedRotate;

    private float angle;

    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            Vector2 playerPosition = player.transform.position;
            RotateTowardsPlayer(playerPosition);
            weapon.Attack(transform.up);
        }
    }

    protected override void RotateTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 castPosition = transform.position;
        Vector2 directionRotate = (playerPosition - castPosition).normalized;
        angle = Mathf.Atan2(directionRotate.y, directionRotate.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), speedRotate * Time.fixedDeltaTime);
    }

    protected override void Death()
    {
        base.Death();
        Destroy(gameObject);
    }
}
