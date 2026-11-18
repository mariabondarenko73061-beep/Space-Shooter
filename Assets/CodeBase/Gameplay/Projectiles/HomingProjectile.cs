using UnityEngine;
using Common;

namespace SpaceShooter
{
    /// <summary>
    /// Самонаводящийся снаряд. Ищет ближайшую цель в радиусе и плавно
    /// поворачивает корпус в ее сторону, пока летит вперед.
    /// </summary>
    public class HomingProjectile : Projectile
    {
        [Header("Homing")]
        [SerializeField] private float m_TurnRate = 220f;
        [SerializeField] private float m_SeekRadius = 40f;
        [SerializeField] private float m_TargetReacquireInterval = 0.2f;

        private Destructible m_Target;
        private float m_ReacquireTimer;

        protected override void Update()
        {
            UpdateTargeting(Time.deltaTime);

            base.Update();
        }

        private void UpdateTargeting(float deltaTime)
        {
            m_ReacquireTimer -= deltaTime;

            bool needNewTarget = m_Target == null || m_Target.IsDead || !m_Target.CanBeTargeted;

            if (needNewTarget && m_ReacquireTimer <= 0f)
            {
                m_Target = FindTarget();
                m_ReacquireTimer = m_TargetReacquireInterval;
            }

            if (m_Target == null) return;

            Vector2 direction = (Vector2)m_Target.transform.position - (Vector2)transform.position;

            if (direction.sqrMagnitude < 0.0001f) return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                m_TurnRate * deltaTime);
        }

        private Destructible FindTarget()
        {
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, m_SeekRadius);

            Destructible bestTarget = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < overlaps.Length; i++)
            {
                Destructible candidate = overlaps[i].transform.root.GetComponent<Destructible>();

                if (candidate == null || candidate == ParentShooter) continue;
                if (!candidate.CanBeTargeted) continue;

                float distance = ((Vector2)candidate.transform.position - (Vector2)transform.position).sqrMagnitude;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = candidate;
                }
            }

            return bestTarget;
        }
    }
}