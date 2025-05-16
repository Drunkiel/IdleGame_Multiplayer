using UnityEngine;

public class OpenCloseUI : MonoBehaviour
{
    public bool isOpen;
    public GameObject UI;

    public void OpenClose()
    {
        isOpen = !isOpen;
        UI.SetActive(isOpen);
    }

    public void Open()
    {
        isOpen = true;
        UI.SetActive(true);
    }

    public void Close()
    {
        isOpen = false;
        UI.SetActive(false);
    }
}
