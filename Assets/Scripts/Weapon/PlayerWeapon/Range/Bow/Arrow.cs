using System;
using UnityEngine;

public class Arrow : Ammunition
{
    private bool isStuck = false;

    public event Action LiftOnTheFloorEvent;
    

    protected override void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.isTrigger)
        {
            Player player = other.GetComponent<Player>();
            if (player)
            {
                if (!isStuck) 
                    return;
                rb.bodyType = RigidbodyType2D.Dynamic;
                transform.rotation = Quaternion.identity;
                LiftOnTheFloorEvent?.Invoke();
                transform.SetParent(null, true);
                isStuck = false;
                gameObject.SetActive(false);
                return;
            }
            Stuck();
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.OnDeathEvent += DropTheFloor;
            transform.SetParent(enemy.transform, true);
            enemy.TakeDamage(damage, flightDirection);
            Stuck();
            return;
        }

        BotShield botShield = other.GetComponent<BotShield>();
        if (botShield)
        {
            botShield.GetComponentInParent<Enemy>().OnDeathEvent += DropTheFloor;
            transform.SetParent(botShield.transform, true);
            Stuck();
            return;
        }

        void Stuck()
        {
            rb.bodyType = RigidbodyType2D.Static;
            flightDirection = Vector3.zero;
            isStuck = true;
        }
    }

    private void DropTheFloor()
    {
        transform.SetParent(null, true);
    }
}

