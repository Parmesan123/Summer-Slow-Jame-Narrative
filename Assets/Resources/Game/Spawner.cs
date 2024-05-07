using UnityEngine;

public class Spawner : MonoBehaviour
{
    //Enemy Spawner Script

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Debug.Log("Enemy spawned from: " + name);
    }
}
