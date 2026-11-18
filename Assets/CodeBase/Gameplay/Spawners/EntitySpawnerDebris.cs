using UnityEngine;
using Common;

namespace SpaceShooter
{
    public class EntitySpawnerDebris : MonoBehaviour
    {
        [SerializeField] private Destructible[] m_DebrisPrefabs;
        [SerializeField] private int m_NumDebris;
        [SerializeField] private CircleArea m_Area;
        [SerializeField] private float m_RandomSpeed;

        public int NumDebris => m_NumDebris;

        public void Configure(int numDebris)
        {
            m_NumDebris = Mathf.Max(0, numDebris);
        }

        private void Start()
        {
            if (m_Area == null)
                m_Area = GetComponent<CircleArea>();

            if (m_DebrisPrefabs == null || m_DebrisPrefabs.Length == 0) return;
            if (m_Area == null) return;

            for (int i = 0; i< m_NumDebris;  ++i)
            {
                SpawnDebris();
            }
        }

        private void SpawnDebris()
        {
            if (m_DebrisPrefabs == null || m_DebrisPrefabs.Length == 0) return;
            if (m_Area == null) return;

            int index = Random.Range(0, m_DebrisPrefabs.Length);

            if (m_DebrisPrefabs[index] == null) return;

            GameObject debris = Instantiate(m_DebrisPrefabs[index].gameObject);

            debris.transform.position = m_Area.GetRandomInsideZone();

            Destructible destructible = debris.GetComponent<Destructible>();

            if (destructible != null)
                destructible.EventOnDeath.AddListener(OnDebrisDeat);

            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();

            if (rb != null && m_RandomSpeed > 0)
            {
                rb.velocity = (Vector2)UnityEngine.Random.insideUnitSphere * m_RandomSpeed;
            }
        }

        private void OnDebrisDeat()
        {
            SpawnDebris();
        }
    }
}