using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    /// <summary>
    /// Живой трекер цели уровня: собирает подсказку из условий
    /// LevelController и отмечает выполненные пункты цветом.
    /// Заголовок берётся из LevelProperties.Hint, а если он пуст —
    /// из названия уровня.
    /// </summary>
    public class LevelHintText : MonoBehaviour
    {
        private const string CompletedColor = "#7CFC00";
        private const string PendingColor = "#FFFFFF";

        [SerializeField] private Text m_Text;

        private string m_CachedText;

        private void Update()
        {
            if (m_Text == null) return;

            string text = BuildText();

            if (text != m_CachedText)
            {
                m_Text.text = text;
                m_CachedText = text;
            }
        }

        private string BuildText()
        {
            LevelController controller = LevelController.Instance;
            if (controller == null) return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.Append(GetHeader(controller));

            LevelCondition[] conditions = controller.Conditions;
            if (conditions == null || conditions.Length == 0) return builder.ToString();

            LevelCondition[] ordered = new LevelCondition[conditions.Length];
            System.Array.Copy(conditions, ordered, conditions.Length);
            System.Array.Sort(ordered, CompareByHintOrder);

            for (int i = 0; i < ordered.Length; i++)
            {
                LevelCondition condition = ordered[i];
                if (condition == null) continue;

                string line = condition.GetHintText();
                if (string.IsNullOrEmpty(line)) continue;

                bool completed = condition.isCompleted;
                string color = completed ? CompletedColor : PendingColor;
                string mark = completed ? "[x]" : "[ ]";

                builder.Append('\n');
                builder.Append("<color=").Append(color).Append('>');
                builder.Append(mark).Append(' ').Append(line);
                builder.Append("</color>");
            }

            return builder.ToString();
        }

        private static string GetHeader(LevelController controller)
        {
            if (controller.Properties != null)
            {
                string hint = controller.Properties.Hint;
                if (string.IsNullOrEmpty(hint) == false) return hint;

                string title = controller.Properties.Title;
                if (string.IsNullOrEmpty(title) == false) return title;
            }

            return "Objective";
        }

        private static int CompareByHintOrder(LevelCondition left, LevelCondition right)
        {
            int leftOrder = left != null ? left.HintOrder : int.MaxValue;
            int rightOrder = right != null ? right.HintOrder : int.MaxValue;

            return leftOrder.CompareTo(rightOrder);
        }
    }
}
