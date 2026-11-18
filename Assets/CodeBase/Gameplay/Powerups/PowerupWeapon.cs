using UnityEngine;

namespace SpaceShooter
{
    public class PowerupWeapon : Powerup
    {
        [SerializeField] private TurretProperties m_Properites;
        [SerializeField] private int m_AmmoValue = 5;

        protected override void OnPickedUp(SpaceShip ship)
        {
            if (ship == null) return;
            if (m_Properites == null) return;

            ship.AssignWeapon(m_Properites);
            ship.AddAmmo(m_AmmoValue);
        }
    }
}