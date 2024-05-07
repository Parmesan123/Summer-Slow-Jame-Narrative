using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player)) 
        {
            player.currentCheckPointPosition = transform.position;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
