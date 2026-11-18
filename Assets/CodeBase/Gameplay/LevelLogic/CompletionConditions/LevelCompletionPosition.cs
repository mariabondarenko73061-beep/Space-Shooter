using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceShooter
{
    public class LevelCompletionPosition : LevelCondition
    {
        [SerializeField] private float m_Radius;

        public override int HintOrder => 2;

        public override string GetHintText()
        {
            return "Reach the finish";
        }

        public override bool isCompleted
        {
            get
            {
                if (Player.Instance == null || Player.Instance.ActiveShip == null) return false;

                // The game is 2D, so the ship and the finish marker may sit on different Z layers.
                // Compare only the visible X/Y position; otherwise the Z offset makes the distance
                // larger than the radius and the finish can never be reached.
                Vector2 shipPosition = Player.Instance.ActiveShip.transform.position;
                Vector2 finishPosition = transform.position;

                return Vector2.Distance(shipPosition, finishPosition) <= m_Radius;
            }
        }
#if UNITY_EDITOR
        private static Color GizmoColor = new Color(0, 1, 0, 0.3f);

        private void OnDrawGizmosSelected()
        {
            Handles.color = GizmoColor;
            Handles.DrawSolidDisc(transform.position, transform.forward, m_Radius);
        }
#endif
    }
}
