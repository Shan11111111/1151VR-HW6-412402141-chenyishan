
using UnityEngine;

public class BasketController : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;

    [Header("Movement")]
    public float moveSpeed = 12f;
    public float minX = -2.4f;
    public float maxX = 2.4f;
    public float minZ = -2.4f;
    public float maxZ = 2.4f;

    [Header("Game Manager")]
    public Director director;

    private Rigidbody rb;
    private Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        targetPosition = transform.position;
    }

    void Update()
    {
        if (mainCamera == null)
        {
            return;
        }

        if (director == null || !director.CanPlay)
        {
            return;
        }

        Vector2 screenPosition;

        if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0))
        {
            screenPosition = Input.mousePosition;
        }
        else
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        Plane groundPlane = new Plane(
            Vector3.up,
            new Vector3(0, transform.position.y, 0)
        );

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            float x = Mathf.Clamp(point.x, minX, maxX);
            float z = Mathf.Clamp(point.z, minZ, maxZ);

            targetPosition = new Vector3(
                x,
                transform.position.y,
                z
            );
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (director == null || !director.CanPlay)
        {
            return;
        }

        Vector3 nextPosition = Vector3.MoveTowards(
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
    }

    void OnTriggerEnter(Collider other)
    {
        if (director == null || !director.CanPlay)
        {
            return;
        }

        if (other.CompareTag("Apple") ||
            other.CompareTag("Bomb"))
        {
            FallingItem item =
                other.GetComponent<FallingItem>();

            if (item == null)
            {
                Debug.LogWarning(
                    "接取物品缺少 FallingItem！"
                );
                return;
            }

            // 避免重複接取
            if (!item.enabled)
            {
                return;
            }

            item.enabled = false;

            // 更新分數及播放接取音效
            director.CatchItem(other.tag);

            // 播放蘋果碎裂或炸彈爆炸效果
            item.BreakItem(other.transform.position);
        }
    }
}
