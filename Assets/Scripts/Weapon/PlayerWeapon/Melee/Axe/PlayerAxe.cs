using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAxe : PlayerMeleeWeapon
{
    [SerializeField] private float speedRotate;
    [SerializeField] private float rotateRadius;
    [SerializeField] private int damage;

    private Vector2 defaultPosition;
    private Quaternion defaultRotation;

    private bool onAttack;

    private Vector2 holder;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (onAttack)
            return;
        
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            Vector2 damageDirection =  other.transform.position - GetComponentInParent<Player>().transform.position ;
            enemy.TakeDamage(damage, damageDirection);
        }
    }

    private void SetDefaultPosition(bool onJump)
    {
        if (!onJump) 
            transform.SetLocalPositionAndRotation(defaultPosition, defaultRotation);
    }
    
    public override void Attack(Vector2 target)
    {
        StartCoroutine(ChangeAttack());

        IEnumerator ChangeAttack()
        {
            float time = 0;

            while (Input.GetKey(KeyCode.Mouse1))
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            
            if (time > 1f)
            {
                ThrowAxe(target);
                yield break;
            }
            
            BaseAttack(target);
        }
    }

    private void BaseAttack(Vector2 targetPoint)
    {
        
    }

    private void ThrowAxe(Vector2 targetPoint)
    {
        
    }
    
    private void RotateAroundHolder()
    {
        float rotationAngle = speedRotate * Time.fixedDeltaTime;
        transform.RotateAround(holder, Vector3.forward, rotationAngle);
    }

    protected override void UpdateUI()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator AttackAnimation() => throw new System.NotImplementedException();
}
