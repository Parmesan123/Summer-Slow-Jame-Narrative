using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public abstract class PlayerRangedWeapon : PlayerWeapon, IInteractable
{
    [SerializeField] protected Ammunition ammunitionPrefab;
    [SerializeField] protected float reloadDuration;
    [SerializeField] protected int maxNumberOfAmmo;
    
    [SerializeField] protected int currentAmmo;

    [SerializeField] protected Transform attackPointTransform;
    [SerializeField] protected Transform ammoContainer;
    
    
    protected Pool<Ammunition> poolAmmo;
    protected Vector2 AttackPointPosition => attackPointTransform.position;

    public Type AmmoType => ammunitionPrefab.GetType();

    private void Start()
    {
        currentAmmo = maxNumberOfAmmo;
    }

    public bool Interactable() => OnTheFloor;

    public void Interact(Player player)
    {
        TakeUp(player.RangedWeaponContainer);
    }
    
    public override void Attack(Vector2 targetPointPosition)
    {
        if (onAttackDelay) 
            return;

        if (currentAmmo <= 0) 
            return;

        UpdateUI();

        currentAmmo -= 1;
        poolAmmo.GetFreeElement(out Ammunition ammunition);
        Vector2 directionVector = (targetPointPosition - AttackPointPosition).normalized;
        ammunition.SetDirectionAndStart(directionVector, attackPointTransform.position);
        StartCoroutine(DelayAfterAttack());
    }

    public override void ThrowWeapon(Vector2 targetPointPosition)
    {
        poolAmmo.DestroyPool();
        base.ThrowWeapon(targetPointPosition);
    }

    public override void TakeUp(Transform weaponContainer)
    {
        poolAmmo = new Pool<Ammunition>(ammunitionPrefab, ammoContainer, true);
        poolAmmo.CreatePool(1);
        foreach (Ammunition ammunition in poolAmmo.PoolList)
        {
            ammunition.SetDamage(damage);
        }
        base.TakeUp(weaponContainer);
    }

    public override void CastOut(Vector2 playerPosition)
    {
        base.CastOut(playerPosition);
        poolAmmo.DestroyPool();
    }

    public int Reload(int ammoInStock)
    {
        
        int needToMaxAmmo = maxNumberOfAmmo - currentAmmo;
        if (needToMaxAmmo == 0 || ammoInStock <= 0)
            return 0;
        StartCoroutine(TimerReload());
        if (ammoInStock <= needToMaxAmmo)
        {
            return ammoInStock;
        }
        return needToMaxAmmo;

        IEnumerator TimerReload()
        {
            float timer = reloadDuration;

            while (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            
            if (ammoInStock <= needToMaxAmmo)
            {
                currentAmmo += ammoInStock;
            }

            currentAmmo = maxNumberOfAmmo;
        }
    }
}
