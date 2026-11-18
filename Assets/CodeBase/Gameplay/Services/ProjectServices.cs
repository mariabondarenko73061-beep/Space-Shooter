using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Explicit entry point for the project-wide services. The object lives in
    /// every scene and initializes the singletons that must be ready before
    /// gameplay starts.
    /// </summary>
    public class ProjectServices : MonoBehaviour
    {
        [SerializeField] private LevelSequencesController m_LevelSequences;

        private void Awake()
        {
            // The randomizer is scene-driven, so it only needs its scene
            // loading hook registered before the first gameplay scene starts.
            LevelRandomizer.Init();

            // Prefer the Inspector reference, but fall back to the controller
            // that lives on this object so services still initialize on a
            // freshly created scene.
            if (m_LevelSequences == null)
                m_LevelSequences = GetComponent<LevelSequencesController>();

            if (m_LevelSequences == null)
                m_LevelSequences = FindObjectOfType<LevelSequencesController>();

            if (m_LevelSequences != null)
                m_LevelSequences.Init();
        }
    }
}
