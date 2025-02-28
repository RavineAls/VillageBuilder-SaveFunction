using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public string saveName = "SaveData_";
    public string AccListName = "UserAccounts";
    public UIController uIController;

    private string GetExecutableFolderPath()
    {
        // Application.dataPath points to the Data folder inside the game executable folder.
        return Directory.GetParent(Application.dataPath).FullName;
    }
    
    private string GetSaveFolderPath()
    {
        string saveFolder = Path.Combine(GetExecutableFolderPath(), "Saves");
        
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
            Debug.Log("Created path: " + saveFolder);
        }
        return saveFolder;
    }

    public void SaveData(string dataToSave)
    {
        string fullSaveName = saveName + PlayerPrefs.GetString("Username");
        if(WriteToFile(fullSaveName, dataToSave))
        {
            Debug.Log("Successfully saved data to " + fullSaveName);

        }
    }

    public void SaveAccList(string dataToSave)
    {
        if(WriteToFile(AccListName, dataToSave))
        {
            Debug.Log("Successfully saved Account List");
        }
    }

    public string LoadData()
    {
        string data = "";
        if(ReadFromFile(saveName+PlayerPrefs.GetString("Username"), out data))
        {
            Debug.Log("Successfully loaded data");
        }
        return data;
    }

    public void DeleteData()
    {
        string fullSaveName = saveName + PlayerPrefs.GetString("Username");
        if(DeleteFile(fullSaveName))
        {
            Debug.Log("File succesfully Deleted");
        }

        return;
    }

    public string LoadAccList()
    {
        string data = "";
        if(ReadFromFile(AccListName, out data))
        {
            Debug.Log("Successfully loaded Account List");
        }
        return data;
    }

    private bool WriteToFile(string name, string content)
    {
        var fullPath = Path.Combine(GetSaveFolderPath(), name);

        try
        {
            File.WriteAllText(fullPath, content);
            return true;
        }
        catch (Exception e)
        {
            uIController.ShowError("Error saving to a file " + e.Message);
        }
        return false;
    }

    private bool ReadFromFile(string name, out string content)
    {
        var fullPath = Path.Combine(GetSaveFolderPath(), name);
        try
        {
            content = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            uIController.ShowError("Error when loading the file " + e.Message);
            content = "";
        }
        return false;
    }

    private bool DeleteFile(string name)
    {
        var fullPath = Path.Combine(GetSaveFolderPath(), name);
        try
        {
            File.Delete(Path.Combine(fullPath));
            return true;
        }
        catch (Exception e)
        {
            uIController.ShowError("Error when deleting the file " + e.Message);
        }
        return false;
    }
}
