using UnityEngine;

namespace Common
{
    /// <summary>
    /// Базовая логика разрушаемого объекта: запас прочности, получение урона,
    /// неуязвимость и момент уничтожения. Не зависит от правил конкретной игры.
    /// </summary>
    public abstract class DestructibleBase : Entity
    {
        #region Properties

        /// <summary>
        /// Показывает, можно ли вообще наносить урон этому объекту.
        /// </summary>
        [SerializeField] private bool m_Indestructible;
        public bool IsIndestructible => m_Indestructible;

        /// <summary>
        /// Максимальный запас прочности.
        /// </summary>
        [SerializeField] private int m_HitPoints;
        public int MaxHitPoints => m_HitPoints;

        /// <summary>
        /// Текущий запас прочности.
        /// </summary>
        private int m_CurrentHitPoints;
        public int HitPoints => m_CurrentHitPoints;

        /// <summary>
        /// Временная неуязвимость (например, после подбора бонуса).
        /// </summary>
        private bool m_Invulnerable;
        public bool IsInvulnerable => m_Invulnerable;

        private bool m_IsDeath;
        public bool IsDead => m_IsDeath;

        /// <summary>
        /// Может ли объект быть выбран целью: он жив, не неуязвим и не бессмертен.
        /// </summary>
        public bool CanBeTargeted => !m_IsDeath && !m_Indestructible && !m_Invulnerable;

        #endregion

        #region Unity Events

        protected virtual void Start()
        {
            m_CurrentHitPoints = m_HitPoints;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Наносит объекту урон.
        /// </summary>
        /// <param name="damage">Количество единиц урона.</param>
        public void ApplyDamage(int damage)
        {
            if (m_Indestructible || m_Invulnerable || m_IsDeath) return;
            if (damage <= 0) return;

            m_CurrentHitPoints -= damage;

            if (m_CurrentHitPoints <= 0)
            {
                m_IsDeath = true;
                OnDeath();
            }
        }

        /// <summary>
        /// Включает или выключает временную неуязвимость.
        /// </summary>
        public void SetInvulnerable(bool value)
        {
            if (m_IsDeath) return;

            m_Invulnerable = value;
        }

        /// <summary>
        /// Устанавливает новый максимум прочности и восстанавливает её.
        /// </summary>
        public void SetMaxHitPoints(int hitPoints)
        {
            m_HitPoints = Mathf.Max(1, hitPoints);
            m_CurrentHitPoints = m_HitPoints;
        }

        #endregion

        /// <summary>
        /// Вызывается в момент уничтожения объекта. Наследник может добавить
        /// собственную реакцию и вызвать base.OnDeath() для удаления объекта.
        /// </summary>
        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}
