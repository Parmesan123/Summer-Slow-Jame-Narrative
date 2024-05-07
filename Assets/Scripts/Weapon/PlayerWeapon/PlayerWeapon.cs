using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public abstract class PlayerWeapon : Weapon
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite paintedSprite;
    [SerializeField] private float speedThrow;
    [SerializeField] private float speedThrowRotation;

    private bool throwWeapon;

    protected bool onAttackDelay;

    public bool OnTheFloor { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        OnTheFloor = !GetComponentInParent<Player>();
    }

    public virtual void TakeUp(Transform weaponContainer)
    {
        transform.SetParent(weaponContainer, false);
        transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        
        OnTheFloor = false; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!throwWeapon)
            return;
        if (!other.isTrigger)
        {
            if (other.GetComponentInParent<Player>())
                return;
            StopAllCoroutines();
            Destroy(gameObject);
        }
        Enemy enemy = GetComponent<Enemy>();
        if (enemy)
        {
            Vector2 directionDamageBoost = other.transform.position - transform.position;
            enemy.TakeDamage(damage * 2, directionDamageBoost);
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
    
    public virtual void CastOut(Vector2 playerPosition)
    {
        transform.SetParent(null, true);
        OnTheFloor = true;
        transform.position = playerPosition;
    }
    
    
    protected IEnumerator DelayAfterAttack()
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
    
    
    public virtual void ThrowWeapon(Vector2 targetPointPosition)
    {
        transform.SetParent(null, true);
        throwWeapon = true;

        Vector2 directionThrow = (targetPointPosition - (Vector2)transform.position).normalized;

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        StartCoroutine(ThrowWeaponRoutine());
        
        IEnumerator ThrowWeaponRoutine()
        {
            while (true)
            {
                Vector2 newPosition = (Vector2)transform.position + speedThrow * Time.fixedDeltaTime * directionThrow;
                rb.MovePosition(newPosition);
                transform.eulerAngles = new Vector3(0,0,transform.eulerAngles.z - speedThrowRotation * Time.fixedDeltaTime);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
    }
    
    
    
    protected abstract void UpdateUI();
}
