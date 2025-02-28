using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeleteLoggedAcc : MonoBehaviour
{
    public SaveSystem saveSystem;
    private UserAccountsData accountsData = new UserAccountsData{userAccounts = new List<UserAccount>()};
    UserAccount LoggedAcc = new UserAccount("","");
    public UIController uIController;
    
    public void DeleteLoggedAccount()
    {
        LoadUserAccounts();
        string uName = PlayerPrefs.GetString("Username");
        UserAccount delete = accountsData.userAccounts.Find(u => u.Username == uName);
        accountsData.userAccounts.RemoveAll(u => u.Username == uName);
        Debug.Log("Deleted User: "+delete.Username);
        SaveUserAccounts();
        uIController.YesQuit();
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

    private void SaveUserAccounts()
    {
        string json = JsonUtility.ToJson(accountsData);
        Debug.Log(json);
        saveSystem.SaveAccList(json);
    }
}
