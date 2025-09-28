using Meta.XR.MRUtilityKit;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using System;
using Meta.XR;

public class SpawnBoard : NetworkBehaviour
{
    [Header("Spawnable")]

    [SerializeField] private Transform rightHandAnchor;

    private EnvironmentRaycastManager raycastManager;

    public OVRHand rightHand; // Ask liam about this if you cant find it

    private bool isPlaced = false;

    public NetworkObject placedBoard;

    private void Awake()
    {
        raycastManager = FindFirstObjectByType<EnvironmentRaycastManager>();
    }

    private void Start()
    {
        //placedBoard.Despawn(true);
    }

    private bool PrimaryActionPressed()
    {
        try
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                //Debug.Log("Pressed");
                return true;
            }
            else if (rightHand && rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
                return true;
        }
        catch { }
        return false;
    }


    public void Update()
    {
        if (!IsOwner) return;

        if (PrimaryActionPressed() && TryGetPlacement(out Vector3 pos, out Quaternion rot))
        {
            if (!isPlaced)
            {
                //Debug.Log("Initial spawn");
                RequestSpawnServerRpc(pos, rot);
                isPlaced = true;
            }
            else
            {
                //Debug.Log("Moved spawn");
                RequestMoveServerRPC(pos, rot);
            }
        }
    }

    private bool TryGetPlacement(out Vector3 pos, out Quaternion rot)
    {
        pos = default; rot = default;

        var ray = new Ray(rightHandAnchor.position, rightHandAnchor.forward);

        if (raycastManager && raycastManager.Raycast(ray, out EnvironmentRaycastHit hit, 12f))
        {
            pos = hit.point;
            Vector3 playerPos = gameObject.transform.position;
            Vector3 toPlayer = playerPos - pos;
            toPlayer.y = 0f;
            toPlayer.Normalize();
            rot = Quaternion.LookRotation(toPlayer, Vector3.up);
            //Debug.Log("Raycast");
            return true;
        }

        return true;
    }

    [ServerRpc]
    private void RequestSpawnServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams rpc = default)
    {
        //placedBoard.Spawn(true);
        //Debug.Log("Actually moved locations");
        //placedBoard.transform.SetPositionAndRotation(position, rotation);
    }
    
    [ServerRpc]
    private void RequestMoveServerRPC(Vector3 position, Quaternion rotation, ServerRpcParams rpc = default)
    {
        //Debug.Log("Also Actually moved locations");
        //placedBoard.transform.SetPositionAndRotation(position, rotation);
    }
}
