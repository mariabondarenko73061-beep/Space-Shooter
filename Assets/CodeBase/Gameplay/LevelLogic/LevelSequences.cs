using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    [CreateAssetMenu(fileName = "LevelSequences", menuName = "Space Shooter/Level Sequences")]
    public class LevelSequences : ScriptableObject
    {
        [SerializeField] private LevelProperties[] m_LevelsProperties;

        public LevelProperties[] LevelsProperties => m_LevelsProperties;

        public int Count => m_LevelsProperties != null ? m_LevelsProperties.Length : 0;

        public LevelProperties GetLevel(int index)
        {
            if (m_LevelsProperties == null) return null;
            if (index < 0 || index >= m_LevelsProperties.Length) return null;

            return m_LevelsProperties[index];
        }

        public LevelProperties FindBySceneName(string sceneName)
        {
            if (m_LevelsProperties == null || string.IsNullOrEmpty(sceneName)) return null;

            for (int i = 0; i < m_LevelsProperties.Length; i++)
            {
                LevelProperties level = m_LevelsProperties[i];
                if (level != null && level.SceneName == sceneName)
                    return level;
            }

            return null;
        }

        public int IndexOf(LevelProperties levelProperties)
        {
            if (m_LevelsProperties == null || levelProperties == null) return -1;

            for (int i = 0; i < m_LevelsProperties.Length; i++)
            {
                if (m_LevelsProperties[i] == levelProperties)
                    return i;
            }

            return -1;
        }
    }
}
