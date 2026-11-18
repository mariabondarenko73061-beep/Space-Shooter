using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Requires the player to destroy the number of enemies configured
    /// in the current level's LevelProperties asset.
    /// </summary>
    public class LevelCompletionScore : LevelCondition
    {
        public override int HintOrder => 0;

        public override string GetHintText()
        {
            int requiredKills = 1;
            LevelController controller = LevelController.Instance;

            if (controller != null && controller.Properties != null)
                requiredKills = controller.Properties.RequiredKills;

            int kills = Player.Instance != null ? Player.Instance.NumKills : 0;

            return string.Format("Kills: {0}/{1}", kills, requiredKills);
        }

        public override bool isCompleted
        {
            get
            {
                if (Player.Instance == null || Player.Instance.ActiveShip == null) return false;

                int requiredKills = 1;
                LevelController controller = LevelController.Instance;

                if (controller != null && controller.Properties != null)
                    requiredKills = controller.Properties.RequiredKills;

                return Player.Instance.NumKills >= requiredKills;
            }
        }
    }
}
