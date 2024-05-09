using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;

public class WalletConnector : MonoBehaviour
{
    static WalletConnector instance;
    public static WalletConnector Instance { get { return instance; } }


    [DllImport("__Internal")]
    private static extern void ConnectWalletAndRetrieveAddress();

    [DllImport("__Internal")]
    private static extern void Deduct(string address, string amountInEther);

    [DllImport("__Internal")]
    private static extern void Credit(string address, string amountInEther);

    [DllImport("__Internal")]
    private static extern void Copy(string text);


    [SerializeField] GameObject _walletConnectPanel;

    [SerializeField] TMP_Text walletText;
    [SerializeField] TMP_Text walletAddressText;

    string walletAddress;
    float walletBalance;

    public bool _walletConneted = false;


    private void Awake()
    {
        if(instance == null)
          instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        _walletConneted = false;
        // Call the JavaScript function to connect the wallet and retrieve details
        ConnectWalletAndRetrieveAddress();

        Actions.GetWalletBalance += WalletBalance;

        Actions.Deduct_MAT += DeductWallet;
        Actions.Credit_MAT += CreditWallet;
    }

    private void CreditWallet(string mat)
    {
        Credit (walletAddress, mat);
        //StartCoroutine(Network.Instance.SendWallet(walletAddress));
    }

    private void DeductWallet(string mat)
    {
        Debug.LogWarning("Deduction happened !!!!!!!" + mat);

        Deduct(walletAddress, mat);
       // StartCoroutine(Network.Instance.SendWallet(walletAddress));
    }

    private void WalletBalance(float balance)
    {
       walletBalance = balance;
    }

    public void ReceiveWalletAddressAndBalance(string addressAndBalance)
    {
        string[] parts = addressAndBalance.Split(',');
        string address = parts[0];
        string balance = parts[1];

        Debug.Log("Wallet Address: " + address);

        walletAddress = address;

        Debug.Log(walletAddress.Length + " " + address.Length);

        string addressFirst = address.Substring(0,4);
        string addressLast = address.Substring(address.Length-4,4);

        string addressPrint = addressFirst +"....." + addressLast;

        //walletAddressText.text = "Wallet address: " + addressPrint;
        Network.Instance.SetId(addressPrint);

        Actions.walletAddress(address);

       // StartCoroutine(Network.Instance.SendWallet(address));

        _walletConnectPanel.SetActive(false);
        _walletConneted = true;
    }

    public void ShowWalletAddress(string addressPrint)
    {
        walletAddressText.text = "Wallet address: " + addressPrint;
    }

    public void OnConnectError(string error)
    {
        Debug.Log("Error: " + error);
        
        walletText.text ="Unable to connect the wallet :" +  error;
    }

    public void Deduct_Rejected()
    {
        Actions.DeductionRejected();
    }

    public void Deduct_GetHash(string hashString)
    {
        Debug.LogWarning("Hash >>" + hashString);
        
        HistoryController.Instance.hash = hashString;
    }


    public void CopyTableId()
    {
        Copy(Network.Instance.Tableid);
    }
}
