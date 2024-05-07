using UnityEngine;
using UnityEngine.Serialization;


public abstract class Weapon : MonoBehaviour
{
	[SerializeField] protected float rateOfAttackPerSec;
	[SerializeField] protected int damage;

	public abstract void Attack(Vector2 targetPointAttack);
}
