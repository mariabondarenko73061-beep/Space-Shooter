using UnityEngine;
using Common;

namespace SpaceShooter
{
    public class EntitySpawner : MonoBehaviour
    {
        public enum SpawnMode
        {
            Start,
            Loop
        }

        [SerializeField] private Entity[] m_EntityPrefabs;
        [SerializeField] private CircleArea m_Area;
        [SerializeField] private SpawnMode m_SpawnMode;
        [SerializeField] private int m_NumSpawns;
        [SerializeField] private float m_RespawnTime;

        private float m_Timer;

        public int NumSpawns => m_NumSpawns;
        public float RespawnTime => m_RespawnTime;
        public SpawnMode Mode => m_SpawnMode;

        public void Configure(int numSpawns, float respawnTime, SpawnMode spawnMode)
        {
            m_NumSpawns = Mathf.Max(0, numSpawns);
            m_RespawnTime = Mathf.Max(0f, respawnTime);
            m_SpawnMode = spawnMode;
        }

        private void Start()
        {
            if (m_Area == null)
                m_Area = GetComponent<CircleArea>();

            if (m_SpawnMode == SpawnMode.Start)
            {
                SpawnEntities();
            }

            if (m_RespawnTime > 0)
                m_Timer = m_RespawnTime;
        }

        private void Update()
        {
            if (m_Timer > 0)
                m_Timer -= Time.deltaTime;

            if (m_SpawnMode == SpawnMode.Loop && m_Timer <= 0 && m_RespawnTime > 0)
            {
                SpawnEntities();
                m_Timer = m_RespawnTime;
            }
        }

        private void SpawnEntities()
        {
            if (m_EntityPrefabs == null || m_EntityPrefabs.Length == 0) return;
            if (m_Area == null) return;
            if (m_NumSpawns <= 0) return;

            for (int i = 0; i < m_NumSpawns; i++)
            {
                int index = Random.Range(0, m_EntityPrefabs.Length);
                if (index < 0 || index >= m_EntityPrefabs.Length) continue;
                if (m_EntityPrefabs[index] == null) continue;

                GameObject e = Instantiate(m_EntityPrefabs[index].gameObject);
                e.transform.position = m_Area.GetRandomInsideZone();
            }
        }
    }
}