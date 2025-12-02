using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float playerMarginFromEdge = 0.5f;

    public float smoothSpeed = 5f;

    public float initialCameraX = -0.27f;

    public float maxX = 100f;

    private float currentMinX;
    private float cameraHalfWidth;

    void Start()
    {
        if (target == null) return;

        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;

        float idealCameraX = target.position.x + cameraHalfWidth - playerMarginFromEdge;

        float finalInitialX = Mathf.Clamp(idealCameraX, initialCameraX, maxX);

        transform.position = new Vector3(finalInitialX, transform.position.y, transform.position.z);

        currentMinX = transform.position.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float desiredFollowX = target.position.x + cameraHalfWidth - playerMarginFromEdge;

        float desiredX = Mathf.Clamp(desiredFollowX, initialCameraX, maxX);

        Vector3 desiredPosition = new Vector3(desiredX, transform.position.y, transform.position.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }

    public float GetMinPlayerX()
    {
        return transform.position.x - cameraHalfWidth + playerMarginFromEdge;
    }
}