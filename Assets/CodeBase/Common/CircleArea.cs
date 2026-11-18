using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Common
{
    public class CircleArea : MonoBehaviour
    {
        [SerializeField] private float m_Radius;
        public float Radius => m_Radius;

        public void SetRadius(float radius)
        {
            m_Radius = Mathf.Max(0f, radius);
        }

        public Vector2 GetRandomInsideZone()
        {
            return (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * m_Radius;
        }

        #if UNITY_EDITOR
        private static Color GizmoColor = new Color(0,1,0,0.3f);

        private void OnDrawGizmosSelected()
        {
            Handles.color = GizmoColor;
            Handles.DrawSolidDisc(transform.position, transform.forward, m_Radius);
        }
        #endif
    }
}