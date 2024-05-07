using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : BotRangeWeapon
{
    private bool onAttackDelay;
    public override void Attack(Vector2 targetPointPosition)
    {
        if (onAttackDelay) 
            return;

        if (currentAmmo <= 0)
        {
            if (!onReload)
                Reload();
            return;
        }

        currentAmmo -= 1;
        poolObject.GetFreeElement(out BotBullet ammunition);
        Vector2 directionVector = (targetPointPosition - FirePointPosition).normalized;
        ammunition.SetDirectionAndStart(directionVector, FirePointPosition);
        StartCoroutine(DelayAfterAttack());
        
        IEnumerator DelayAfterAttack()
        {
            onAttackDelay = true;
            float timer = 1 / rateOfAttackPerSec;

            while (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            onAttackDelay = false;
        }
    }
    
    protected override void Reload()
    {
        StartCoroutine(DelayBeforeReloading());
        
        IEnumerator DelayBeforeReloading()
        {
            onReload = true;
            
            float time = reloadDuration;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            currentAmmo = maxNumberOfAmmo;
            onReload = false;
        }
    }
    
}
