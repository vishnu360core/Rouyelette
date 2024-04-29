using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;

public class WalletConnector : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectWalletAndRetrieveAddress();

    [DllImport("__Internal")]
    private static extern void Deduct(string address, string amountInEther);

    [DllImport("__Internal")]
    private static extern void Credit(string address, string amountInEther);


    [SerializeField] GameObject _walletConnectPanel;

    string walletAddress;
    float walletBalance;

    public bool _walletConneted = false;

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
        Credit(walletAddress, mat);
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

        StartCoroutine(Network.Instance.SendWallet(address));

        _walletConnectPanel.SetActive(false);
        _walletConneted = true;
    }


    public void Deduct_Rejected()
    {
        Actions.DeductionRejected();
    }
}
