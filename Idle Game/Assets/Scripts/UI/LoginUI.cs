using TMPro;
using UnityEngine;

public class LoginUI : MonoBehaviour
{
    [Header("Login")]
    public GameObject loginPage;
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    [Header("Register")]
    public GameObject registrationPage;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_Dropdown dropdownHeroClass;

    public bool CanLogIn()
    {
        if (string.IsNullOrEmpty(loginUsernameInput.text) || string.IsNullOrEmpty(loginPasswordInput.text))
            return false;

        return true;
    }

    public void OpenLogPage()
    {
        loginPage.SetActive(true);
        registrationPage.SetActive(false);
    }

    public void OpenRegPage()
    {
        loginPage.SetActive(false);
        registrationPage.SetActive(true);
    }
}
