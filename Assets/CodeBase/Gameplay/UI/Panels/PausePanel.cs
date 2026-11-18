using UnityEngine;
using UnityEngine.SceneManagement;


namespace SpaceShooter
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private GameObject m_Panel;

        private void Start()
        {
            if (m_Panel != null)
                m_Panel.SetActive(false);

            Time.timeScale = 1;
        }

        public void ShowPause()
        {
            if (m_Panel != null)
                m_Panel.SetActive(true);

            Time.timeScale = 0;
        }

        public void HidePause()
        { 
            if (m_Panel != null)
                m_Panel.SetActive(false);

            Time.timeScale = 1;
        }

        public void LoadMainMenu()
        {
            if (m_Panel != null)
                m_Panel.SetActive(false);

            Time.timeScale = 1;

            SceneManager.LoadScene(0);
        }
    }
}
