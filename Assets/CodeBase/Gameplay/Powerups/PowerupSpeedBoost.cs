using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Поверап временного ускорения корабля игрока.
    /// </summary>
    public class PowerupSpeedBoost : Powerup
    {
        [SerializeField] private float m_Multiplier = 1.8f;
        [SerializeField] private float m_Duration = 8f;

        protected override void OnPickedUp(SpaceShip ship)
        {
            ship.ApplySpeedBoost(m_Multiplier, m_Duration);
        }
    }
}