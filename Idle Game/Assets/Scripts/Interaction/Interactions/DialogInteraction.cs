using UnityEngine;

public class DialogInteraction : MonoBehaviour
{
    public int dialogIndex;

    public void StartDialog()
    {
        DialogController.instance.StartDialog(dialogIndex);
    }
}
