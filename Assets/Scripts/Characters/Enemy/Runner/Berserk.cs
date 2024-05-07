using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody2D))]
public class Berserk : Enemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timeToPreparationJump;
    [SerializeField] private float timeToPreparationThrow;
    [SerializeField] private float jumpCoolDownTime;
    [SerializeField] private float throwCoolDownTime;
    [SerializeField] private List<BerserkAxe> axes = new List<BerserkAxe>();

    private Rigidbody2D rb;
    private bool isReady = true;
    private bool onTheJump;

    protected override void Start()
    {
        foreach (BerserkAxe berserkAxe in axes)
            berserkAxe.InitializedWeapon();

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (CheckPlayer(out Player player))
        {
            if (!onTheJump)
                RotateTowardsPlayer(player.transform.position);

            if (isReady)
                StartCoroutine(PreparationForAttack(player));
        }
    }

    private IEnumerator PreparationForAttack(Player player)
    {
        Vector2 finishPosition = player.transform.position;
        int randValue = Random.Range(0, 100);
        isReady = false;
        
        if (randValue < 0)
        {

            yield return StartCoroutine(Preparation(timeToPreparationJump));
            StartCoroutine(Jump(finishPosition));
        }
        else
        {
            yield return StartCoroutine(Preparation(timeToPreparationJump));
            StartCoroutine(ThrowAxe(finishPosition));
        }

        IEnumerator Preparation(float prepTime)
        {
            float time = prepTime;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            
            finishPosition = player.transform.position;
        }
    }

    private IEnumerator Jump(Vector2 finishPosition)
    {
        onTheJump = true;

        Vector2 directionJump = finishPosition - rb.position;

        float distance = Vector2.Distance(finishPosition, transform.position);
        float time = distance / moveSpeed;

        directionJump = directionJump.normalized;

        foreach (BerserkAxe berserkAxe in axes)
            berserkAxe.RotateAttack(time);

        while (time > 0)
        {
            Vector2 newPosition = rb.position + moveSpeed * Time.fixedDeltaTime * directionJump;
            rb.MovePosition(newPosition);

            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        onTheJump = false;
        StartCoroutine(CoolDownAttack(jumpCoolDownTime));
    }

    private IEnumerator ThrowAxe(Vector2 targetPosition)
    {
        float time = 0;
        foreach (BerserkAxe berserkAxe in axes)
        {
            berserkAxe.Attack(targetPosition);
            time = (targetPosition - (Vector2)transform.position).magnitude / berserkAxe.SpeedFlying * 2f;
        }

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        StartCoroutine(CoolDownAttack(throwCoolDownTime));
    }
    
    private IEnumerator CoolDownAttack(float timeCoolDown)
    {
        float time = timeCoolDown;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        isReady = true;
    }

    public override void TakeDamage(int damage, Vector2 damageDirection)
    {
        base.TakeDamage(damage, damageDirection);
        
    }
}
