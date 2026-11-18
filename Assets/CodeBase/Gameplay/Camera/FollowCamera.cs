using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float m_InterpolationLinear;
    [SerializeField] private float m_InterpolationAngular;
    [SerializeField] private float m_CameraZoffset;
    [SerializeField] private float m_ForwardOffset;

    private void Awake()
    {
        if (m_Camera == null)
            m_Camera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        if (m_Target == null) return;

        // The script can live either on the camera itself or on a separate
        // "CameraRig" object that references the camera.
        Transform cameraTransform = m_Camera != null ? m_Camera.transform : transform;

        Vector2 camPos = cameraTransform.position;
        Vector2 targetPos = m_Target.position + m_Target.up * m_ForwardOffset;

        float linearT = Mathf.Clamp01(m_InterpolationLinear * Time.fixedDeltaTime);
        Vector2 newCamPos = Vector2.Lerp(camPos, targetPos, linearT);

        cameraTransform.position = new Vector3(newCamPos.x, newCamPos.y, m_CameraZoffset);

        if (m_InterpolationAngular > 0f)
        {
            float angularT = Mathf.Clamp01(m_InterpolationAngular * Time.fixedDeltaTime);

            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation,
                                                        m_Target.rotation,
                                                        angularT);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        m_Target = newTarget;
    }
}
