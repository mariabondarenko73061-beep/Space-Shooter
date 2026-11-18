using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class LevelSellectionButton : MonoBehaviour
    {
        [SerializeField] private LevelProperties m_LevelProperties;
        [SerializeField] private Text m_LevelTitle;
        [SerializeField] private Image m_PreviewImage;

        private void Start()
        {
            SetLevelProperties(m_LevelProperties);
        }

        public void SetLevelProperties(LevelProperties levelProperties)
        {
            m_LevelProperties = levelProperties;

            if (m_LevelProperties == null) return;

            if (m_PreviewImage != null)
                m_PreviewImage.sprite = m_LevelProperties.PreviweImage;

            if (m_LevelTitle != null)
                m_LevelTitle.text = m_LevelProperties.Title;
        }

        public void LoadLevel()
        {
            if (m_LevelProperties == null) return;
            if (string.IsNullOrEmpty(m_LevelProperties.SceneName)) return;

            SceneManager.LoadScene(m_LevelProperties.SceneName);
        }
    }
}