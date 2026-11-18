using UnityEngine;

namespace SpaceShooter
{
    public class LevelSelectionPanel : MonoBehaviour
    {
        [SerializeField] private LevelSellectionButton m_LevelButtonPrefab;
        [SerializeField] private Transform m_Parent;

        private bool m_Built;

        private void OnEnable()
        {
            BuildLevelButtons();
        }

        public void BuildLevelButtons()
        {
            if (m_Built) return;
            if (m_LevelButtonPrefab == null) return;

            LevelSequencesController controller = LevelSequencesController.GetOrFind();
            if (controller == null || controller.LevelSequences == null) return;

            LevelProperties[] levels = controller.LevelSequences.LevelsProperties;
            if (levels == null || levels.Length == 0) return;

            Transform parent = m_Parent != null ? m_Parent : transform;

            // The panel may already contain hand-wired level buttons
            // (for example in the MainMenuGUI prefab). Remove them first so
            // the visible buttons always match the current LevelSequences.
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                GameObject existing = parent.GetChild(i).gameObject;
                existing.SetActive(false);
                Destroy(existing);
            }

            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] == null) continue;

                LevelSellectionButton button = Instantiate(m_LevelButtonPrefab, parent);
                button.SetLevelProperties(levels[i]);
            }

            m_Built = true;
        }
    }
}
