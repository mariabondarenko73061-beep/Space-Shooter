using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField] private Text m_Text;

        private int lastScoreText;

        private void Update()
        {
            if (Player.Instance == null) return;
            if (m_Text == null) return;

            int score = Player.Instance.Score;

            if (lastScoreText != score)
            {
                m_Text.text = "Score : " + score.ToString();
                lastScoreText = score;
            }
        }
    }
}