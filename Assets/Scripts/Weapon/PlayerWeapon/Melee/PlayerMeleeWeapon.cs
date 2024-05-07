using System.Collections;
using UnityEngine;

public abstract class PlayerMeleeWeapon : PlayerWeapon, IInteractable
{
    [SerializeField] protected float distanceAttack;
    [SerializeField] protected float angleAttack;
    [SerializeField] protected AnimationCurve attackCurve;
    [SerializeField] protected Transform handTransform;

    protected SpriteRenderer currentRenderer;

    private void Awake()
    {
        currentRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public bool Interactable() => OnTheFloor;

    public void Interact(Player player)
    {
        TakeUp(player.MeleeWeaponContainer);
    }
    
    protected abstract IEnumerator AttackAnimation();
}
