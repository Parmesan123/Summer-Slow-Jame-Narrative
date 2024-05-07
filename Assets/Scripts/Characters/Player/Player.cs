using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCoolDown;

    [SerializeField] private float defaultTimeInvul;
    
    [SerializeField] private LayerMask levelLayer;
    
    [SerializeField] private PlayerMeleeWeapon currentMeleeWeapon;
    [SerializeField] private PlayerRangedWeapon currentRangedWeapon;

    [SerializeField] private Transform rangedWeaponContainer;
    [SerializeField] private Transform meleeWeaponContainer;

    public Transform MeleeWeaponContainer => meleeWeaponContainer;
    public Transform RangedWeaponContainer => rangedWeaponContainer;

    [NonSerialized] public Vector2 currentCheckPointPosition;

    private readonly Dictionary<Type, int> ammoStack = new Dictionary<Type, int>();

    private bool dashOnCoolDown;
    private bool isDashing;
    private bool invulnerable;

    private Rigidbody2D body;
    
    private static Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void Update()
    {
        if (!body) 
            return;
        RotateTowardsMouse();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashOnCoolDown)
            Dash();

        if (Input.GetKeyDown(KeyCode.Mouse1)) 
            if (currentMeleeWeapon)
                currentMeleeWeapon.Attack(CursorPosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            if (currentRangedWeapon)
                currentRangedWeapon.Attack(CursorPosition);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentRangedWeapon)
            {
                if (currentRangedWeapon is not Bow)
                {
                    currentRangedWeapon.ThrowWeapon(CursorPosition);
                    currentRangedWeapon = null;
                }
            }

            if (currentMeleeWeapon)
            {
                currentMeleeWeapon.ThrowWeapon(CursorPosition);
                currentRangedWeapon = null;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            IInteractable interactable = InteractableDetector();
            
            PickUpItem(interactable);
            //interactable?.Interact(this);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(currentRangedWeapon)
                Reload(currentRangedWeapon);
        }
    }



    private void FixedUpdate()
    {
        if (!isDashing && body)
            Move();
    }
        
    public void Initialize() => body = GetComponent<Rigidbody2D>();
    public void Deinitialize() => body = null; 

    
    private void RotateTowardsMouse()
    {
        Vector2 lookDirection = CursorPosition - body.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void Move()
    {
        Vector2 moveVector = InputVector();
        Vector2 newPosition = body.position + moveSpeed * Time.fixedDeltaTime * moveVector;
        body.MovePosition(newPosition);
    }

    private void Dash()
    {
        Vector2 directionDash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (directionDash == Vector2.zero) 
            return;

        isDashing = true;
        dashOnCoolDown = true;
        StartCoroutine(CoolDownDash());
        StartCoroutine(DashMoving(directionDash));
        
        IEnumerator CoolDownDash()
        {
            float time = dashCoolDown;

            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            dashOnCoolDown = false;
        }
        
        IEnumerator DashMoving(Vector2 dirDash)
        {
            float time = dashDuration;
            float speed = dashSpeed;
            
            while (time > 0.1)
            {
                Vector2 newPosition = body.position + speed * Time.fixedDeltaTime * dirDash;
                Vector2 newDirection = Vector2.zero;

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, dirDash, 0.8f, levelLayer);

                if (hitInfo)
                {
                    if (Math.Abs(Vector2.Dot(dirDash, hitInfo.normal)) >= 0.98f)
                        break;

                    newDirection = SlideCollision(dirDash, hitInfo) * (speed * time / dashDuration);
                }

                if (newDirection != Vector2.zero)
                    newPosition = body.position + newDirection * Time.fixedDeltaTime;

                body.MovePosition(newPosition);
                time -= Time.fixedDeltaTime;

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            isDashing = false;
            
            Vector3 SlideCollision(Vector2 direction, RaycastHit2D hitInfo) => Vector3.ProjectOnPlane(direction, hitInfo.normal);
        }
    }
    
    private Vector2 InputVector()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (inputVector.magnitude > 1) 
            inputVector = inputVector.normalized;

        return inputVector;
    }

    private IInteractable InteractableDetector()
    {
        Collider2D[] hitsInfo = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        if (hitsInfo.Length == 0)
            return null;

        return hitsInfo.Select(hitInfo => hitInfo.GetComponentInParent<IInteractable>())
                       .Where(interactable => interactable != null)
                       .FirstOrDefault(interactable => interactable.Interactable());
    }

    public void PickUpItem<T>(T item) where T : IInteractable
    {
        Vector2 playerPosition = transform.position;
        
        switch (item)
        {
            case PlayerMeleeWeapon weapon:
            {
                if (currentMeleeWeapon)
                    currentMeleeWeapon.CastOut(playerPosition);

                currentMeleeWeapon = weapon;
                currentMeleeWeapon.TakeUp(meleeWeaponContainer);
                return;
            }
            case PlayerRangedWeapon weapon:
            {
                if (currentRangedWeapon)
                    currentRangedWeapon.CastOut(playerPosition);

                currentRangedWeapon = weapon;
                currentRangedWeapon.TakeUp(rangedWeaponContainer);
                return;
            }
            case StackForAmmo stackForAmmo:
            {
                foreach (Type keyValuePair in ammoStack.Keys)
                {
                    if (keyValuePair == stackForAmmo.weaponType.GetType())
                    {
                        ammoStack[keyValuePair] += stackForAmmo.ammoInStack;
                        return;
                    }
                }
                ammoStack.Add(stackForAmmo.weaponType.GetType(), stackForAmmo.ammoInStack);
                return;
            }
        }
        
        IInteractable interactable = item as IInteractable;
        interactable?.Interact(this);
    }
    
    public override void TakeDamage(int damage, Vector2 damageDirection)
    {
        if (IgnoreDamage()) 
            return;
        base.TakeDamage(damage, damageDirection);

        if (!invulnerable)
        {
            StartCoroutine(TimerInvulnerable());
            StartCoroutine(DamageBoost());
        }

        IEnumerator TimerInvulnerable()
        {
            float time = defaultTimeInvul + 0.1f * (damage - 1);

            invulnerable = true;
        
            while (time > 0)
            {
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            invulnerable = false;
        }

        IEnumerator DamageBoost()
        {
            damageDirection = damageDirection.normalized;
            
            float time = (defaultTimeInvul + 0.1f * (damage - 1));
            float distance = damage;

            float speed = distance / time;

            isDashing = true;
            
            while (time > 0)
            {
                Vector2 newPosition = body.position +  speed * Time.fixedDeltaTime * damageDirection;
                body.MovePosition(newPosition);
                time -= Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            isDashing = false;
        }
        
    }

    private void Reload(PlayerRangedWeapon reloadWeapon)
    {
        foreach (Type ammoStackKey in ammoStack.Keys)
        {
            if (ammoStackKey != reloadWeapon.GetType())
                continue;
            ammoStack[ammoStackKey] -= reloadWeapon.Reload(ammoStack[ammoStackKey]);
            return;
        }
    }
    
    public bool IgnoreDamage() => isDashing || invulnerable;
    
    protected override void Death()
    {
        Debug.Log("Death");
    }
}
