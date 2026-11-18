using System.Runtime.InteropServices;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// Определяет тип устройства, на котором запущена игра.
    /// Нужен, чтобы автоматически выбирать управление: сенсорное (телефон/планшет)
    /// или клавиатура (ПК).
    /// </summary>
    public static class DeviceDetector
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int SS_IsTouchPrimaryDevice();
#endif

        /// <summary>
        /// true, если основное устройство ввода — сенсорный экран.
        /// </summary>
        public static bool IsTouchPrimaryDevice()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                return SS_IsTouchPrimaryDevice() == 1;
            }
            catch (System.Exception)
            {
                // Плагин недоступен — используем общую проверку ниже.
            }
#endif
            return Application.isMobilePlatform;
        }
    }
}
