using Common;
using SpaceShooter;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private FollowCamera m_FollowCameraPrefab;
    [SerializeField] private Player m_PlayerPrefab;
    [SerializeField] private ShipInputController m_ShipInputController;
    [SerializeField] private VirtualGamePad m_VirtualGamePad;
    [SerializeField] private Transform m_SpawnPoint;

   public Player Spawn()
    {
        Player player = Player.Instance;

        // The Player may be placed in the scene directly, so only spawn
        // a new one when there is no existing instance.
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        bool reusedExistingPlayer = player != null;

        if (player == null)
        {
            if (m_PlayerPrefab == null)
            {
                Debug.LogError("PlayerSpawner: Player prefab is not assigned.");
                return null;
            }

            player = Instantiate(m_PlayerPrefab);
        }

        FollowCamera followCamera = null;

        if (m_FollowCameraPrefab != null)
        {
            followCamera = Instantiate(m_FollowCameraPrefab);
        }
        else
        {
            followCamera = FindObjectOfType<FollowCamera>();
        }

        WireBackground(followCamera);

        VirtualGamePad virtualGamePad = null;

        if (m_VirtualGamePad != null)
        {
            virtualGamePad = Instantiate(m_VirtualGamePad);
        }
        else
        {
            virtualGamePad = FindObjectOfType<VirtualGamePad>();
        }

        ShipInputController shipInputController = null;

        if (m_ShipInputController != null)
        {
            shipInputController = Instantiate(m_ShipInputController);
        }
        else
        {
            shipInputController = FindObjectOfType<ShipInputController>();
        }

        if (shipInputController != null)
            shipInputController.Construct(virtualGamePad);

        Transform spawnPoint = m_SpawnPoint != null
            ? m_SpawnPoint
            : (reusedExistingPlayer ? player.transform : transform);

        player.Construct(followCamera, shipInputController, spawnPoint);

        return player;
    }

    private static void WireBackground(FollowCamera followCamera)
    {
        if (followCamera == null) return;

        Camera cameraComponent = followCamera.GetComponent<Camera>();

        if (cameraComponent == null)
            cameraComponent = followCamera.GetComponentInChildren<Camera>();

        Transform target = cameraComponent != null ? cameraComponent.transform : followCamera.transform;

        SyncTransform[] syncTransforms = FindObjectsOfType<SyncTransform>();

        for (int i = 0; i < syncTransforms.Length; i++)
        {
            if (syncTransforms[i] != null)
                syncTransforms[i].SetTarget(target);
        }
    }
}
