using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public UIController uiController;
    public int mGold, rupiah;
    public int goldToBuy, goldToSell, transactionAmmount;
    public int gToRRate = 1000;
    public int rToGRate = 1000;
    public bool isOnConversion = false;
    public bool isOnTransaction = false;
    public GameObject activeGoldUI, activeRpUI, mainGoldUI, mainRpUI, convGoldUI, convRpUI, transGoldUI, transRpUI;
    public GameObject reqRpUI, recRpUI, convGoldRN, convGToR, convRToG, transRpRN, topUpObj, cashOutObj;
    public TMP_InputField buyGoldField, sellGoldField, transactionField;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        buyGoldField.contentType = TMP_InputField.ContentType.IntegerNumber;
        sellGoldField.contentType = TMP_InputField.ContentType.IntegerNumber;
        transactionField.contentType = TMP_InputField.ContentType.IntegerNumber;
        
        SwapToMainUI();
        UpdateResourceUI();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(mGold < 0)
        {
            mGold = 0;
            UpdateResourceUI();
        }

        if(rupiah < 0)
        {
            rupiah = 0;
            UpdateResourceUI();
        }
        */
    }

    public void UpdateResourceUI()
    {
        activeGoldUI.GetComponentInChildren<Text>().text = "Gold = " + mGold.ToString("#,##0");
        activeRpUI.GetComponentInChildren<Text>().text = "Rupiah = " + rupiah.ToString("#,##0");
        if(isOnConversion)
        {
            convGoldRN.GetComponentInChildren<Text>().text = mGold.ToString("#,##0") + " mGold";
            convRToG.GetComponentInChildren<Text>().text = "Rp." + rToGRate.ToString("#,##0") + " per mGold";
            convGToR.GetComponentInChildren<Text>().text = "Rp." + gToRRate.ToString("#,##0") + " per mGold";
        }

        if(isOnTransaction)
        {
            transRpRN.GetComponentInChildren<Text>().text = "Rp. " + rupiah.ToString("#,##0");
        }        
    }

    public void UpdateBuyingField()
    {
        string text = buyGoldField.text;
        int.TryParse(text, out goldToBuy);
        reqRpUI.GetComponentInChildren<Text>().text ="Rp." + (goldToBuy*gToRRate).ToString("#,##0");
    }

    public void UpdateSellingField()
    {
        string text = sellGoldField.text;
        int.TryParse(text, out goldToSell);
        recRpUI.GetComponentInChildren<Text>().text ="Rp." + (goldToSell*rToGRate).ToString("#,##0");
    }

    public void UpdateTransactionField()
    {
        string text = transactionField.text;
        int.TryParse(text, out transactionAmmount);
    }

    public async void AddRupiah()
    {
        if(transactionAmmount < 0)
        {
            uiController.ShowError("Jumlah transaksi tidak bisa kurang dari 0");
        }
        else 
        {
            bool confState = await uiController.ShowConfirmation("Apakah anda ingin melakukan Top-Up sebesar "+transactionAmmount.ToString("#,##0")+" rupiah?");
            if (confState)
            {
                rupiah += transactionAmmount;
                UpdateResourceUI();
                Debug.Log("Topped Up "+transactionAmmount.ToString("#,##0")+" rupiah");
                transactionField.text = "";
                gameManager.SaveGame();
            }
        }
    }

    public async void AddGold()
    {
        if(goldToBuy < 0)
        {
            uiController.ShowError("Jumlah konversi tidak bisa kurang dari 0");
        }
        else if(rupiah>=goldToBuy*rToGRate)
        {
            bool confState = await uiController.ShowConfirmation("Apakah anda ingin membeli mGold sebanyak "+goldToBuy.ToString("#,##0")+" seharga "+(goldToBuy*rToGRate).ToString("#,##0")+" rupiah?");
            if (confState)
            {
                mGold += goldToBuy;
                rupiah -= goldToBuy*rToGRate;
                UpdateResourceUI();
                Debug.Log("Bought "+goldToBuy+" mGold");
                buyGoldField.text = "";
                gameManager.SaveGame();
            }
        }
        else
        {
            uiController.ShowError("Uang tidak mencukupi");
        }
    }

    public async void SubsRupiah()
    {
        if(transactionAmmount < 0)
        {
            uiController.ShowError("Jumlah transaksi tidak bisa kurang dari 0");
        }
        else if(rupiah >= transactionAmmount)
        {
            bool confState = await uiController.ShowConfirmation("Apakah anda ingin melakukan Cash-Out sebesar "+transactionAmmount.ToString("#,##0")+" rupiah?");
            if (confState)
            {
                rupiah -= transactionAmmount;
                UpdateResourceUI();
                Debug.Log("Cashed Out "+transactionAmmount.ToString("#,##0")+" rupiah");
                transactionField.text = "";
                gameManager.SaveGame();
            }
        }
        else
        {
            uiController.ShowError("Uang tidak mencukupi");
        }
    }

    public async void SubsGold()
    {
        if(goldToSell < 0)
        {
            uiController.ShowError("Jumlah konversi tidak bisa kurang dari 0");
        }
        else if(mGold >= goldToSell)
        {
            bool confState = await uiController.ShowConfirmation("Apakah anda ingin menjual mGold sebanyak "+goldToSell.ToString("#,##0")+" seharga "+(goldToSell*gToRRate).ToString("#,##0")+" rupiah?");
            if (confState)
            {
                mGold -= goldToSell;
                rupiah += goldToSell*gToRRate;
                UpdateResourceUI();
                Debug.Log("Sold "+goldToSell+" mGold");
                sellGoldField.text = "";
                gameManager.SaveGame();
            }
        }
        else
        {
            uiController.ShowError("mGold tidak mencukupi");
        }
    }

    public void ResetInputField()
    {
        buyGoldField.text = "";
        sellGoldField.text = "";
        transactionField.text = "";
    }

    public void SwapToMainUI()
    {
        activeGoldUI = mainGoldUI;
        activeRpUI = mainRpUI;
        isOnConversion = false;
        isOnTransaction = false;
        UpdateResourceUI();
        ResetInputField();
    }

    public void SwapToConvUI()
    {
        activeGoldUI = convGoldUI;
        activeRpUI = convRpUI;
        isOnConversion = true;
        isOnTransaction = false;
        UpdateBuyingField();
        UpdateSellingField();
        UpdateResourceUI();
        ResetInputField();
    }

    public void SwapToTopUpUI(){
        activeGoldUI = transGoldUI;
        activeRpUI = transRpUI;
        isOnConversion = false;
        isOnTransaction = true;
        UpdateTransactionField();
        UpdateResourceUI();
        topUpObj.SetActive(true);
        cashOutObj.SetActive(false);
        ResetInputField();
    }

    public void SwapToCashOutUI(){
        activeGoldUI = transGoldUI;
        activeRpUI = transRpUI;
        isOnConversion = false;
        isOnTransaction = true;
        UpdateTransactionField();
        UpdateResourceUI();
        cashOutObj.SetActive(true);
        topUpObj.SetActive(false);
        ResetInputField();
    }
}
