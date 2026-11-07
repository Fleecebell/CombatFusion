using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Transform target;
    [SerializeField] private InputReader input;
    [SerializeField] private Transform cam;

    [Header("视角")]
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;

    [Header("距离与偏移")]
    [SerializeField] private float distance = 3.5f;
    [SerializeField] private float minDistance = 0.6f;
    [SerializeField] private float pivotHeight = 1.5f;
    [SerializeField] private float shoulderOffset = 0.5f;
    [SerializeField] private float followSharpness = 12f;
    [SerializeField] private float minHeight = 0.4f;
    [SerializeField] private LayerMask obstructionMask = ~0; // 默认所有层都阻挡

    private float _yaw;
    private float _pitch = 15f;
    private float _currentDistance;

    private void Awake()
    {
        _currentDistance = distance;
    }

    private void LateUpdate()
    {
        if (target == null || cam == null) return;

        UpdateAngles();
        FollowPivot();
        PlaceCamera();
    }

    private void UpdateAngles()
    {
        if (input == null) return;

        Vector2 look = input.Intent.look * sensitivity;
        _yaw += look.x;
        _pitch -= look.y;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    private void FollowPivot()
    {
        transform.position = target.position + Vector3.up * pivotHeight;
    }

    private void PlaceCamera()
    {
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 pivot = transform.position;

        // 理想位置：本地偏移被视角旋转到世界空间
        Vector3 desired = pivot + rot * new Vector3(shoulderOffset, 0f, -distance);
        Vector3 dir = desired - pivot;

        // 防穿墙
        float allowed = distance;
        if (Physics.Linecast(pivot, desired, out RaycastHit hit, obstructionMask, QueryTriggerInteraction.Ignore))
        {
            allowed = Mathf.Clamp(hit.distance - 0.2f, minDistance, distance);
        }

        _currentDistance = Mathf.Lerp(_currentDistance, allowed, followSharpness * Time.deltaTime);

        Vector3 pos = pivot + dir.normalized * _currentDistance;
        pos.y = Mathf.Max(pos.y, minHeight);
        cam.position = pos;
        cam.rotation = rot;
    }
}