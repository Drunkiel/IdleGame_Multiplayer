using UnityEngine;

public class OpenCloseInteraction : MonoBehaviour
{
    public void Open(int index)
    {
        InteractionSystem.instance.allUI[index].SetActive(true);
    }

    public void Close(int index)
    {
        InteractionSystem.instance.allUI[index].SetActive(false);
    }
}
