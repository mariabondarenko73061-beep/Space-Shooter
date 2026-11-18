using UnityEngine;
using Common;

namespace SpaceShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceShip : Destructible
    {
        [SerializeField] private Sprite m_PreviewImage;

        /// <summary>
        /// Масса для автоматической установки у ригида.
        /// </summary>
        [Header("Space ship")]
        [SerializeField] private float m_Mass;

        /// <summary>
        /// Толкающая вперед сила.
        /// </summary>
        [SerializeField] private float m_Thrust;

        /// <summary>
        /// Вращающая сила.
        /// </summary>
        [SerializeField] private float m_Mobility;

        /// <summary>
        /// Максимальная линейная скорость.
        /// </summary>
        [SerializeField] private float m_MaxLinearVelocity;

        /// <summary>
        /// Максимальная вращательная скорость. В градусах/сек.
        /// </summary>
        [SerializeField] private float m_MaxAngularVelocity;

        [Header("Explosion")]
        [SerializeField] private GameObject m_ExplosionPrefab;

        [Header("Powerup visuals")]
        [SerializeField] private Color m_InvulnerabilityColor = new Color(0.55f, 0.9f, 1f, 1f);
        [SerializeField] private Color m_SpeedBoostColor = new Color(1f, 0.85f, 0.3f, 1f);
        [SerializeField] private float m_InvulnerabilityBlinkInterval = 0.1f;

        /// <summary>
        /// Сохраненная ссылка на ригид.
        /// </summary>
        private Rigidbody2D m_Rigid;

        public Sprite PreviewImage => m_PreviewImage;
        private SpriteRenderer[] m_ShipRenderers;
        private Color[] m_BaseColors;

        public float MaxLinearVelocity => m_MaxLinearVelocity;
        public float MaxAngularVelocity => m_MaxAngularVelocity;
        private float m_InvulnerabilityTimer;
        private float m_SpeedBoostTimer;
        private float m_SpeedBoostMultiplier = 1f;
        private float m_BlinkTimer;
        private bool m_BlinkVisible = true;

        public bool IsSpeedBoosted => m_SpeedBoostTimer > 0f;

        #region Public API

        /// <summary>
        /// Управление линейной тягой. -1.0 до +1.0
        /// </summary>
        public float ThrustControl { get; set; }

        /// <summary>
        /// Управление вращательной тягой. -1.0 до +1.0
        /// </summary>
        public float TorqueControl { get; set; }

        /// <summary>
        /// Включает временную неуязвимость корабля.
        /// </summary>
        public void ApplyInvulnerability(float duration)
        {
            if (duration <= 0f) return;

            m_InvulnerabilityTimer = Mathf.Max(m_InvulnerabilityTimer, duration);
            m_BlinkTimer = 0f;
            SetInvulnerable(true);
        }

        /// <summary>
        /// Включает временное ускорение корабля.
        /// </summary>
        public void ApplySpeedBoost(float multiplier, float duration)
        {
            if (multiplier <= 0f || duration <= 0f) return;

            m_SpeedBoostMultiplier = Mathf.Max(1f, multiplier);
            m_SpeedBoostTimer = Mathf.Max(m_SpeedBoostTimer, duration);
        }

        #endregion

        #region Unity Event

        protected override void Start()
        {
            base.Start();

            m_Rigid = GetComponent<Rigidbody2D>();
            m_Rigid.mass = m_Mass;

            m_Rigid.inertia = 1;

            CacheShipRenderers();
            InitOffensive();
        }

        private void Update()
        {
            UpdatePowerupTimers(Time.deltaTime);
            UpdateShipVisuals();
        }

        private void FixedUpdate()
        {
            UpdateRigidBody();

            UpdateEnergyRegen();
        }

        /// <summary>
        /// Метод добавления сил кораблю для движения
        /// </summary>
        private void UpdateRigidBody()
        {
            if (m_Rigid == null) return;

            m_Rigid.AddForce(ThrustControl * m_Thrust * m_SpeedBoostMultiplier * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);

            if (m_MaxLinearVelocity > 0f)
                m_Rigid.AddForce(-m_Rigid.velocity * (m_Thrust / m_MaxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

            m_Rigid.AddTorque(TorqueControl * m_Mobility * Time.fixedDeltaTime, ForceMode2D.Force);

            if (m_MaxAngularVelocity > 0f)
                m_Rigid.AddTorque(-m_Rigid.angularVelocity * (m_Mobility / m_MaxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        #endregion

        #region Powerups

        private void CacheShipRenderers()
        {
            m_ShipRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_BaseColors = new Color[m_ShipRenderers.Length];

            for (int i = 0; i < m_ShipRenderers.Length; i++)
            {
                m_BaseColors[i] = m_ShipRenderers[i].color;
            }
        }

        private void UpdatePowerupTimers(float deltaTime)
        {
            if (m_InvulnerabilityTimer > 0f)
            {
                m_InvulnerabilityTimer -= deltaTime;

                if (m_InvulnerabilityTimer <= 0f)
                {
                    m_InvulnerabilityTimer = 0f;
                    SetInvulnerable(false);
                }
            }

            if (m_SpeedBoostTimer > 0f)
            {
                m_SpeedBoostTimer -= deltaTime;

                if (m_SpeedBoostTimer <= 0f)
                {
                    m_SpeedBoostTimer = 0f;
                    m_SpeedBoostMultiplier = 1f;
                }
            }
        }

        private void UpdateShipVisuals()
        {
            if (m_ShipRenderers == null) return;

            bool invulnerable = m_InvulnerabilityTimer > 0f;
            bool boosted = m_SpeedBoostTimer > 0f;

            if (invulnerable)
            {
                m_BlinkTimer -= Time.deltaTime;

                if (m_BlinkTimer <= 0f)
                {
                    m_BlinkTimer = m_InvulnerabilityBlinkInterval;
                    m_BlinkVisible = !m_BlinkVisible;
                }
            }
            else
            {
                m_BlinkTimer = 0f;
                m_BlinkVisible = true;
            }

            for (int i = 0; i < m_ShipRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = m_ShipRenderers[i];

                if (spriteRenderer == null) continue;

                Color tint = m_BaseColors[i];

                if (invulnerable)
                    tint = m_InvulnerabilityColor;

                if (boosted)
                    tint = Color.Lerp(tint, m_SpeedBoostColor, 0.6f);

                spriteRenderer.color = tint;
                spriteRenderer.enabled = m_BlinkVisible;
            }
        }

        #endregion

        #region Weapons

        [SerializeField] private Turret[] m_Turrets;

        public void Fire(TurretMode mode)
        {
            if (IsDead) return;
            if (m_Turrets == null) return;

            for (int i = 0; i < m_Turrets.Length; i++)
            {
                if (m_Turrets[i] != null && m_Turrets[i].Mode == mode)
                {
                    m_Turrets[i].Fire();
                }
            }
        }

        [SerializeField] private int m_MaxEnergy;
        [SerializeField] private int m_MaxAmmo;
        [SerializeField] private int m_EnegyRegenPerSecond;

        private float m_PrimaryEnergy;
        private int m_SecondaryAmmo;

        public void AddEnergy(int e)
        {
            m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy + e, 0, m_MaxEnergy);
        }

        public void AddAmmo(int ammo)
        {
            m_SecondaryAmmo = Mathf.Clamp(m_SecondaryAmmo + ammo, 0, m_MaxAmmo);
        }

        private void InitOffensive()
        {
            m_PrimaryEnergy = m_MaxEnergy;
            m_SecondaryAmmo = m_MaxAmmo;
        }

        private void UpdateEnergyRegen()
        {
            m_PrimaryEnergy += (float)m_EnegyRegenPerSecond * Time.fixedDeltaTime;
            m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy, 0, m_MaxEnergy);
        }

        public bool DrawAmmo(int count)
        {
            if (count == 0)
                return true;

            if (m_SecondaryAmmo >= count)
            {
                m_SecondaryAmmo -= count;
                return true;
            }

            return false;
        }

        public bool DrawEnergy(int count)
        {
            if (count == 0)
                return true;

            if (m_PrimaryEnergy >= count)
            {
                m_PrimaryEnergy -= count;
                return true;
            }

            return false;
        }

        public void AssignWeapon(TurretProperties props)
        {
            if (m_Turrets == null) return;

            for (int i = 0; i < m_Turrets.Length; i++)
            {
                if (m_Turrets[i] != null)
                    m_Turrets[i].AssignLoadout(props);
            }
        }

        #endregion

        protected override void OnDeath()
        {
            if (m_ExplosionPrefab != null)
            {
                Instantiate(
                    m_ExplosionPrefab,
                    transform.position,
                    transform.rotation
                );
            }
            else
            {
                Debug.LogError("Explosion Prefab не назначен у корабля: " + name);
            }

            base.OnDeath();
        }
    }
}