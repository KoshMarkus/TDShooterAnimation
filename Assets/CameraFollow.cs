using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform target;

    [SerializeField] float smoothSpeed;
    [SerializeField] Vector3 offset;

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

}
