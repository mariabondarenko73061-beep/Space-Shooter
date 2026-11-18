using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Requires the player to earn the number of points configured in the
    /// current level's LevelProperties asset. A value of zero means the
    /// condition is satisfied immediately and never blocks the level.
    /// </summary>
    public class LevelCompletionScoreValue : LevelCondition
    {
        public override int HintOrder => 1;

        public override string GetHintText()
        {
            int requiredScore = GetRequiredScore();
            int score = Player.Instance != null ? Player.Instance.Score : 0;

            return string.Format("Score: {0}/{1}", score, requiredScore);
        }

        public override bool isCompleted
        {
            get
            {
                if (Player.Instance == null || Player.Instance.ActiveShip == null) return false;

                return Player.Instance.Score >= GetRequiredScore();
            }
        }

        private static int GetRequiredScore()
        {
            LevelController controller = LevelController.Instance;

            if (controller != null && controller.Properties != null)
                return controller.Properties.RequiredScore;

            return 0;
        }
    }
}
