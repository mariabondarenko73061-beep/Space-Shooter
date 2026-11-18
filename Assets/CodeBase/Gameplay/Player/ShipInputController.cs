using UnityEngine;
using Common;

namespace SpaceShooter
{
    public class ShipInputController : MonoBehaviour
    {
        public enum ControlMode
        {
            Keyboard,
            Mobile
        }

        [SerializeField] private ControlMode m_ControlMode;
        [SerializeField] private bool m_DetectDeviceAutomatically = true;
        [SerializeField] private VirtualJoystick m_MobileJoystick;
        [SerializeField] private PointerClickHold m_MobileFirePrimary;
        [SerializeField] private PointerClickHold m_MobileFireSecondary;

        private SpaceShip m_TargetShip;

        public void SetTargetShip(SpaceShip ship) => m_TargetShip = ship;

        public void Construct(VirtualGamePad virtualGamePad)
        {
            if (virtualGamePad == null)
                return;

            if (virtualGamePad.VirtualJoystick != null)
                m_MobileJoystick = virtualGamePad.VirtualJoystick;

            if (virtualGamePad.MobileFirePrimary != null)
                m_MobileFirePrimary = virtualGamePad.MobileFirePrimary;

            if (virtualGamePad.MobileFireSecondary != null)
                m_MobileFireSecondary = virtualGamePad.MobileFireSecondary;

            ApplyControlVisibility();
        }

        private void Start()
        {
            // При запуске выбираем управление по устройству: сенсорный экран
            // (телефон/планшет) или клавиатура (ПК).
            if (m_DetectDeviceAutomatically)
                DetectControlMode();

            // Serialized mobile references may be missing when the game pad is
            // spawned at runtime. Resolve them automatically in that case.
            if (m_MobileJoystick == null || m_MobileFirePrimary == null || m_MobileFireSecondary == null)
            {
                VirtualGamePad virtualGamePad = FindObjectOfType<VirtualGamePad>();

                if (virtualGamePad != null)
                    Construct(virtualGamePad);
            }

            ApplyControlVisibility();
        }

        private void DetectControlMode()
        {
            SetControlMode(DeviceDetector.IsTouchPrimaryDevice() ? ControlMode.Mobile : ControlMode.Keyboard);
        }

        private void RefreshControlMode()
        {
            // Касание экрана переключает на мобильное управление.
            if (Input.touchCount > 0)
            {
                SetControlMode(ControlMode.Mobile);
                return;
            }

            // Нажатие игровой клавиши переключает на управление с клавиатуры.
            if (IsGameplayKeyDown())
                SetControlMode(ControlMode.Keyboard);
        }

        private static bool IsGameplayKeyDown()
        {
            return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)
                || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)
                || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)
                || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X);
        }

        private void SetControlMode(ControlMode mode)
        {
            if (m_ControlMode == mode)
                return;

            m_ControlMode = mode;
            ApplyControlVisibility();
        }

        private void ApplyControlVisibility()
        {
            bool isMobile = m_ControlMode == ControlMode.Mobile;

            SetActiveSafe(m_MobileJoystick != null ? m_MobileJoystick.gameObject : null, isMobile);
            SetActiveSafe(m_MobileFirePrimary != null ? m_MobileFirePrimary.gameObject : null, isMobile);
            SetActiveSafe(m_MobileFireSecondary != null ? m_MobileFireSecondary.gameObject : null, isMobile);
        }

        private static void SetActiveSafe(GameObject target, bool active)
        {
            if (target != null)
                target.SetActive(active);
        }

        private void Update()
        {
            if (m_DetectDeviceAutomatically)
                RefreshControlMode();

            if (m_TargetShip == null) return;
            if (m_TargetShip.IsDead) return;

            if (m_ControlMode == ControlMode.Keyboard)
                ControlKeyboard();

            if (m_ControlMode == ControlMode.Mobile)
                ControlMobile();
        }

        private void ControlMobile()
        {
            if (m_MobileJoystick != null)
            {
                var dir = m_MobileJoystick.Value;

                m_TargetShip.ThrustControl = dir.y;
                m_TargetShip.TorqueControl = -dir.x;
            }

            if (m_MobileFirePrimary != null && m_MobileFirePrimary.IsHold)
                m_TargetShip.Fire(TurretMode.Primary);

            if (m_MobileFireSecondary != null && m_MobileFireSecondary.IsHold)
                m_TargetShip.Fire(TurretMode.Secondary);
        }

        private void ControlKeyboard()
        {
            float thrust = 0;
            float torque = 0;

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                thrust = 1.0f;

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                thrust = -1.0f;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                torque = 1.0f;

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                torque = -1.0f;

            if (Input.GetKey(KeyCode.Space))
                m_TargetShip.Fire(TurretMode.Primary);

            if (Input.GetKey(KeyCode.X))
                m_TargetShip.Fire(TurretMode.Secondary);

            m_TargetShip.ThrustControl = thrust;
            m_TargetShip.TorqueControl = torque;
        }
    }
}
