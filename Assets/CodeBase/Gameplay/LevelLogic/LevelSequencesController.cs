using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    public class LevelSequencesController : SingletonBase<LevelSequencesController>
    {
        private const string DefaultLevelSequencesResource = "LevelSequences";

        [SerializeField] private LevelSequences m_LevelSequences;

        /// <summary>
        /// Ordered list of levels used by the game. When the asset is not
        /// assigned in the Inspector it is loaded from Resources, so the
        /// service keeps working on a freshly created scene.
        /// </summary>
        public LevelSequences LevelSequences
        {
            get
            {
                if (m_LevelSequences == null)
                    m_LevelSequences = Resources.Load<LevelSequences>(DefaultLevelSequencesResource);

                return m_LevelSequences;
            }
        }

        public bool HasLevelSequences
        {
            get
            {
                LevelSequences sequences = LevelSequences;
                return sequences != null && sequences.Count > 0;
            }
        }

        public void SetLevelSequences(LevelSequences sequences)
        {
            m_LevelSequences = sequences;
        }

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// Returns the initialized instance. If the scene has a
        /// LevelSequencesController that was not initialized yet (no
        /// ProjectServices on the scene), it is initialized here.
        /// </summary>
        public static LevelSequencesController GetOrFind()
        {
            if (Instance != null) return Instance;

            LevelSequencesController controller = FindObjectOfType<LevelSequencesController>();
            if (controller != null)
                controller.Init();

            return Instance;
        }

        public bool CurrentLevelIsLast()
        {
            if (HasLevelSequences == false) return true;

            LevelSequences sequences = LevelSequences;
            LevelProperties lastLevel = sequences.GetLevel(sequences.Count - 1);
            if (lastLevel == null) return true;

            string sceneName = SceneManager.GetActiveScene().name;
            return lastLevel.SceneName == sceneName;
        }

        public LevelProperties GetCurrentLoadedLevel()
        {
            if (HasLevelSequences == false) return null;

            string sceneName = SceneManager.GetActiveScene().name;
            LevelProperties level = LevelSequences.FindBySceneName(sceneName);

            if (level == null)
                Debug.LogError("Level properties not found! Scene: " + sceneName);

            return level;
        }

        public LevelProperties GetNextLevelProperties(LevelProperties levelProperties)
        {
            if (HasLevelSequences == false || levelProperties == null) return null;

            LevelSequences sequences = LevelSequences;
            int index = sequences.IndexOf(levelProperties);
            if (index < 0) return null;
            if (index >= sequences.Count - 1) return null;

            return sequences.GetLevel(index + 1);
        }

        public LevelProperties GetNextLevelProperties(string sceneName)
        {
            if (HasLevelSequences == false) return null;

            return GetNextLevelProperties(LevelSequences.FindBySceneName(sceneName));
        }
    }
}
