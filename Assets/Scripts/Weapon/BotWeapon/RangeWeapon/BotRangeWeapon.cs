using System.Collections;
using UnityEngine;

public abstract class BotRangeWeapon : BotWeapon
{
    [SerializeField] private BotBullet ammoPrefab;
    [SerializeField] private Transform ammoContainer;
    [SerializeField] private Transform firePoint;
    [SerializeField] protected float reloadDuration;
    [SerializeField] protected int maxNumberOfAmmo;

    protected int currentAmmo;

    protected bool onReload;
    
    protected Vector2 FirePointPosition => firePoint.position;
    
    protected Pool<BotBullet> poolObject;

    public override void InitializedWeapon()
    {
        poolObject = new Pool<BotBullet>(ammoPrefab, ammoContainer, true);
        poolObject.CreatePool(5);
        foreach (BotBullet bullet in poolObject.PoolList)
        {
             bullet.SetDamage(damage);
        }

        currentAmmo = maxNumberOfAmmo;
    }

    public override void DestroyWeapon()
    {
        poolObject.DestroyPool();
    }

    protected virtual void Reload()
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
