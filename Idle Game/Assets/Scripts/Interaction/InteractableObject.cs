using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    private bool canInteract = true;

    public bool isTriggerNearby;
    public UnityEvent onClickFunctionalities;
    public UnityEvent onHoldFunctionalities;
    public UnityEvent onEndClickFunctionalities;

    private TriggerController _triggerController;
    [SerializeField] private InputActionAsset inputActions;

    private void Start()
    {
        _triggerController = GetComponent<TriggerController>();

        if (TryGetComponent(out PlayerInput _input))
            _input.actions = inputActions;
    }

    private void Update()
    {
        if (InteractionSystem.isInteracting)
            return;

        isTriggerNearby = _triggerController.isTriggered;
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (!canInteract)
            return;

        if (onClickFunctionalities.GetPersistentEventCount() > 0 && isTriggerNearby && context.performed)
            StartOnClickInteraction();
    }

    public void Hold(InputAction.CallbackContext context)
    {
        if (!canInteract)
            return;

        if (isTriggerNearby && context.performed)
            StartOnHoldInteraction();
    }

    public void End(InputAction.CallbackContext context)
    {
        if (isTriggerNearby && context.canceled)
            StartOnEndInteraction();
    }

    private void StartOnClickInteraction()
    {
        onClickFunctionalities.Invoke();
    }

    private void StartOnHoldInteraction()
    {
        onHoldFunctionalities.Invoke();
    }

    private void StartOnEndInteraction()
    {
        onEndClickFunctionalities.Invoke();
    }

    public void Deactivate()
    {
        canInteract = false;
    }
}
