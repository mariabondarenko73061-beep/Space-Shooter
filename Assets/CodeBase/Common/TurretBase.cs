using UnityEngine;

namespace Common
{
    /// <summary>
    /// Базовая логика турели: общий таймер перезарядки и шаблон выстрела.
    /// Конкретная реализация сама решает, чем и как стрелять.
    /// </summary>
    public abstract class TurretBase : MonoBehaviour
    {
        [SerializeField] private float m_ProjectileSpawnOffset = 0.35f;
        protected float ProjectileSpawnOffset => m_ProjectileSpawnOffset;

        private float m_RefireTimer;

        public bool CanFire => m_RefireTimer <= 0f;

        protected virtual void Update()
        {
            if (m_RefireTimer > 0f)
                m_RefireTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Пытается выстрелить, если перезарядка завершена.
        /// </summary>
        public void Fire()
        {
            if (!CanFire) return;

            OnFire();
        }

        /// <summary>
        /// Игровая логика выстрела. Наследник создаёт снаряд и вызывает SetRefireTimer.
        /// </summary>
        protected abstract void OnFire();

        /// <summary>
        /// Запускает перезарядку на указанное время. Значение 0 сбрасывает таймер.
        /// </summary>
        protected void SetRefireTimer(float duration)
        {
            m_RefireTimer = Mathf.Max(0f, duration);
        }
    }
}
