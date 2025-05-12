using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TeleportEvent : MonoBehaviour
{
    [SerializeField] private float cooldownToTeleport = 2f;
    public Vector3[] positions;
    [SerializeField] private Animator anim;

    public UnityEvent onTeleportEvent;

    public void TeleportToPosition(int positionIndex)
    {
        PortalController _portalController = PortalController.instance;

        //if (_portalController.IsOnCooldown() || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
        //    return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            onTeleportEvent.Invoke();
            PortalController.instance.TeleportToPosition(positions[positionIndex]);
            _portalController.SetCooldown();
        }));
    }

    public void TeleportToObject(Transform objectTransform)
    {
        PortalController _portalController = PortalController.instance;

        //if (_portalController.IsOnCooldown() || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
        //    return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            _portalController.SetCooldown();

            onTeleportEvent.Invoke();
            PortalController.instance.TeleportToObject(objectTransform);
        }));
    }

    public void TeleportToScene(string sceneName)
    {
        PortalController _portalController = PortalController.instance;

        //if (_portalController.IsOnCooldown() || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
        //    return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            //Basic string verificacion
            if (string.IsNullOrEmpty(sceneName))
            {
                //ConsoleController.instance.ChatMessage(SenderType.System, $"Scene named: {sceneName} is not found");
                return;
            }

            //Check if position exists
            if (positions.Length < 1)
                positions = new Vector3[1] { Vector3.zero };

            onTeleportEvent.Invoke();
            PortalController.instance.TeleportToScene(sceneName, positions[0]);
            _portalController.SetCooldown();
        }));
    }

    public void TeleportToPrevScene()
    {
        PortalController _portalController = PortalController.instance;

        //if (_portalController.IsOnCooldown() || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
        //    return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            onTeleportEvent.Invoke();
            PortalController.instance.TeleportToPrevScene();
            _portalController.SetCooldown();
        }));
    }

    private IEnumerator PauseBeforeTeleport(Action action)
    {
        yield return new WaitForSeconds(cooldownToTeleport * 0.1f);
        if (anim != null)
            anim.Play("Teleport");
        //PlayerController.instance.isStopped = true;
        yield return new WaitForSeconds(cooldownToTeleport * 0.9f);
        action();
    }
}
