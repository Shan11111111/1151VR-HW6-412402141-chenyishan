
using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("Falling Settings")]
    public float fallSpeed = 3f;
    public float destroyY = -3f;

    [Header("Ground Detection")]
    public float groundY = 0.1f;
    public float groundCheckOffset = 0.25f;

    [Header("Effects")]
    public GameObject appleBurstEffect;
    public GameObject bombExplosionEffect;

    [Header("Ground Audio")]
    public AudioClip appleGroundClip;
    public AudioClip bombGroundClip;

    [Range(0f, 1f)]
    public float groundVolume = 0.7f;

    private Rigidbody rb;
    private Collider itemCollider;
    private Director director;
    private bool isDestroyed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();

        director = FindFirstObjectByType<Director>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;

            rb.collisionDetectionMode =
                CollisionDetectionMode.Continuous;

            rb.linearVelocity = Vector3.down * fallSpeed;
        }
    }

    void FixedUpdate()
    {
        if (isDestroyed)
        {
            return;
        }

        float bottomY = transform.position.y;

        if (itemCollider != null)
        {
            bottomY = itemCollider.bounds.min.y;
        }
        else
        {
            bottomY -= groundCheckOffset;
        }

        // 到達地板高度
        if (bottomY <= groundY)
        {
            Vector3 effectPosition = transform.position;
            effectPosition.y = groundY + 0.1f;

            BreakItem(effectPosition, true);
        }
    }

    void Update()
    {
        // 防止意外穿過場景
        if (!isDestroyed && transform.position.y < destroyY)
        {
            Vector3 effectPosition = transform.position;
            effectPosition.y = groundY + 0.1f;

            BreakItem(effectPosition, true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed)
        {
            return;
        }

        if (collision.gameObject.name.StartsWith("Tile_"))
        {
            Vector3 effectPosition = transform.position;

            if (collision.contactCount > 0)
            {
                effectPosition =
                    collision.GetContact(0).point;
            }

            // 第二個參數 true 代表碰到地板
            BreakItem(effectPosition, true);
        }
    }

    // 被接住或落地時都能呼叫
    public void BreakItem(
        Vector3 position,
        bool hitGround = false
    )
    {
        if (isDestroyed)
        {
            return;
        }

        isDestroyed = true;

        GameObject effectPrefab = null;
        AudioClip groundClip = null;

        if (CompareTag("Apple"))
        {
            effectPrefab = appleBurstEffect;
            groundClip = appleGroundClip;

            Debug.Log("Apple 碎裂！");
        }
        else if (CompareTag("Bomb"))
        {
            effectPrefab = bombExplosionEffect;
            groundClip = bombGroundClip;

            Debug.Log("Bomb 爆炸！");
        }

        // 生成粒子特效
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(
                effectPrefab,
                position,
                Quaternion.identity
            );

            Destroy(effect, 3f);
        }

        // 只有落地時才播放落地音效
        if (hitGround && groundClip != null)
        {
            if (director != null)
            {
                director.PlayGroundSound(
                    groundClip,
                    groundVolume
                );
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        // 立即隱藏物件
        foreach (Renderer renderer in
                 GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        Destroy(gameObject);
    }
}
