using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class KillText : MonoBehaviour
    {
        [SerializeField] private Text m_Text;

        private int lastNumKills;

        private void Update()
        {
            if (Player.Instance == null) return;
            if (m_Text == null) return;

            int numKills = Player.Instance.NumKills;

            if(lastNumKills != numKills)
            {
                m_Text.text = "Kills : " + numKills.ToString();
                lastNumKills = numKills;
            }
        }
    }
}
