using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : PlayerMeleeWeapon
{
    public override void Attack(Vector2 pointOfAttack)
    {
        StartCoroutine(AttackAnimation());

        Vector2 castedTransform = transform.position;
        Vector2 directionAttack = pointOfAttack - castedTransform;

        Collider2D[] hitInfos = Physics2D.OverlapCircleAll(castedTransform, distanceAttack);

        foreach (Collider2D hitInfo in hitInfos)
        {
            if (hitInfo.transform.parent)
            {
                if (hitInfo.transform.parent.TryGetComponent(out Enemy enemy))
                {
                    Vector2 directionToEnemy = enemy.transform.position - transform.position;
                    if (Vector2.Angle(directionToEnemy, directionAttack) <= angleAttack / 2)
                        enemy.TakeDamage(damage, (Vector2)hitInfo.transform.position - castedTransform);
                }
            }
        }
    }


    protected override IEnumerator AttackAnimation()
    {
        currentRenderer.enabled = true;
        yield return RotateTowards(new Vector3(0, 0, angleAttack));

        yield return RotateTowards(Vector3.zero);
        currentRenderer.enabled = false;

        IEnumerator RotateTowards(Vector3 targetRotation)
        {
            Quaternion startRotation = handTransform.localRotation;
            float currentTime = 0f, targetTime = 1f;

            while (currentTime != targetTime)
            {
                currentTime = Mathf.MoveTowards(currentTime, targetTime, Time.deltaTime / 0.25f);

                handTransform.localRotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetRotation), attackCurve.Evaluate(currentTime));

                yield return new WaitForEndOfFrame();
            }
        }
    }
    
    
    protected override void UpdateUI()
    {
        
    }
}
