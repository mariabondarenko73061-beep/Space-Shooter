using Common;
using UnityEngine;

namespace SpaceShooter
{
    public class Turret : TurretBase
    {
        [SerializeField] private TurretMode m_Mode;
        public TurretMode Mode => m_Mode;

        [SerializeField] private TurretProperties m_TurretProperties;

        private SpaceShip m_Ship;
        private AudioSource m_LaunchAudioSource;

        private void Awake()
        {
            m_LaunchAudioSource = GetComponent<AudioSource>();

            if (m_LaunchAudioSource == null)
                m_LaunchAudioSource = gameObject.AddComponent<AudioSource>();

            m_LaunchAudioSource.playOnAwake = false;
            m_LaunchAudioSource.spatialBlend = 1f;
            m_LaunchAudioSource.volume = 0.8f;
        }

        private void Start()
        {
            // Ищем корабль-владелец среди родителей, а не только в корне:
            // так турель продолжает работать, даже если корабль окажется
            // вложенным в контейнер сгенерированного контента.
            m_Ship = GetComponentInParent<SpaceShip>();
        }

        protected override void OnFire()
        {
            if (m_TurretProperties == null) return;
            if (m_TurretProperties.ProjectilePrefab == null) return;
            if (m_Ship == null) return;
            if (m_Ship.IsDead) return;

            if (!m_Ship.DrawEnergy(m_TurretProperties.EnergyUsage))
                return;

            if (!m_Ship.DrawAmmo(m_TurretProperties.AmmoUsage))
                return;

            Projectile projectile = Instantiate(m_TurretProperties.ProjectilePrefab).GetComponent<Projectile>();

            if (projectile == null) return;

            projectile.transform.position = transform.position + transform.up * ProjectileSpawnOffset;
            projectile.transform.up = transform.up;

            projectile.SetParentShooter(m_Ship);

            SetRefireTimer(m_TurretProperties.RateOfFire);

            if (m_LaunchAudioSource != null && m_TurretProperties.LaunchSFX != null)
                m_LaunchAudioSource.PlayOneShot(m_TurretProperties.LaunchSFX);
        }

        public void AssignLoadout(TurretProperties props)
        {
            if (props == null) return;
            if (m_Mode != props.Mode) return;

            SetRefireTimer(0f);
            m_TurretProperties = props;
        }
    }
}
