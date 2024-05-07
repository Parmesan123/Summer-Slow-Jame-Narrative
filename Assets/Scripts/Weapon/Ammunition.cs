using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Ammunition : MonoBehaviour, IDestroyed
{
	[SerializeField] private float speed = 6;

	protected int damage;

	protected Rigidbody2D rb;
	protected Vector2 flightDirection;

	public Vector2 FlightDirection => flightDirection;

	protected void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (flightDirection != Vector2.zero)
			Flight();
	}


	protected abstract void OnTriggerEnter2D(Collider2D other);

	private void Flight()
	{
		Vector2 newPosition = (Vector2)transform.position + speed * Time.fixedDeltaTime * flightDirection;
		rb.MovePosition(newPosition);
	}

	public void SetDirectionAndStart(Vector2 direction, Vector2 startPosition)
	{
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		transform.SetPositionAndRotation(startPosition, Quaternion.Euler(0f, 0f, angle - 90f));

		flightDirection = direction.normalized;
		gameObject.SetActive(true);
	}

	public void DestroyThisObject()
	{
		Destroy(gameObject);
	}

	public void SetDamage(int setDamage)
	{
		this.damage = Math.Abs(setDamage);
	}
}