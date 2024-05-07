using System.Collections;
using UnityEngine;

public class Gunner : Enemy
{
    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            Vector2 playerPosition = player.transform.position;
            RotateTowardsPlayer(playerPosition);
            weapon.Attack(playerPosition);
        }
    }

    protected override bool CheckPlayer(out Player player)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rangeDetected);

        player = null;
        
        if (colliders.Length == 0)
            return false;
        
        foreach (Collider2D findCollider in colliders)
            if (findCollider.TryGetComponent(out player))
            {
                return true;
            }

        return false;
    }
}
