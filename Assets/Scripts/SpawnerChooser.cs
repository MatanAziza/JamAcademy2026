using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace StarterAssets
{
public class RandomSpawner : MonoBehaviour
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
                Debug.Log("hihi");
                return ;
            }
            StarterAssetsInputs playerInput = player.GetComponent<StarterAssetsInputs>();
            if (playerInput == null){
                Debug.Log("houhou");
                return ;
            }
            if (playerInput.interact && player != null && GetComponent<Collider>().enabled && !spawned)
            {
                spawned = true;
                Debug.Log("hehe");
                SpawnEnemies();
            }
        }
}

// Extension method for shuffling (Fisher-Yates algorithm)
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }
}
}