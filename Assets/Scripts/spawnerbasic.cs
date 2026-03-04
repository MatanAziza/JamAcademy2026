using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace StarterAssets
{
public class BasicSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawners;
    [SerializeField] public int numberToSpawn = 3;
    private bool spawned = false;

    public void SpawnEnemies()
    {
        // Create indices and shuffle
        var indices = Enumerable.Range(0, spawners.Length).ToList();
        indices.Shuffle();

        for (int i = 0; i < numberToSpawn && i < indices.Count; i++)
        {
            int index = indices[i];
            var spawnerScript = spawners[index].GetComponent<EnemySpawner>();
            spawnerScript?.SpawnEnemy();
        }
    }

    void OnTriggerStay(Collider other)
        {
            // Vérifie que l'autre est bien le joueur
            ThirdPersonController player = other.GetComponent<ThirdPersonController>();
            if (player == null){
                return;
            }
            if (player != null && !spawned)
            {
                spawned = true;
                SpawnEnemies();
            }
        }
}}