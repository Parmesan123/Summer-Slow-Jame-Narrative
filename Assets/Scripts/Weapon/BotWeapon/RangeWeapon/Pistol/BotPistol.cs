using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPistol : BotRangeWeapon
{
	public override void Attack(Vector2 targetPointPosition)
	{
		if (currentAmmo <= 0)
		{
			if(!onReload)
				Reload();
			return;
		}
		currentAmmo -= 1;
		poolObject.GetFreeElement(out BotBullet botBullet);
		Vector2 directionVector = (targetPointPosition - (Vector2)FirePointPosition).normalized;
		botBullet.SetDirectionAndStart(directionVector, FirePointPosition);
	}
}
