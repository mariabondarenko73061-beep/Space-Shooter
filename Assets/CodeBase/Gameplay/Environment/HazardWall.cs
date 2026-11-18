using UnityEngine;
using Common;

namespace SpaceShooter
{
    /// <summary>
    /// Стенка коридора. При столкновении уничтожает корабль игрока.
    /// </summary>
    public class HazardWall : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            var destructible = collision.transform.root.GetComponent<Destructible>();

            if (destructible != null)
                destructible.ApplyDamage(destructible.HitPoints);
        }
    }
}