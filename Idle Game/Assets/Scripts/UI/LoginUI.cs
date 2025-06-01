using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

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

    // REGEX
    private readonly Regex usernameRegex = new(@"^[a-zA-Z0-9_]{5,20}$"); // bez spacji, bez znak�w specjalnych
    private readonly Regex passwordRegex = new(@"^(?=.*[A-Z])(?=.*\d).{8,}$"); // min. 8 znak�w, 1 wielka litera, 1 cyfra

    public bool CanLogIn()
    {
        string username = loginUsernameInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;

        //if (!usernameRegex.IsMatch(username))
        //{
        //    Debug.LogWarning("Nieprawid�owa nazwa u�ytkownika (login).");
        //    return false;
        //}

        //if (!passwordRegex.IsMatch(password))
        //{
        //    Debug.LogWarning("Has�o (login) nie spe�nia wymaga�.");
        //    return false;
        //}

        return true;
    }

    public void OpenLogPage()
    {
        loginPage.SetActive(true);
        registrationPage.SetActive(false);
    }

    public bool CanReg()
    {
        string username = registerUsernameInput.text.Trim();
        string password = registerPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;

        //if (!usernameRegex.IsMatch(username))
        //{
        //    Debug.LogWarning("Nieprawid�owa nazwa u�ytkownika (rejestracja).");
        //    return false;
        //}

        //if (!passwordRegex.IsMatch(password))
        //{
        //    Debug.LogWarning("Has�o (rejestracja) nie spe�nia wymaga�.");
        //    return false;
        //}

        return true;
    }

    public void OpenRegPage()
    {
        loginPage.SetActive(false);
        registrationPage.SetActive(true);
    }
}
