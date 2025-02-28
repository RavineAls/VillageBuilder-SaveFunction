using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public class UserAccount
{
    public string Username;
    public string Password;

    public UserAccount(string uName, string pass)
    {
        Username = uName;
        Password = pass;
    }
}

[Serializable]
public class UserAccountsData
{
    public List<UserAccount> userAccounts;
}

public class LoginManager : MonoBehaviour
{
    public TMP_InputField loginUName, loginPass, signupUName, signupPass, signupPassVerifivation;
    public GameObject errorMessage;
    public SaveSystem saveSystem;
    private UserAccountsData accountsData = new UserAccountsData{userAccounts = new List<UserAccount>()};
    UserAccount LoggedAcc = new UserAccount("","");

    // Start is called before the first frame update
    void Start()
    {
        errorMessage.SetActive(false);
        PlayerPrefs.SetInt("LoggedIn", 0);
        LoadUserAccounts();
        
        if (accountsData.userAccounts.Count > 0)
        {
            Debug.Log("Loaded " + accountsData.userAccounts.Count + " Accounts");
        } else
        {
            Debug.Log("No users found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Game Keluar");
        }
    }

    public void Login()
    {
        string uName = loginUName.text;
        string pass = loginPass.text;
        LoadUserAccounts();

        UserAccount user = accountsData.userAccounts.Find(u => u.Username == uName && u.Password == pass);
        if(user != null)
        {
            Debug.Log("Login Success");
            LoggedAcc = user;
            PlayerPrefs.SetString("Username", LoggedAcc.Username);
            PlayerPrefs.SetInt("LoggedIn", 1);
            SceneManager.LoadScene(1);
        }
        else
        {
            ShowError("Login gagal");
        }
    }
    
    public void Signup()
    {
        string uName = signupUName.text;
        string pass = signupPass.text;
        string passVer = signupPassVerifivation.text;

        UserAccount user = accountsData.userAccounts.Find(u => u.Username == uName);
        if(string.IsNullOrEmpty(uName) || string.IsNullOrEmpty(pass))
        {
            ShowError("Username dan/atau password tidak bisa kosong");
        }
        else if(user != null)
        {
            ShowError("Username sudah digunakan");
        } 
        else if(pass != passVer)
        {
            ShowError("Password dan verifikasi tidak cocok");
        } 
        else 
        {
            UserAccount newUser = new UserAccount(uName, pass);
            accountsData.userAccounts.Add(newUser);
            SaveUserAccounts();

            ShowError("Akun baru ditambahkan, username : " + uName);
            Debug.Log("Added New User, username= " + uName + ", password = " + pass);
            
            signupUName.text = "";
            signupPass.text = "";
            signupPassVerifivation.text = "";
        }
    }

    public void ShowError(string errorString)
    {
        errorMessage.SetActive(true);
        errorMessage.GetComponentInChildren<Text>().text = errorString;
    }

    public void CloseError()
    {
        errorMessage.SetActive(false);
    }

    private void SaveUserAccounts()
    {
        string json = JsonUtility.ToJson(accountsData);
        Debug.Log(json);
        saveSystem.SaveAccList(json);
    }
    
    private void LoadUserAccounts()
    {
        string json = "";
        json = saveSystem.LoadAccList();
        if (String.IsNullOrEmpty(json) == false)
        {
            accountsData = JsonUtility.FromJson<UserAccountsData>(json);
            Debug.Log("Loaded JSON: " + json);
        }
        else
        {
            accountsData.userAccounts = new List<UserAccount>();
            Debug.Log("JSON file not found. Starting with empty user list.");
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    /// 
    private void OnDisable()
    {
        Debug.Log("Logged user is "+LoggedAcc.Username);
    }
}
