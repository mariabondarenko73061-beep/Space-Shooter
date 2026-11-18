using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    /// <summary>
    /// Игровое представление разрушаемого объекта: команда, очки за уничтожение
    /// и событие смерти. Общая механика прочности живёт в Common.DestructibleBase.
    /// </summary>
    public class Destructible : DestructibleBase
    {
        public const int TeamIdNeutral = 0;

        [SerializeField] private int m_TeamId;
        public int TeamId => m_TeamId;

        [SerializeField] private int m_ScoreValue;
        public int ScoreValue => m_ScoreValue;

        [SerializeField] private UnityEvent m_EventOnDeath;
        public UnityEvent EventOnDeath => m_EventOnDeath;

        private static HashSet<Destructible> m_AllDestructibles;

        public static IReadOnlyCollection<Destructible> AllDestructibles => m_AllDestructibles;

        protected override void OnDeath()
        {
            base.OnDeath();

            m_EventOnDeath?.Invoke();
        }

        protected virtual void OnEnable()
        {
            if (m_AllDestructibles == null)
                m_AllDestructibles = new HashSet<Destructible>();

            m_AllDestructibles.Add(this);
        }

        protected virtual void OnDestroy()
        {
            if (m_AllDestructibles == null) return;

            m_AllDestructibles.Remove(this);
        }
    }
}
