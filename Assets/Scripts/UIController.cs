using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement, OnBigStructurePlacement, OnDestroySelection, OnCloseMenu, onMainInactive, onMainActive;
    public Button placeRoadButton, placeHouseButton, placeSpecialButton, placeBigStructureButton, destroyButton, openBuildMenuButton, closeBuildMenuButton, openMToConvMenuButton, openTToConvMenuButton, closeConversionMenuButton, openMToTransMenuButton, openCToTransMenuButton, closeTransactionMenuButton, confTrue, confFalse;
    public GameObject buildingMenuPanel, mainButtons, mainCanvas, ConversionCanvas, TransactionCanvas, accountName, errorMessage, quitPanel, confirmationPanel, deletePanel;
    public Color outlineColor;
    public DeleteLoggedAcc deleteLoggedAcc;
    List<Button> buildButtonList;

    private void Start()
    {
        if(PlayerPrefs.HasKey("LoggedIn") && PlayerPrefs.GetInt("LoggedIn") == 1)
        {
            accountName.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Username");
            Debug.Log("Account name is: " + PlayerPrefs.GetString("Username"));
        }else
        {
            accountName.GetComponentInChildren<Text>().text = "Dio Brando";
            Debug.Log("You are not logged in to an account");
        }

        buildButtonList = new List<Button> { placeHouseButton, placeRoadButton, placeSpecialButton, placeBigStructureButton, destroyButton };
        
        mainCanvas.SetActive(true);
        errorMessage.SetActive(false);
        buildingMenuPanel.SetActive(false);
        ConversionCanvas.SetActive(false);
        TransactionCanvas.SetActive(false);
        quitPanel.SetActive(false);
        confirmationPanel.SetActive(false);
        deletePanel.SetActive(false);

        openMToConvMenuButton.onClick.AddListener(() =>
        {
            mainButtons.SetActive(true);
            buildingMenuPanel.SetActive(false);
            ConversionCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            onMainInactive?.Invoke();
            ResetButtonColor();
        });

        openTToConvMenuButton.onClick.AddListener(() =>
        {
            ConversionCanvas.SetActive(true);
            TransactionCanvas.SetActive(false);
            ResetButtonColor();
        });

        closeConversionMenuButton.onClick.AddListener(() =>
        {
            mainCanvas.SetActive(true);
            ConversionCanvas.SetActive(false);
            mainButtons.SetActive(true);
            buildingMenuPanel.SetActive(false);
            onMainActive?.Invoke();
            ResetButtonColor();
        });

        openMToTransMenuButton.onClick.AddListener(() =>
        {
            mainButtons.SetActive(true);
            buildingMenuPanel.SetActive(false);
            TransactionCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            onMainInactive?.Invoke();
            ResetButtonColor();
        });

        openCToTransMenuButton.onClick.AddListener(() =>
        {
            TransactionCanvas.SetActive(true);
            ConversionCanvas.SetActive(false);
            ResetButtonColor();
        });

        closeTransactionMenuButton.onClick.AddListener(() =>
        {
            mainCanvas.SetActive(true);
            TransactionCanvas.SetActive(false);
            mainButtons.SetActive(true);
            buildingMenuPanel.SetActive(false);
            onMainActive?.Invoke();
            ResetButtonColor();
        });

        openBuildMenuButton.onClick.AddListener(() =>
        {
            buildingMenuPanel.SetActive(true);
            mainButtons.SetActive(false);
        });

        closeBuildMenuButton.onClick.AddListener(() =>
        {
            mainButtons.SetActive(true);
            buildingMenuPanel.SetActive(false);
            OnCloseMenu?.Invoke();
            ResetButtonColor();
        });

        placeRoadButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeRoadButton);
            OnRoadPlacement?.Invoke();

        });
        placeHouseButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeHouseButton);
            OnHousePlacement?.Invoke();

        });
        placeSpecialButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeSpecialButton);
            OnSpecialPlacement?.Invoke();

        });
        placeBigStructureButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeBigStructureButton);
            OnBigStructurePlacement?.Invoke();

        });
        destroyButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(destroyButton);
            OnDestroySelection?.Invoke();

        });
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            quitPanel.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            deletePanel.SetActive(true);
        }
    }

    private void ModifyOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    public void ResetButtonColor()
    {
        foreach (Button button in buildButtonList)
        {
            button.GetComponent<Outline>().enabled = false;
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

    private TaskCompletionSource<bool> confirmationTCS;
    public Task<bool> ShowConfirmation(string confirmString)
    {
        confirmationTCS = new TaskCompletionSource<bool>();
        confirmationPanel.SetActive(true);
        confirmationPanel.GetComponentInChildren<Text>().text = confirmString;
        return confirmationTCS.Task;
    }
    
    public void OnConfirmationYes()
    {
        confirmationTCS?.SetResult(true);
        confirmationPanel.SetActive(false);
    }

    public void OnConfirmationNo()
    {
        confirmationTCS?.SetResult(false);
        confirmationPanel.SetActive(false);
    }

    public void NoQuit()
    {
        quitPanel.SetActive(false);
    }

    public void YesQuit()
    {
        SceneManager.LoadScene(0);
    }

    public void NoDelete()
    {
        deletePanel.SetActive(false);
    }

    public void YesDelete()
    {
        deleteLoggedAcc.DeleteLoggedAccount();
    }
}
