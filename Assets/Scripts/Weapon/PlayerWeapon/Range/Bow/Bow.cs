using UnityEngine;


public class Bow : PlayerRangedWeapon
{
    public override void TakeUp(Transform weaponContainer)
    {
        base.TakeUp(weaponContainer);
        foreach (Ammunition ammunition in poolAmmo.PoolList)
        {
            ammunition.GetComponent<Arrow>().LiftOnTheFloorEvent += Reload;
        }
    }

    public void Reload()
    {
        currentAmmo = 1;
    }
    

    protected override void UpdateUI()
    {
    }

    private void ReloadUI()
    {

    }
}