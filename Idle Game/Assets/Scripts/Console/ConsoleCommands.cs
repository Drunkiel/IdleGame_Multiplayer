using UnityEngine;

public class ConsoleCommands : MonoBehaviour
{
    [SerializeField] private ConsoleController _consoleController;

    public void Clear()
    {
        foreach (Message singleMessage in _consoleController.messages)
            Destroy(singleMessage.messageText.gameObject);
        _consoleController.messages.Clear();
    }

    public void Say(string text)
    {
        _consoleController.ChatMessage(SenderType.System, text);
    }

    public void GetItem(string itemName)
    {
        ItemContainer.instance.GetItemByName(itemName);
    }

    public void KillEvent(string id)
    {
        if (!short.TryParse(id, out short targetID))
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"'<color=yellow>{id}</color>' can't be parsed to number", OutputType.Error);
            return;
        }

        QuestController.instance.InvokeKillEvent(targetID);
    }

    //Teleport
    public void Tp(string sceneName)
    {
        PortalController.instance.TeleportToScene(sceneName, Vector3.up);
    }
}