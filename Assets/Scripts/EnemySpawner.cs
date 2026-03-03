using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject[] enemies;
    public int spawnerId;

    // Update is called once per frame
    public void SpawnEnemy(){
        Transform transform = GetComponent<Transform>();
        Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.identity, transform);
    } 
}
