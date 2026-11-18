using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    public class LevelController : SingletonBase<LevelController>
    {
        private const string MainMenuSceneName = "main_menu";

        public event UnityAction LevelPassed;
        public event UnityAction LevelLost;

        [SerializeField] private LevelProperties m_LevelProperties;
        [SerializeField] private LevelCondition[] m_Conditions;

        private LevelSequencesController m_LevelSequencesController;
        private bool m_IsLevelCompleted;
        private float m_LevelTime;

        public bool HasNextLevel
        {
            get
            {
                if (GetNextLevel() != null) return true;

                return m_LevelProperties != null && m_LevelProperties.NextLevel != null;
            }
        }

        public float LevelTime => m_LevelTime;
        public LevelProperties Properties => m_LevelProperties;

        /// <summary>
        /// Условия прохождения уровня. Используются в том числе подсказкой
        /// (LevelHintText), чтобы показать игроку текущую цель.
        /// </summary>
        public LevelCondition[] Conditions => m_Conditions;

        private void Start()
        {
            Time.timeScale = 1;

            m_LevelTime = 0;

            m_LevelSequencesController = LevelSequencesController.GetOrFind();
        }

        private LevelProperties GetNextLevel()
        {
            LevelSequencesController controller = m_LevelSequencesController != null
                ? m_LevelSequencesController
                : LevelSequencesController.GetOrFind();

            if (controller == null) return null;

            LevelProperties nextLevel = controller.GetNextLevelProperties(m_LevelProperties);
            if (nextLevel == null) return null;
            if (string.IsNullOrEmpty(nextLevel.SceneName)) return null;

            return nextLevel;
        }

        private void Update()
        {
            if(m_IsLevelCompleted == false)
            {
                m_LevelTime += Time.deltaTime;
                CheckLevelConditions();
            }

            if (Player.Instance != null && Player.Instance.NumLives <= 0)
            {
                if (m_IsLevelCompleted == false)
                {
                    m_IsLevelCompleted = true;
                    Lose();
                }
            }
        }

        private bool CheckLevelConditions()
        {
            if (m_Conditions == null || m_Conditions.Length == 0) return false;

            int numCompleted = 0;

            for (int i = 0; i < m_Conditions.Length; i++)
            {
                if (m_Conditions[i] != null && m_Conditions[i].isCompleted == true)
                {
                    numCompleted++;
                }
            }

            if (numCompleted == m_Conditions.Length)
            {
                m_IsLevelCompleted = true;

                Pass();

                return true;
            }
            return false;
        }

        private void Lose()
        {
            LevelLost?.Invoke();
            Time.timeScale = 0;
        }

        private void Pass()
        {
            LevelPassed?.Invoke();
            Time.timeScale = 0;
        }

        public void LoadNextLevel()
        {
            LevelProperties nextLevel = GetNextLevel();

            if (nextLevel == null && m_LevelProperties != null && m_LevelProperties.NextLevel != null)
            {
                LevelProperties fallback = m_LevelProperties.NextLevel;
                if (string.IsNullOrEmpty(fallback.SceneName) == false)
                    nextLevel = fallback;
            }

            if (nextLevel != null && string.IsNullOrEmpty(nextLevel.SceneName) == false)
            {
                SceneManager.LoadScene(nextLevel.SceneName);
                return;
            }

            SceneManager.LoadScene(MainMenuSceneName);
        }

        public void RestartLevel()
        {
            if (m_LevelProperties != null && string.IsNullOrEmpty(m_LevelProperties.SceneName) == false)
            {
                SceneManager.LoadScene(m_LevelProperties.SceneName);
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            if (scene.buildIndex >= 0)
                SceneManager.LoadScene(scene.buildIndex);
        }
    }
}