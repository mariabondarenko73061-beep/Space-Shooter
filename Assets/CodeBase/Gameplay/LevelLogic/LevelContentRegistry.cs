using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Каталог префабов окружения, которые генератор уровня расставляет
    /// динамически: декоративные планеты, гравитационные планеты, стены и
    /// патрульные корабли. Ассет лежит в папке Resources, поэтому генератор
    /// получает префабы по имени и сцены не нужно редактировать вручную.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelContent", menuName = "SpaceShooter/Level Content")]
    public class LevelContentRegistry : ScriptableObject
    {
        public const string ResourcePath = "LevelContent";

        [Header("Планеты (декор)")]
        [SerializeField] private GameObject[] m_Planets;

        [Header("Гравитационные планеты")]
        [SerializeField] private GameObject[] m_GravityWells;

        [Header("Стены-препятствия")]
        [SerializeField] private GameObject[] m_Walls;

        [Header("Патрульные корабли")]
        [SerializeField] private GameObject[] m_PatrolShips;

        public GameObject[] Planets => m_Planets;
        public GameObject[] GravityWells => m_GravityWells;
        public GameObject[] Walls => m_Walls;
        public GameObject[] PatrolShips => m_PatrolShips;

        private static LevelContentRegistry s_Instance;

        /// <summary>
        /// Загруженный из Resources каталог. Может быть null, если ассет
        /// отсутствует: в этом случае генератор просто не создаёт
        /// соответствующие объекты.
        /// </summary>
        public static LevelContentRegistry Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = Resources.Load<LevelContentRegistry>(ResourcePath);

                return s_Instance;
            }
        }

        /// <summary>
        /// Возвращает случайный префаб из набора или null, если набор пуст.
        /// </summary>
        public static GameObject Pick(GameObject[] prefabs)
        {
            if (prefabs == null || prefabs.Length == 0) return null;

            int index = Random.Range(0, prefabs.Length);
            return prefabs[index];
        }
    }
}
