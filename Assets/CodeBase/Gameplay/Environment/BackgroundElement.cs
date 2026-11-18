using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BackgroundElement : MonoBehaviour
    {
        [Range(0.0f, 4.0f)]
        [SerializeField] private float m_ParallaxPower;

        [SerializeField] private float m_TextureScale;

        private Material m_QuadMaterial;
        private Vector2 m_InitialOffset;

        private void Start()
        {
            m_QuadMaterial = GetComponent<MeshRenderer>().material;
            m_InitialOffset = UnityEngine.Random.insideUnitCircle;

            if (m_QuadMaterial != null)
                m_QuadMaterial.mainTextureScale = Vector2.one * m_TextureScale;
        }

        private void Update()
        {
            if (m_QuadMaterial == null) return;
            if (Mathf.Approximately(m_ParallaxPower, 0f)) return;

            Vector3 scale = transform.localScale;

            Vector2 offset = m_InitialOffset;

            if (!Mathf.Approximately(scale.x, 0f))
                offset.x += transform.position.x / scale.x / m_ParallaxPower;

            if (!Mathf.Approximately(scale.y, 0f))
                offset.y += transform.position.y / scale.y / m_ParallaxPower;

            m_QuadMaterial.mainTextureOffset = offset;

        }

    }
}