using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class ResultPanel : MonoBehaviour
    {
        private const string PassedText = "Passed";
        private const string LoseText = "Lose";
        private const string RestartText = "Restart";
        private const string NextText = "Next";
        private const string MainMenuText = "Main menu";
        private const string KillsTextPrefix = "Kills : ";
        private const string ScoreTextPrefix = "Scores : ";
        private const string TimeTextPrefix = "Time : ";

        [SerializeField] private Text m_Kills;
        [SerializeField] private Text m_Score;
        [SerializeField] private Text m_Time;
        [SerializeField] private Text m_Result;
        [SerializeField] private Text m_ButtonNextText;

        private bool m_LevelPassed = false;
        private LevelController m_LevelController;

        private void Start()
        {
            gameObject.SetActive(false);

            m_LevelController = LevelController.Instance;

            if (m_LevelController == null)
                return;

            m_LevelController.LevelLost += OnLevelLost;
            m_LevelController.LevelPassed += OnLevelPassed;
        }

        private void OnDestroy()
        {
            if (m_LevelController == null) return;

            m_LevelController.LevelLost -= OnLevelLost;
            m_LevelController.LevelPassed -= OnLevelPassed;
        }

        private void OnLevelPassed()
        {
            gameObject.SetActive(true);

            m_LevelPassed = true;

            if (m_Result != null)
                m_Result.text = PassedText;

            FillLevelStatistics();

            if (m_ButtonNextText != null)
            {
                if (m_LevelController != null && m_LevelController.HasNextLevel == true)
                    m_ButtonNextText.text = NextText;
                else
                    m_ButtonNextText.text = MainMenuText;
            }
        }

        private void OnLevelLost()
        {
            gameObject.SetActive(true);

            m_LevelPassed = false;

            FillLevelStatistics();

            if (m_Result != null)
                m_Result.text = LoseText;

            if (m_ButtonNextText != null)
                m_ButtonNextText.text = RestartText;
        }

        private void FillLevelStatistics()
        {
            if (m_Kills != null)
                m_Kills.text = KillsTextPrefix + (Player.Instance != null ? Player.Instance.NumKills.ToString() : "0");

            if (m_Score != null)
                m_Score.text = ScoreTextPrefix + (Player.Instance != null ? Player.Instance.Score.ToString() : "0");

            if (m_Time != null)
                m_Time.text = TimeTextPrefix + (m_LevelController != null ? m_LevelController.LevelTime.ToString("F0") : "0");
        }

        public void OnButtonNextAction()
        {
            gameObject.SetActive(false);

            if (m_LevelController == null)
                return;

            if (m_LevelPassed == true)
                m_LevelController.LoadNextLevel();
            else
                m_LevelController.RestartLevel();
        }
    }
}
