using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    [CreateAssetMenu]
    public class LevelProperties : ScriptableObject
    {
        [SerializeField] private string m_Title;
        [SerializeField] private string m_SceneName;
        [SerializeField] private Sprite m_PreviweImage;
        [SerializeField] private LevelProperties m_NextLevel;
        [SerializeField] private int m_RequiredKills = 1;
        [SerializeField] private int m_RequiredScore = 0;
        [SerializeField, TextArea] private string m_Hint;

        public string Title => m_Title;
        public string SceneName => m_SceneName;
        public Sprite PreviweImage => m_PreviweImage;
        public LevelProperties NextLevel => m_NextLevel;
        public int RequiredKills => m_RequiredKills;
        public int RequiredScore => m_RequiredScore;

        /// <summary>
        /// Необязательный ручной текст подсказки. Если не задан, подсказка
        /// собирается автоматически из условий уровня (LevelCondition).
        /// </summary>
        public string Hint => m_Hint;
    }
}
