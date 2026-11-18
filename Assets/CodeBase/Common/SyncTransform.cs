using UnityEngine;

namespace Common
{
    /// <summary>
    /// Держит объект в экранных координатах камеры. Используется фоном (звёзды),
    /// чтобы параллакс-слои всегда оставались перед камерой.
    /// </summary>
    public class SyncTransform : MonoBehaviour
    {
        private const float CameraSearchInterval = 0.5f;

        [SerializeField] private Transform m_Target;

        public Transform Target => m_Target;

        private float m_SearchTimer;

        private void Awake()
        {
            if (m_Target == null)
                m_Target = FindCameraTransform();
        }

        private void Update()
        {
            // Камера может быть создана позже (например, PlayerSpawner в Start),
            // поэтому цель ищем не только в Awake, а периодически до первого успеха.
            if (m_Target == null)
            {
                m_SearchTimer -= Time.unscaledDeltaTime;

                if (m_SearchTimer <= 0f)
                {
                    m_SearchTimer = CameraSearchInterval;
                    m_Target = FindCameraTransform();
                }
            }

            if (m_Target == null) return;

            transform.position = new Vector3(m_Target.position.x, m_Target.position.y, transform.position.z);
        }

        /// <summary>
        /// Явно назначает цель синхронизации. Вызывается спавнером камеры,
        /// чтобы фон начал следовать сразу и не зависел от тега MainCamera.
        /// </summary>
        public void SetTarget(Transform target)
        {
            m_Target = target;
        }

        private static Transform FindCameraTransform()
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }

            return mainCamera != null ? mainCamera.transform : null;
        }
    }
}
