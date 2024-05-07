using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : PlayerRangedWeapon
{
	[SerializeField] private int hp;
	[SerializeField] private float angle;
	
	public override void Attack(Vector2 targetPointPosition)
	{
		if (onAttackDelay) 
			return;

		if (currentAmmo <= 0) 
			return;

		UpdateUI();

		currentAmmo -= 3;
		for (int i = 0; i < 3; i++)
		{
			poolAmmo.GetFreeElement(out Ammunition ammunition);
			Vector2 right = transform.right;
			Vector2 directionVector = (Quaternion.Euler(0,0, angle * (1 - i)) * (targetPointPosition  - AttackPointPosition) ).normalized;
			Vector2 offsetStartPosition = new Vector2(AttackPointPosition.x + right.x * 0.2f * (1 - i), AttackPointPosition.y);
			ammunition.SetDirectionAndStart(directionVector, offsetStartPosition);
		}
		StartCoroutine(DelayAfterAttack());
	}

	public void TakeDamage(int takeDamage)
	{
		hp -= takeDamage;
		if (hp <= 0)
		{
			Destroy(gameObject);
		}
	}
	
	protected override void UpdateUI()
	{
	}
}
