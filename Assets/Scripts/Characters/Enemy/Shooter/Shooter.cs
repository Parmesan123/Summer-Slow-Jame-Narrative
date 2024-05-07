using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Shooter : Enemy
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

    public override void TakeDamage(int damage, Vector2 damageDirection)
    {
        base.TakeDamage(damage, damageDirection);

        StartCoroutine(DamageBoost());
        
        IEnumerator DamageBoost()
        {
            damageDirection = damageDirection.normalized;
            
            float time = 0.1f;

            while (time > 0)
            {
                Vector2 newPosition = (Vector2)transform.position + speedDamageBust * Time.fixedDeltaTime * damageDirection;
                transform.position = newPosition;
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
    }

    
    
    protected override void Death()
    {
        base.Death();
        weapon.DestroyWeapon();
        Destroy(gameObject);
    }
}
