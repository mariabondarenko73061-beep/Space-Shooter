using UnityEngine;
using Common;

namespace SpaceShooter
{
    [RequireComponent(typeof(SpaceShip))]
    public class AIController : MonoBehaviour
    {
        public enum AIBehaviour
        {
            Null,
            Patrol
        }

        [SerializeField] private AIBehaviour m_AIBehaviour;

        [SerializeField] private AIPointPatrol m_PatrolPoint;

        [Header("Patrol Route")]
        [SerializeField] private Transform[] m_PatrolRoute;
        [SerializeField] private float m_PatrolPointReachRadius = 1.5f;
        private int m_CurrentPatrolIndex;

        [Header("Aggro")]
        [SerializeField] private float m_AggroRadius = 8f;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_NavigationLinear;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_NavigationAngular;

        [SerializeField] private float m_RandomSelectMovePointTime;

        [SerializeField] private float m_FindNewTargetTime;

        [SerializeField] private float m_ShootDelay;

        [SerializeField] private float m_EvadeRayLegth;

        private SpaceShip m_SpaceShip;

        private Vector3 m_MovePosition;

        private Destructible m_SelectedTarget;

        private Timer m_RandomizeDirectionTimer;
        private Timer m_FireTimer;
        private Timer m_FindNewTargetTimer;

        private void Start()
        {
           m_SpaceShip = GetComponent<SpaceShip>();

            InitTimers();
        }

        private void Update()
        {
            UpdateTimers();

            UpdateAi();
        }

        private void UpdateAi()
        {
            if (m_AIBehaviour == AIBehaviour.Patrol)
            {
                UpdateBehaviourPatrol();
            }
        }

        private void UpdateBehaviourPatrol()
        {
            ActionFindNewMovePosition();
            ActionControlShip();
            ActionFindNewAttackTarget();
            ActionFire();
            ActionEvadeCollision();
        }

        private void ActionFindNewMovePosition()
        {
            if (m_AIBehaviour != AIBehaviour.Patrol) return;

            if (m_SelectedTarget != null)
            {
                float distToTarget = Vector2.Distance(transform.position, m_SelectedTarget.transform.position);

                if (distToTarget <= m_AggroRadius)
                {
                    m_MovePosition = m_SelectedTarget.transform.position;
                    return;
                }
                else
                {
                    m_SelectedTarget = null;
                }
            }

            if (m_PatrolRoute != null && m_PatrolRoute.Length > 0)
            {
                Transform currentPoint = GetCurrentPatrolPoint();
                if (currentPoint != null)
                {
                    m_MovePosition = currentPoint.position;

                    float distToMovePos = (m_MovePosition - transform.position).sqrMagnitude;
                    float reachSqr = m_PatrolPointReachRadius * m_PatrolPointReachRadius;

                    if (distToMovePos < reachSqr)
                    {
                        m_CurrentPatrolIndex = (m_CurrentPatrolIndex + 1) % m_PatrolRoute.Length;
                    }
                    return;
                }
            }

            if (m_PatrolPoint != null)
            {
                bool isInsidePatrolZone = (m_PatrolPoint.transform.position - transform.position).sqrMagnitude < m_PatrolPoint.Radius * m_PatrolPoint.Radius;

                if (isInsidePatrolZone == true)
                {
                    if (m_RandomizeDirectionTimer.IsFinished == true)
                    {
                        Vector2 newPoint = UnityEngine.Random.onUnitSphere * m_PatrolPoint.Radius + m_PatrolPoint.transform.position;

                        m_MovePosition = newPoint;

                        m_RandomizeDirectionTimer.Start(m_RandomSelectMovePointTime);
                    }
                }
                else
                {
                    m_MovePosition = m_PatrolPoint.transform.position;
                }
            }
        }

        private Transform GetCurrentPatrolPoint()
        {
            if (m_PatrolRoute == null || m_PatrolRoute.Length == 0) return null;
            if (m_CurrentPatrolIndex < 0 || m_CurrentPatrolIndex >= m_PatrolRoute.Length)
                m_CurrentPatrolIndex = 0;
            return m_PatrolRoute[m_CurrentPatrolIndex];
        }

        private void ActionEvadeCollision()
        {
            if (Physics2D.Raycast(transform.position, transform.up, m_EvadeRayLegth) == true)
            {
                m_MovePosition = transform.position + transform.right * 100.0f;
            }
        }

        private void ActionControlShip()
        {
            if (m_SpaceShip == null) return;

            m_SpaceShip.ThrustControl = m_NavigationLinear;
            m_SpaceShip.TorqueControl = ComputeAliginTorqueTNormalized(m_MovePosition, m_SpaceShip.transform) * m_NavigationAngular;
        }

        private const float MAX_ANGLE = 45.0f;

        private static float ComputeAliginTorqueTNormalized(Vector3 targetPosition, Transform ship)
        {
            Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);

            float angle = Vector3.SignedAngle(localTargetPosition, Vector3.up, Vector3.forward);

            angle = Mathf.Clamp(angle, -MAX_ANGLE, MAX_ANGLE) / MAX_ANGLE;

            return -angle;
        }

        private void ActionFindNewAttackTarget()
        {
            if (m_FindNewTargetTimer.IsFinished == true)
            {
                Destructible nearest = FindNearesDestructableTarget();

                if (nearest != null)
                {
                    float dist = Vector2.Distance(transform.position, nearest.transform.position);
                    if (dist <= m_AggroRadius)
                    {
                        m_SelectedTarget = nearest;
                    }
                    else
                    {
                        m_SelectedTarget = null;
                    }
                }
                else
                {
                    m_SelectedTarget = null;
                }

                m_FindNewTargetTimer.Start(m_FindNewTargetTime);
            }
        }

        private void ActionFire()
        {
            if (m_SelectedTarget == null) return;
            if (m_SpaceShip == null) return;

            float distToTarget = Vector2.Distance(transform.position, m_SelectedTarget.transform.position);
            if (distToTarget > m_AggroRadius)
            {
                m_SelectedTarget = null;
                return;
            }

            if (m_FireTimer.IsFinished == true)
            {
                m_SpaceShip.Fire(TurretMode.Primary);
                m_FireTimer.Start(m_ShootDelay);
            }
        }

        private Destructible FindNearesDestructableTarget()
        {
            if (m_SpaceShip == null) return null;
            if (Destructible.AllDestructibles == null) return null;

            float maxDist = float.MaxValue;
            Destructible potentialTarget = null;

            foreach (var v in Destructible.AllDestructibles)
            {
                if (v == null) continue;
                if (v.GetComponent<SpaceShip>() == m_SpaceShip) continue;
                if (v.TeamId == Destructible.TeamIdNeutral) continue;
                if (v.TeamId == m_SpaceShip.TeamId) continue;

                float dist = Vector2.Distance(m_SpaceShip.transform.position, v.transform.position);
                if (dist > m_AggroRadius) continue;

                if (dist < maxDist)
                {
                    maxDist = dist;
                    potentialTarget = v;
                }
            }
            return potentialTarget;
        }

        #region Timers
        private void InitTimers()
        {
            m_RandomizeDirectionTimer = new Timer(m_RandomSelectMovePointTime);
            m_FindNewTargetTimer = new Timer(m_FindNewTargetTime);
            m_FireTimer = new Timer(m_ShootDelay);
        }

        private void UpdateTimers()
        {
            m_RandomizeDirectionTimer.RemoveTime(Time.deltaTime);
            m_FireTimer.RemoveTime(Time.deltaTime);
            m_FindNewTargetTimer.RemoveTime(Time.deltaTime);
        }

        public void SetPatrolBehaviour(AIPointPatrol point)
        {
            m_AIBehaviour = AIBehaviour.Patrol;
            m_PatrolPoint = point;
        }

        #endregion
    }
}