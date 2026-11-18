using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Подбирает спрайт планеты по её типу и размеру.
    /// Притягивающие планеты (GravityWell с ненулевой силой) получают один набор
    /// спрайтов, обычные — другой. Внутри набора есть три размера: маленькая,
    /// средняя и большая планета.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlanetVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_Renderer;

        [Header("Обычные планеты")]
        [SerializeField] private Sprite m_NormalSmall;
        [SerializeField] private Sprite m_NormalMedium;
        [SerializeField] private Sprite m_NormalLarge;

        [Header("Притягивающие планеты")]
        [SerializeField] private Sprite m_GravitySmall;
        [SerializeField] private Sprite m_GravityMedium;
        [SerializeField] private Sprite m_GravityLarge;

        [Header("Размеры (мировые единицы, диаметр диска планеты)")]
        [SerializeField] private float m_MediumDiameter = 1.6f;
        [SerializeField] private float m_LargeDiameter = 2.6f;

        /// <summary>
        /// Номинальный размер исходной текстуры планеты (Планета.png, 2134 px, 100 PPU).
        /// Нужен, чтобы определить размер планеты до замены спрайта: у спрайтов
        /// с кольцами картинка больше самого диска.
        /// </summary>
        [SerializeField] private float m_ReferenceSpriteSize = 21.339f;

        [Header("Тип")]
        [SerializeField] private bool m_AutoDetectGravity = true;
        [SerializeField] private bool m_IsGravity;

        private void Awake()
        {
            Apply();
        }

        private void OnEnable()
        {
            // ExecuteAlways: держим правильный спрайт и в режиме редактирования,
            // чтобы планеты не выглядели одинаковыми до запуска игры.
            Apply();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<SpriteRenderer>();
            }

            // Не трогаем сам префаб-ассет, обновляем только объекты в сцене.
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            {
                Apply();
            }
        }
#endif

        /// <summary>
        /// Пересчитывает спрайт планеты. Нужно процедурной генерации уровня:
        /// она меняет масштаб уже после создания объекта, поэтому Awake/OnEnable
        /// видят ещё старый размер и спрайт выбирается заново.
        /// </summary>
        public void Refresh()
        {
            Apply();
        }

        private void Apply()
        {
            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<SpriteRenderer>();
            }

            if (m_Renderer == null) return;

            if (m_AutoDetectGravity)
            {
                GravityWell well = GetComponentInParent<GravityWell>();
                m_IsGravity = well != null && well.Force > 0f;
            }

            float diameter = m_ReferenceSpriteSize * Mathf.Abs(transform.lossyScale.y);

            Sprite sprite;
            if (m_IsGravity)
            {
                sprite = diameter < m_MediumDiameter ? m_GravitySmall
                       : diameter < m_LargeDiameter ? m_GravityMedium
                       : m_GravityLarge;
            }
            else
            {
                sprite = diameter < m_MediumDiameter ? m_NormalSmall
                       : diameter < m_LargeDiameter ? m_NormalMedium
                       : m_NormalLarge;
            }

            if (sprite != null && m_Renderer.sprite != sprite)
            {
                m_Renderer.sprite = sprite;
            }
        }
    }
}