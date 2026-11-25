
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject applePrefab;
    public GameObject bombPrefab;

    [Header("Spawn Settings")]
    public float interval = 1.2f;
    public float speed = 3f;

    [Range(0f, 1f)]
    public float bombRate = 0.2f;

    [Header("Spawn Area")]
    public float spawnHeight = 6f;
    public float minX = -2.5f;
    public float maxX = 2.5f;
    public float minZ = -2.5f;
    public float maxZ = 2.5f;

    [Header("Difficulty Settings")]
    public bool enableDifficulty = true;
    public float stage2Time = 20f;
    public float stage3Time = 40f;

    private float spawnTimer = 0f;
    private float elapsedTime = 0f;
    private int currentStage = 1;

    void Start()
    {
        if (enableDifficulty)
        {
            ApplyDifficulty(1);
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (enableDifficulty)
        {
            if (elapsedTime >= stage3Time && currentStage != 3)
            {
                ApplyDifficulty(3);
            }
            else if (elapsedTime >= stage2Time &&
                     elapsedTime < stage3Time &&
                     currentStage != 2)
            {
                ApplyDifficulty(2);
            }
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= interval)
        {
            SpawnItem();
            spawnTimer = 0f;
        }
    }

    void ApplyDifficulty(int stage)
    {
        currentStage = stage;

        if (stage == 1)
        {
            interval = 1.2f;
            speed = 3f;
            bombRate = 0.2f;
        }
        else if (stage == 2)
        {
            interval = 0.9f;
            speed = 4.5f;
            bombRate = 0.25f;
        }
        else if (stage == 3)
        {
            interval = 0.6f;
            speed = 6f;
            bombRate = 0.35f;
        }

        Debug.Log(
            "難度階段：" + currentStage +
            "，Interval：" + interval +
            "，Speed：" + speed +
            "，BombRate：" + bombRate
        );
    }

    void SpawnItem()
    {
        if (applePrefab == null || bombPrefab == null)
        {
            Debug.LogWarning("Apple 或 Bomb Prefab 尚未設定！");
            return;
        }

        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        Vector3 spawnPosition = new Vector3(
            x,
            spawnHeight,
            z
        );

        GameObject selectedPrefab;

        if (Random.value < bombRate)
        {
            selectedPrefab = bombPrefab;
        }
        else
        {
            selectedPrefab = applePrefab;
        }

        GameObject item = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FallingItem fallingItem = item.GetComponent<FallingItem>();

        if (fallingItem != null)
        {
            fallingItem.fallSpeed = speed;
        }
    }
}
