using System;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private MonoBehaviour itemPrefab;


    private void Start()
    {
        if(itemPrefab)
            SpawnItem(itemPrefab);
    }

    public void SpawnItem(MonoBehaviour spawnWeaponPrefab)
    {
        itemPrefab = spawnWeaponPrefab;
        if (!itemPrefab) 
            return;
        Sprite itemSprite = itemPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemSprite;
        Collider2D coll = gameObject.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;
    }

    public bool Interactable() => true;

    public void Interact(Player player)
    {
        //player.PickUpItem(itemPrefab);
        Destroy(gameObject);
    }
}
