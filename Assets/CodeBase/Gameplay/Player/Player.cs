using SpaceShooter;
using UnityEngine;

public class Player : SingletonBase<Player>
{
    public static SpaceShip SelectedSpaceShip;

    [SerializeField] private int m_NumLives;
    [SerializeField] private SpaceShip m_PlayerShipPrefab;

    public SpaceShip ActiveShip => m_Ship;

    private FollowCamera m_FollowCamera;
    private ShipInputController m_ShipInputController;
    private Transform m_SpawnPoint;

    public FollowCamera FollowCamera => m_FollowCamera;

    private SpaceShip m_Ship;

    private int m_Score;
    private int m_NumKills;

    public int Score => m_Score;
    public int NumKills => m_NumKills;
    public int NumLives => m_NumLives;

    public SpaceShip ShipPrefab
    {
        get
        {
            if (SelectedSpaceShip != null)
                return SelectedSpaceShip;

            return m_PlayerShipPrefab;
        }
    }

    public void Construct(FollowCamera followCamera, ShipInputController shipInputController, Transform spawnPoint)
    {
        if (followCamera != null)
            m_FollowCamera = followCamera;

        if (shipInputController != null)
            m_ShipInputController = shipInputController;

        if (spawnPoint != null)
            m_SpawnPoint = spawnPoint;

        // The spawner may be created after this object has already spawned a ship.
        // Wire the new camera/input to the existing ship in that case.
        WireShip();
    }

    private void Start()
    {
        if (m_NumLives <= 0)
            m_NumLives = 1;

        if (m_FollowCamera == null)
            m_FollowCamera = GetComponent<FollowCamera>();

        if (m_ShipInputController == null)
            m_ShipInputController = GetComponent<ShipInputController>();

        Respawn();
    }

    private void RegisterShipEvents()
    {
        if (m_Ship == null) return;

        m_Ship.EventOnDeath.RemoveListener(OnShipDeath);
        m_Ship.EventOnDeath.AddListener(OnShipDeath);
    }

    private void OnShipDeath()
    {
        if (m_Ship != null)
            m_Ship.EventOnDeath.RemoveListener(OnShipDeath);

        m_NumLives--;

        if (m_NumLives > 0)
            Respawn();
        else
            Debug.Log("Game Over");
    }

    private void Respawn()
    {
        SpaceShip shipPrefab = ShipPrefab;

        if (shipPrefab == null)
        {
            Debug.LogError("Player: ship prefab is not assigned.");
            return;
        }

        Transform spawnPoint = m_SpawnPoint != null ? m_SpawnPoint : transform;
        m_Ship = Instantiate(shipPrefab, spawnPoint.position, spawnPoint.rotation);

        if (m_Ship == null)
        {
            Debug.LogError("Player: failed to spawn the ship.");
            return;
        }

        WireShip();
        RegisterShipEvents();
    }

    private void WireShip()
    {
        if (m_Ship == null) return;

        if (m_FollowCamera != null)
            m_FollowCamera.SetTarget(m_Ship.transform);

        if (m_ShipInputController != null)
            m_ShipInputController.SetTargetShip(m_Ship);
    }

    public void Addkill()
    {
        m_NumKills += 1;
    }

    public void AddScore(int num)
    {
        m_Score += num;
    }
}
