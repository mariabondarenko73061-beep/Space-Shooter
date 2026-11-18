using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public abstract class ProjectileBase : Entity
    {
        [SerializeField] private float m_Velosity;
        [SerializeField] private float m_Lifetime;
        [SerializeField] private int m_Damage;

        [Header("Explosion")]
        [SerializeField] private GameObject m_ExplosionEffectPrefab;
        [SerializeField] private float m_ExplosionRadius;
        [SerializeField, Range(0f, 1f)] private float m_ExplosionDamageFalloff = 0.4f;

        protected virtual void OnHit(DestructibleBase destructible) { }
        protected virtual void OnHit(Collider2D collider2D) { }

        protected DestructibleBase m_Parent;

        protected int Damage => m_Damage;
        protected DestructibleBase ParentShooter => m_Parent;
        private float m_Timer;

        protected virtual void Update()
        {
            float stepLenght = Time.deltaTime * m_Velosity;
            Vector2 step = transform.up * stepLenght;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, stepLenght);

            if (hit)
            {
                OnHit(hit.collider);

                DestructibleBase dest = hit.collider.transform.root.GetComponent<DestructibleBase>();

                // Сообщаем о попадании только по «живой» цели. Если объект уже
                // уничтожен, неуязвим или бессмертен, урон не применяется и
                // событие попадания не рассылается — иначе очки и убийства
                // начислялись бы повторно.
                if (dest != null && dest != m_Parent && dest.CanBeTargeted)
                {
                    dest.ApplyDamage(m_Damage);

                    OnHit(dest);
                }

                OnProjectileLifeEnd(hit.collider, hit.point);
                return;
            }

            transform.position += new Vector3(step.x, step.y, 0);

            m_Timer += Time.deltaTime;

            if (m_Timer > m_Lifetime)
            {
                OnProjectileLifetimeEnd();
                return;
            }
        }

        protected virtual void OnProjectileLifeEnd(Collider2D col, Vector2 pos)
        {
            DestructibleBase directTarget = col != null ? col.transform.root.GetComponent<DestructibleBase>() : null;

            ApplyAreaDamage(pos, directTarget);

            SpawnExplosionEffect(pos);

            Destroy(gameObject);
        }

        protected virtual void OnProjectileLifetimeEnd()
        {
            SpawnExplosionEffect(transform.position);

            Destroy(gameObject);
        }

        /// <summary>
        /// Наносит урон всем разрушаемым объектам в радиусе взрыва.
        /// Цель под прямым попаданием исключается: ей урон уже был нанесён.
        /// </summary>
        private void ApplyAreaDamage(Vector2 position, DestructibleBase directTarget)
        {
            if (m_ExplosionRadius <= 0f) return;

            Collider2D[] overlaps = Physics2D.OverlapCircleAll(position, m_ExplosionRadius);
            HashSet<DestructibleBase> damagedTargets = new HashSet<DestructibleBase>();

            for (int i = 0; i < overlaps.Length; i++)
            {
                DestructibleBase target = overlaps[i].transform.root.GetComponent<DestructibleBase>();

                if (target == null || target == m_Parent || target == directTarget) continue;
                if (target.IsDead) continue;
                if (damagedTargets.Contains(target)) continue;

                damagedTargets.Add(target);

                float distance = Vector2.Distance(position, target.transform.position);
                float distanceRatio = Mathf.Clamp01(distance / m_ExplosionRadius);
                float damageMultiplier = Mathf.Lerp(1f, m_ExplosionDamageFalloff, distanceRatio);
                int damage = Mathf.Max(1, Mathf.RoundToInt(m_Damage * damageMultiplier));

                target.ApplyDamage(damage);
            }
        }

        private void SpawnExplosionEffect(Vector2 position)
        {
            if (m_ExplosionEffectPrefab == null) return;

            Instantiate(m_ExplosionEffectPrefab, position, Quaternion.identity);
        }

        public void SetParentShooter(DestructibleBase parent)
        {
            m_Parent = parent;
        }
    }
}
