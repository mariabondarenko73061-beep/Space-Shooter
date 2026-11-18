using Common;
using UnityEngine;

namespace SpaceShooter
{
    public class Projectile : ProjectileBase
    {
        [SerializeField] private ImpactEffect m_ImpactEffectPrefab;

        protected override void OnHit(DestructibleBase destructible)
        {
            if (destructible == null) return;
            if (Player.Instance == null || Player.Instance.ActiveShip == null) return;
            if (m_Parent != Player.Instance.ActiveShip) return;

            // Очки и убийства начисляются только за уничтожение цели,
            // а не за каждое попадание по ней.
            if (destructible.IsDead == false) return;

            if (destructible is Destructible target)
                Player.Instance.AddScore(target.ScoreValue);

            if (destructible is SpaceShip)
                Player.Instance.Addkill();
        }

        protected override void OnProjectileLifeEnd(Collider2D col, Vector2 pos)
        {
            if (m_ImpactEffectPrefab != null)
                Instantiate(m_ImpactEffectPrefab, pos, Quaternion.identity);

            base.OnProjectileLifeEnd(col, pos);
        }
    }
}
