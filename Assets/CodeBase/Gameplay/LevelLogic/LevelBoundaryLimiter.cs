using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Ограничитель позиции. Раблотает в связке со скриптом LevelBoundary если таковой имеется на сцене.
    /// Кидается на обьект который надо ограничить.
    /// </summary>
    public class LevelBoundaryLimiter : MonoBehaviour
    {
        private void Update()
        {
            if (LevelBoundary.Instance == null) return;

            var Ib = LevelBoundary.Instance;
            var r = Ib.Radius;

            if (r <= 0f) return;

            Vector2 center = Ib.transform.position;
            Vector2 offset = (Vector2)transform.position - center;

            if (offset.sqrMagnitude > r * r)
            {
                Vector2 direction = offset.sqrMagnitude > 0f ? offset.normalized : Vector2.up;
                Vector2 targetOffset = direction * r;

                if (Ib.LimitMode == LevelBoundary.Mode.Limit)
                {
                    transform.position = center + targetOffset;
                }

                if (Ib.LimitMode == LevelBoundary.Mode.Teleport)
                {
                    transform.position = center - targetOffset;
                }
            }
        }
    }
}