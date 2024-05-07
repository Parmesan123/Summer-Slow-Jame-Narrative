using UnityEngine;

public class Game : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public void InitializeAll() 
    {
        player.Initialize();
    }

    public void DeinitializeAll() 
    {
        player.Deinitialize();
    }

    public void TeleportPlayerToLastCheckPoint() 
    {
        player.transform.position = player.currentCheckPointPosition;
        Camera.main.transform.position = player.transform.position;
    }
}
