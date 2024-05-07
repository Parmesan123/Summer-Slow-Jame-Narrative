using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ErasableWall : MonoBehaviour, IInteractable
{
    [SerializeField] private Image eraserUI;
    [SerializeField] private VisualEffect effectToPlay;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool canBeErased;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public bool Interactable() => true;

    public void Interact(Player player)
    {
        if (!canBeErased)
            return;

        spriteRenderer.enabled = false;
        col.enabled = false;
        eraserUI.enabled = false;

        effectToPlay.Play();

        Destroy(gameObject, 2f);
    }

    public void MakeErasable() => canBeErased = true;
}
