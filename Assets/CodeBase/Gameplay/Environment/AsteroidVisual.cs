using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Выбирает случайный спрайт астероида и слегка его поворачивает,
    /// чтобы одинаковые камни не выглядели клонами.
    /// </summary>
    public class AsteroidVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_Renderer;
        [SerializeField] private Sprite[] m_Sprites;

        [Header("Случайность")]
        [SerializeField] private bool m_RandomSprite = true;
        [SerializeField] private bool m_RandomRotation = true;
        [SerializeField] private bool m_RandomFlip = true;
        [SerializeField] private float m_ScaleJitter = 0.12f;

        private void Start()
        {
            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<SpriteRenderer>();
            }

            if (m_Renderer == null) return;

            if (m_RandomSprite && m_Sprites != null && m_Sprites.Length > 0)
            {
                m_Renderer.sprite = m_Sprites[Random.Range(0, m_Sprites.Length)];
            }

            if (m_RandomFlip)
            {
                m_Renderer.flipX = Random.value > 0.5f;
                m_Renderer.flipY = Random.value > 0.5f;
            }

            if (m_RandomRotation)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            }

            if (m_ScaleJitter > 0f)
            {
                float k = 1f + Random.Range(-m_ScaleJitter, m_ScaleJitter);
                transform.localScale = transform.localScale * k;
            }
        }
    }
}