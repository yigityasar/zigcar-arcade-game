using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EndlessGround : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject groundPrefab;       // Ground objesi
    [SerializeField] private Transform player;
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    [Header("Settings")]
    [SerializeField] private int tilesAhead = 2;            // Önünde kaç tile olacak
    [SerializeField] private int tilesBehind = 2;           // Arkasında kaç tile olacak
    [SerializeField] private float tileLength = 200f;       // Ground Z boyutu

    private Queue<GameObject> activeTiles = new Queue<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Otomatik olarak ObstacleSpawner bulunursa ata (inspektörde atanmamışsa)
        if (obstacleSpawner == null)
        {
            // Use FindAnyObjectByType for newer Unity versions (faster and not obsolete)
            obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
            if (obstacleSpawner != null)
                Debug.Log("ObstacleSpawner auto-assigned in EndlessGround.Start");
            else
                Debug.LogWarning("No ObstacleSpawner found in scene. Obstacles won't be spawned.");
        }

        for (int i = -tilesBehind; i < tilesAhead; i++)
        {
            SpawnTile(i * tileLength);
        }

    }

    void Update()
    {
        while (player.position.z + tilesAhead * tileLength > spawnZ)
        {
            SpawnTile(spawnZ);
        }

        while (activeTiles.Count > tilesAhead + tilesBehind)
        {
            RemoveTile();
        }
    }

    void SpawnTile(float zPosition)
    {
        GameObject tile = Instantiate(groundPrefab, new Vector3(0f, 0f, zPosition), Quaternion.identity);
        activeTiles.Enqueue(tile);

        // Eğer bir ObstacleSpawner varsa, bu tile üzerine obstacle'lar yerleştir
        if (obstacleSpawner != null)
        {
            obstacleSpawner.SpawnObstaclesOnTile(tile, zPosition);
        }
        else
        {
            // Bilgilendirici log (sahnede spawner yoksa)
            Debug.Log("Spawned tile but no ObstacleSpawner assigned; skipping obstacle spawn.");
        }

        if (zPosition >= spawnZ)
            spawnZ += tileLength;
    }


    void RemoveTile()
    {
        GameObject oldTile = activeTiles.Dequeue();
        Destroy(oldTile);
    }
    
    public float GetTileLength()
    {
        return tileLength;
    }
}
