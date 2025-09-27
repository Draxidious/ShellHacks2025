using UnityEngine;
using UnityEngine.InputSystem; // Input System
using UnityEngine.EventSystems; // RaycastResult
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SpawnCard : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Vector3 spawnOffset = new Vector3(0, 0.2f, 0);

    public XRRayInteractor rayInteractor;
    public InputActionReference triggerAction;

    void OnEnable()
    {
        triggerAction?.action?.Enable();
    }

    void OnDisable()
    {
        triggerAction?.action?.Disable();
    }

    void Update()
    {
        if (objectToSpawn == null || rayInteractor == null || triggerAction == null) return;

        if (triggerAction.action.WasPressedThisFrame())
        {
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                Instantiate(objectToSpawn, hit.point + spawnOffset, Quaternion.identity);
                return;
            }

            if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult uiHit))
            {
                Instantiate(objectToSpawn, uiHit.worldPosition + spawnOffset, Quaternion.identity);
            }
        }
    }
}
