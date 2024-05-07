using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] protected BotWeapon weapon;
    [SerializeField] protected float rangeDetected;
    [SerializeField] protected float speedDamageBust;
    
    protected virtual void Start()
    {
        if (weapon)
            weapon.InitializedWeapon();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player)
        {
            Vector2 directionDamageBoost = player.transform.position - transform.position;
            player.TakeDamage(1, directionDamageBoost);
        }
    }

    protected virtual bool CheckPlayer(out Player player)
    {
        player = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rangeDetected);

        foreach (Collider2D collide in colliders)
        {
            if (collide.isTrigger)
                continue;
            player = collide.GetComponentInParent<Player>();
            if (player)
                break;
        }

        if (!player)
            return false;
        
        Vector2 directionPlayer = player.transform.position - transform.position;
        
        RaycastHit2D[] hitsInfo = Physics2D.RaycastAll(transform.position, directionPlayer, rangeDetected);

        foreach (RaycastHit2D hitInfo in hitsInfo)
        {
            if (hitInfo.collider.isTrigger) 
                continue;
            player = hitInfo.collider.GetComponentInParent<Player>();

            if (player)
            {
                return true;
            }
            break;
        }
        player = null;
        return false;
    }

    protected virtual void RotateTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 castPosition = transform.position;
        Vector2 directionRotate = playerPosition - castPosition;
        float angle = Mathf.Atan2(directionRotate.y, directionRotate.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}
