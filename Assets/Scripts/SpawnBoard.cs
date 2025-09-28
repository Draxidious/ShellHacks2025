using UnityEngine;

using Unity.Netcode;

public class SpawnBoard : NetworkBehaviour
{
    [Header("Spawnable")]
    [SerializeField] private NetworkObject spawnPrefab;

    [SerializeField] private Transform rightHandAnchor;

    public override void OnNetworkSpawn()
    {

    }

    private bool PrimaryActionPressed()
    {
        try
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                return true;
        }
        catch { }
        return false;
    }


    private void Update()
    {
        if (!IsOwner) return;


        if (PrimaryActionPressed())
        {
            RequestSpawnServerRpc(rightHandAnchor.position, rightHandAnchor.rotation);
        }
    }

    [ServerRpc]
    private void RequestSpawnServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams rpc = default)
    {
        if (spawnPrefab == null) { Debug.LogError("SpawnBoard: spawnPrefab not assigned."); return; }

        Instantiate(spawnPrefab, position, rotation).Spawn(true);
    }
}
