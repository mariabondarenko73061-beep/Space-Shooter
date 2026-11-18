using UnityEngine;
using Common;

namespace SpaceShooter
{
    /// <summary>
    /// Астероид, который при уничтожении раскалывается на более мелкие части.
    /// Каждое следующее поколение меньше, имеет меньше хитов и быстрее вращается.
    /// </summary>
    public class Asteroid : Destructible
    {
        [Header("Splitting")]
        [SerializeField] private int m_Generation;
        [SerializeField] private int m_MaxGeneration = 2;
        [SerializeField] private int m_FragmentsPerSplit = 2;
        [SerializeField] private float m_FragmentScale = 0.6f;
        [SerializeField] private float m_FragmentSpread = 0.45f;
        [SerializeField] private float m_FragmentSpeed = 2f;

        protected override void OnDeath()
        {
            if (m_Generation < m_MaxGeneration)
                SplitIntoFragments();

            base.OnDeath();
        }

        private void SplitIntoFragments()
        {
            int nextGeneration = m_Generation + 1;
            int nextHitPoints = Mathf.Max(1, MaxHitPoints / 2);

            Rigidbody2D sourceBody = GetComponent<Rigidbody2D>();
            Vector2 sourceVelocity = sourceBody != null ? sourceBody.velocity : Vector2.zero;

            for (int i = 0; i < m_FragmentsPerSplit; i++)
            {
                Vector2 offset = Random.insideUnitCircle * m_FragmentSpread;
                Vector3 spawnPosition = transform.position + new Vector3(offset.x, offset.y, 0f);

                GameObject fragment = Instantiate(gameObject, spawnPosition, transform.rotation);
                fragment.name = name + "_Fragment";
                fragment.transform.localScale = transform.localScale * m_FragmentScale;

                Asteroid fragmentAsteroid = fragment.GetComponent<Asteroid>();

                if (fragmentAsteroid != null)
                {
                    fragmentAsteroid.m_Generation = nextGeneration;
                    fragmentAsteroid.SetMaxHitPoints(nextHitPoints);
                }

                Rigidbody2D fragmentBody = fragment.GetComponent<Rigidbody2D>();

                if (fragmentBody != null)
                {
                    Vector2 direction = offset.sqrMagnitude > 0.0001f
                        ? offset.normalized
                        : Random.insideUnitCircle.normalized;

                    fragmentBody.velocity = sourceVelocity * 0.5f + direction * m_FragmentSpeed;
                    fragmentBody.angularVelocity = Random.Range(-90f, 90f);
                }
            }
        }
    }
}