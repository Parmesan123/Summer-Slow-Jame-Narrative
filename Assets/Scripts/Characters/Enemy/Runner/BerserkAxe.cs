using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(BoxCollider2D))]
public partial class BerserkAxe : BotWeapon
{
    [SerializeField] private float speedRotate;
    [SerializeField] private float speedThrowRotation;
    [SerializeField] private float speedFlying;
    [SerializeField] private float rotateRadius;
    [SerializeField] private int indexAxe;
    [SerializeField] private float heightParabola;
    [SerializeField, Range(0f, 1f)] private float coef;
    private const int TotalCount = 2;
    public float SpeedFlying => speedFlying;

    private Vector2 defaultPosition;
    private Quaternion defaultRotation;
    private Collider2D axeCollider;
    private Berserk holder;
    private bool onAttack;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!onAttack) 
            return;
        if (!other.TryGetComponent(out Player player)) 
            return;
        Vector2 damageDirection = player.transform.position - transform.position;
        player.TakeDamage(damage, damageDirection);
    }

    private void RotateAroundHolder()
    {
        float rotationAngle = speedRotate * Time.fixedDeltaTime;
        transform.RotateAround(holder.transform.position, Vector3.forward, rotationAngle);
    }

    private void SetAttackPosition()
    {
        const float angle = 360f / TotalCount;
        float angleInRadius = angle * indexAxe * Mathf.Deg2Rad;
        float x = rotateRadius * Mathf.Cos(angleInRadius);
        float y = rotateRadius * Mathf.Sin(angleInRadius);
        Vector2 positionKnives = new Vector2(x, y);

        transform.localPosition = positionKnives;
    }

    private void SetDefaultPosition()
    {
        transform.SetLocalPositionAndRotation(defaultPosition, defaultRotation);
    }

    public void RotateAttack(float time)
    {
        SetAttackPosition();
        StartCoroutine(AttackCoroutine());
        onAttack = true;
        
        IEnumerator AttackCoroutine()
        {
            float timer = time;

            while (timer > 0)
            {
                RotateAroundHolder();
                timer -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            onAttack = false;
            
            SetDefaultPosition();
        }
    }
    
    public override void Attack(Vector2 targetPointAttack)
    {
        onAttack = true;

        transform.localPosition = Vector2.zero;
        
        StartCoroutine(AttackCoroutine(targetPointAttack));
        
        
        IEnumerator AttackCoroutine(Vector2 targetPoint)
        {
            Vector2 startPoint = transform.position;
            
            Vector2 directionVector = (targetPoint - startPoint).normalized;
            float distance = Vector2.Distance(startPoint, targetPoint);
            float time = distance / speedFlying;
            distance /= 2;
            heightParabola = distance * distance;
            
            Vector2 perpendicularVector = Vector2.zero;
            
            if (indexAxe == 1)
                perpendicularVector = new Vector2(-directionVector.y, directionVector.x);
            else if (indexAxe == 2) 
                perpendicularVector = new Vector2(directionVector.y, -directionVector.x);
            
            float timer = 0;
            
            while (timer < time)
            {
                timer += Time.fixedDeltaTime;
                
                transform.eulerAngles = new Vector3(0,0,transform.eulerAngles.z - speedThrowRotation * Time.fixedDeltaTime);
                    
                float dX = timer * speedFlying;
                float dY = (-Mathf.Pow((dX - distance), 2) + heightParabola) * coef;
                //float dY = 2 * heightParabola * Mathf.Sqrt(Mathf.Abs(dX * distance - dX * dX)) / distance;
                
                Vector2 offSetX = dX * directionVector;
                Vector2 offSetY = dY * perpendicularVector;
                
                Vector2 newPosition = startPoint + offSetX + offSetY;
                
                if (newPosition.x is float.NaN)
                {
                    Debug.Log("");
                }
                transform.position = newPosition;
                

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            
            if (targetPoint == (Vector2)holder.transform.position)
            {
                SetDefaultPosition();
                onAttack = false;
                yield break;
            }

            StartCoroutine(AttackCoroutine(holder.transform.position));
        }
    }

    public override void InitializedWeapon()
    {
        holder = GetComponentInParent<Berserk>();
        defaultPosition = transform.localPosition;
        defaultRotation = transform.localRotation;
    }

    public override void DestroyWeapon()
    {
        Destroy(gameObject);
    }
}
