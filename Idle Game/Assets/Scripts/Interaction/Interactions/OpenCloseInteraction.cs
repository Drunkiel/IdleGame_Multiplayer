using UnityEngine;

public class OpenCloseInteraction : MonoBehaviour
{
    public void Open(int index)
    {
        UIController.instance.Open(index);
    }

    public void Close(int index)
    {
        UIController.instance.Close(index);
    }
}
