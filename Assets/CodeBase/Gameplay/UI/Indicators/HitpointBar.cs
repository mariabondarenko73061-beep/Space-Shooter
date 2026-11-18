using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class HitpointBar : MonoBehaviour
    {
        [SerializeField] private Image m_Image;

        private float lastHitpoints;

        private void Update()
        {
            if (m_Image == null) return;
            if (Player.Instance == null) return;
            if (Player.Instance.ActiveShip == null) return;
            if (Player.Instance.ActiveShip.MaxHitPoints <= 0) return;

            float hitpoints = ((float)Player.Instance.ActiveShip.HitPoints / (float)Player.Instance.ActiveShip.MaxHitPoints);

            if(hitpoints != lastHitpoints)
            {
                m_Image.fillAmount = hitpoints;
                lastHitpoints = hitpoints;
            }
        }
    }
}
