using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class GravityWell : MonoBehaviour
    {
        [SerializeField] private float m_Force;
        [SerializeField] private float m_Radius;

        /// <summary>
        /// Сила притяжения. 0 (или меньше) — планета ничего не притягивает.
        /// </summary>
        public float Force => m_Force;

        /// <summary>
        /// Радиус действия притяжения.
        /// </summary>
        public float Radius => m_Radius;

        [Header("Unique level mechanic")]
        [SerializeField] private bool m_ChangeGravityDirection;
        [SerializeField] private float m_GravityChangeInterval = 3.0f;

        private float m_GravityChangeTimer;
        private bool m_GravityReversed;

        /// <summary>
        /// Настраивает планету при процедурной генерации уровня. Радиус задаётся
        /// в мировых единицах: коллайдер пересчитывается с учётом масштаба, чтобы
        /// зона притяжения совпадала с силой, которую чувствует корабль.
        /// </summary>
        public void Configure(float force, float radius, bool changeDirection, float changeInterval)
        {
            m_Force = force;
            m_Radius = Mathf.Max(0f, radius);
            m_ChangeGravityDirection = changeDirection;
            m_GravityChangeInterval = Mathf.Max(0f, changeInterval);

            CircleCollider2D trigger = GetComponent<CircleCollider2D>();

            if (trigger != null)
            {
                float scale = Mathf.Max(0.0001f, Mathf.Abs(transform.lossyScale.x));
                trigger.radius = m_Radius / scale;
            }
        }

        private void Update()
        {
            if (!m_ChangeGravityDirection) return;
            if (m_GravityChangeInterval <= 0) return;

            m_GravityChangeTimer += Time.deltaTime;

            if (m_GravityChangeTimer >= m_GravityChangeInterval)
            {
                m_GravityChangeTimer = 0;
                m_GravityReversed = !m_GravityReversed;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.attachedRigidbody == null) return;
            if (m_Radius <= 0f) return;

            Vector2 dir = transform.position - collision.transform.position;

            float dist = dir.magnitude;

            if (dist < m_Radius && dist > 0f)
            {
                float direction = m_GravityReversed ? -1.0f : 1.0f;
                Vector2 force = dir.normalized * m_Force * (dist / m_Radius) * direction;
                collision.attachedRigidbody.AddForce(force, ForceMode2D.Force);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetComponent<CircleCollider2D>().radius = m_Radius;
        }
#endif
    }
}