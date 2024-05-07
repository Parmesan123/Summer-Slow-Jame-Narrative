using UnityEngine;


public class PlayerBullet : Ammunition
{
	protected override void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.isTrigger)
		{
			Player player = other.GetComponent<Player>();
			if (player)
			{
				return;
			}
			
			gameObject.SetActive(false);
		}
		Enemy enemy = other.GetComponent<Enemy>();
		if (enemy)
		{
			enemy.TakeDamage(damage, flightDirection);
			gameObject.SetActive(false);
			return;
		}
	}
}