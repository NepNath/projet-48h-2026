using UnityEngine;
using Random = UnityEngine.Random;

public class RandomAppleSpawner : MonoBehaviour
{
    [Header("Global Settings")]
    public GameObject prefab;
    public static int MaxAppleForGoal;

    [Header("Spawn Settings")]
    public int maxSpawnCountRange;
    public int minSpawnCountRange;
    private int spawnCount;

    [Header("Radius")]
    public float radius;

    void Awake()
    {
        spawnCount = Random.Range(minSpawnCountRange, maxSpawnCountRange);
        SpawnPrefabs();
        MaxAppleForGoal = spawnCount;

    }

    void SpawnPrefabs()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * radius;

            Instantiate(prefab, randomPos, Quaternion.identity);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
#endif
    }
}
