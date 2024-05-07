using UnityEngine;
using UnityEngine.UI;

public class Eraser : MonoBehaviour, IInteractable
{
    [SerializeField] private ErasableWall objectToErase;
    [SerializeField] private Image eraserUI;

    public bool Interactable() => true;

    public void Interact(Player player)
    {
        objectToErase.MakeErasable();
        eraserUI.enabled = true;
        Destroy(gameObject);
    }
}
