using System;
using System.Collections;
using UnityEngine;


public class BotShield : BotRangeWeapon
{
    [SerializeField] private int countBullet;
    [SerializeField] private float angleAttack;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerBullet>(out PlayerBullet bullet))
        {
            RaycastHit2D[] hitsInfo = Physics2D.RaycastAll(bullet.transform.position, bullet.FlightDirection);
            bullet.gameObject.SetActive(false);
            foreach (RaycastHit2D raycastHit2D in hitsInfo)
            {
                if (!raycastHit2D.collider.GetComponent<BotShield>())
                    continue;
                Vector2 normalVector = raycastHit2D.normal;
                Vector2 newDirection = Vector2.Reflect(bullet.FlightDirection, normalVector);
                poolObject.GetFreeElement(out BotBullet ammunition);
                ammunition.SetDirectionAndStart(newDirection, raycastHit2D.point);
                return;
            }
        }
    }

    public override void Attack(Vector2 directionAttack)
    {
        if (currentAmmo <= 0)
        {
            if (!onReload)
                Reload();
            return;
        }

        currentAmmo -= countBullet;

        int limit = (int)Math.Ceiling(countBullet / (double)2);
        for (int i = 0; i < limit; i++)
        {
            if (i == 0)
            {
                poolObject.GetFreeElement(out BotBullet ammunition);
                ammunition.SetDirectionAndStart(directionAttack, FirePointPosition);
                continue;
            }
            CreateBullet(limit - i);
            CreateBullet(i - limit);
        }

        void CreateBullet(int i)
        {
            poolObject.GetFreeElement(out BotBullet ammunition);
            Vector2 directionVector = (Quaternion.Euler(0,0, angleAttack / limit * i) * (directionAttack) ).normalized;
            ammunition.SetDirectionAndStart(directionVector, FirePointPosition);
        }
    }
    
}
