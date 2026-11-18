using UnityEngine;

public class AIPointPatrol : MonoBehaviour
{
    [SerializeField] private float m_Radius;
    public float Radius => m_Radius;

    /// <summary>
    /// Задаёт радиус зоны патрулирования. Нужен процедурной генерации уровня,
    /// которая создаёт точки патруля во время выполнения.
    /// </summary>
    public void Configure(float radius)
    {
        m_Radius = Mathf.Max(0f, radius);
    }

    private static readonly Color GizmoColor = new Color (1, 0, 0, 0.3f);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawSphere(transform.position, m_Radius);
    }
}