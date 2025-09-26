using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text _ScoreText;

    [Header("Obstacle Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;  
    [SerializeField] private float obstacleY = 0.55f;       // Obstacle'ların Y pozisyonu
    
    [Header("Difficulty Settings")]
    [SerializeField] private float initialGapSize = 8f;     // Başlangıçta aralar arası mesafe
    [SerializeField] private float minGapSize = 4f;         // Minimum gap mesafesi
    
    [Header("Spawn Settings")]
    [SerializeField] private int obstacleRowsPerTile = 15;   // Her tile'da kaç sıra obstacle olacak
    [SerializeField, Tooltip("Minimum spacing between obstacles on the same row (smaller = more frequent)")] private float minObstacleSpacing = 0.05f;
    [SerializeField, Tooltip("Maximum spacing between obstacles on the same row")] private float maxObstacleSpacing = 0.2f;
    
    private float currentGapSize;
    private int currentScore = 0;
    private int Score = 0;
    
    // Ground boyutları (EndlessGround'dan aldığınız bilgilere göre)
    private const float GROUND_WIDTH = 15f;     // Ground scale X = 15
    private const float GROUND_LENGTH = 200f;   // Ground scale Z = 200
    
    void Start()
    {
        currentGapSize = initialGapSize;

        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("ObstacleSpawner: No obstacle prefabs assigned in inspector.");
        }
        else
        {
            Debug.Log($"ObstacleSpawner initialized with {obstaclePrefabs.Length} prefabs. Initial gap: {currentGapSize}");

            // Ölçümleri al ve uyarıları göster (ör. yanlışlıkla ground prefab atandıysa tespit etmek için)
            for (int i = 0; i < obstaclePrefabs.Length; i++)
            {
                var prefab = obstaclePrefabs[i];
                if (prefab == null)
                {
                    Debug.LogWarning($"ObstacleSpawner: obstaclePrefabs[{i}] is null.");
                    continue;
                }

                float measured = MeasurePrefabWidth(prefab);
                if (measured > 0f)
                {
                    Debug.Log($"  Prefab '{prefab.name}' measured width: {measured:F2}");

                    // Eğer prefab ground genişliğine çok yakınsa uyar
                    if (measured >= GROUND_WIDTH * 0.8f)
                    {
                        Debug.LogWarning($"  Prefab '{prefab.name}' appears very wide ({measured:F2}) — did you assign the ground prefab by mistake?");
                    }
                }
                else
                {
                    Debug.Log($"  Prefab '{prefab.name}' has no readable mesh bounds; fallback width heuristics will be used.");
                }
            }
        }
    }

    // Awake runs before other scripts' Start(); ensure gap size is initialized early
    private void Awake()
    {
        if (currentGapSize <= 0f)
            currentGapSize = initialGapSize;
    }
    private void Update()
    {
        currentScore = Convert.ToInt32(_ScoreText.text);
        if (currentScore - Score >= 300)
        {
            obstacleRowsPerTile += 5;
            Score = currentScore;
        
        if (maxObstacleSpacing > 0.05f)
            maxObstacleSpacing -= 0.05f;
        if (maxObstacleSpacing > 0.01f)
            minObstacleSpacing -= 0.005f;
        }
        
    }
    public void SpawnObstaclesOnTile(GameObject tile, float tileZPosition)
    {
        // Ensure spawner has a valid gap size even if Start() hasn't executed yet
        if (currentGapSize <= 0f)
        {
            currentGapSize = initialGapSize;
            Debug.LogWarning($"ObstacleSpawner: currentGapSize was not initialized. Falling back to initialGapSize={initialGapSize}");
        }

        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("No obstacle prefabs assigned!");
            return;
        }
        // Her tile'da obstacleRowsPerTile kadar sıra obstacle yerleştir
        Debug.Log($"Spawning obstacles on tile at Z={tileZPosition} with {obstacleRowsPerTile} rows. Parent={tile.name}");
        for (int row = 0; row < obstacleRowsPerTile; row++)
        {
            // Hesap: tileZPosition represent the center of the tile. Bölümleri eşit aralıklı yerleştir.
            float segmentLength = GROUND_LENGTH / (float)obstacleRowsPerTile;
            // Her segmentin ortasına yerleştir
            float rowZ = tileZPosition - (GROUND_LENGTH / 2f) + (row + 0.5f) * segmentLength;
            Debug.Log($"  Spawning row {row} at Z={rowZ}");
            SpawnObstacleRow(tile.transform, rowZ);
        }
    }
    
    private void SpawnObstacleRow(Transform parent, float zPosition)
    {
        if (parent == null)
        {
        Debug.LogWarning("SpawnObstacleRow: parent is null, skipping row spawn!");
        return; // parent yoksa spawn etme
        }

        // Arabalar için geçiş yolu bırak (ortada veya sağda/solda)
        float passagePosition = UnityEngine.Random.Range(-GROUND_WIDTH/2 + 2f, GROUND_WIDTH/2 - 2f);
        
        // Sol taraftaki obstacle'lar
        float leftEnd = passagePosition - currentGapSize/2;
        SpawnObstaclesInRange(parent, zPosition, -GROUND_WIDTH/2 + 1f, leftEnd);
        
        // Sağ taraftaki obstacle'lar
        float rightStart = passagePosition + currentGapSize/2;
        SpawnObstaclesInRange(parent, zPosition, rightStart, GROUND_WIDTH/2 - 1f);
    }
    
    private void SpawnObstaclesInRange(Transform parent, float zPosition, float startX, float endX)
    {
        if (parent == null) return; // parent yoksa spawn etme
    if (startX >= endX) return; // geçerli aralık yoksa çık

    // X aralıklarını clamp ile güvenceye al
    float currentX = Mathf.Max(startX, -GROUND_WIDTH / 2 + 0.5f);
    endX = Mathf.Min(endX, GROUND_WIDTH / 2 - 0.5f);

    while (currentX < endX)
    {
        int obstacleIndex = UnityEngine.Random.Range(0, obstaclePrefabs.Length);
        GameObject selectedPrefab = obstaclePrefabs[obstacleIndex];

        float obstacleWidth = GetObstacleWidth(selectedPrefab);

        if (currentX + obstacleWidth <= endX)
        {
            float spawnY = parent.position.y + obstacleY;
            Vector3 spawnPosition = new Vector3(currentX + obstacleWidth / 2, spawnY, zPosition);

            GameObject inst = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            if (inst != null)
            {
                inst.transform.SetParent(parent, true);

                Rigidbody rb = inst.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                foreach (var crb in inst.GetComponentsInChildren<Rigidbody>())
                {
                    crb.isKinematic = true;
                    crb.useGravity = false;
                }
            }

            float spacing = UnityEngine.Random.Range(minObstacleSpacing, maxObstacleSpacing);
            currentX += obstacleWidth + spacing;
        }
        else
        {
            break;
        }
    }
    }
    
    private float GetObstacleWidth(GameObject obstaclePrefab)
    {
        // Öncelikle prefab'ın mesh/bounds'undan ölçmeye çalış
        float measured = MeasurePrefabWidth(obstaclePrefab);
        if (measured > 0f)
            return measured;

        // Fallback isim tabanlı heuristik
        if (obstaclePrefab.name.Contains("Small"))
            return 1f; // obstacleSmall scale x = 1
        else
            return 3f; // obstacle scale x = 3
    }

    private float MeasurePrefabWidth(GameObject prefab)
    {
        if (prefab == null) return -1f;

        // Try MeshFilter.sharedMesh first (editor/runtime-safe), then Renderer.bounds
        var mf = prefab.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            var size = mf.sharedMesh.bounds.size;
            // Multiply by prefab's local scale.x to estimate world width
            return Mathf.Abs(size.x * prefab.transform.localScale.x);
        }

        var rend = prefab.GetComponent<Renderer>();
        if (rend != null)
        {
            // Renderer.bounds is in world-space if prefab were instantiated; use bounds.size.x as best-effort
            return Mathf.Abs(rend.bounds.size.x);
        }

        return -1f;
    }
}