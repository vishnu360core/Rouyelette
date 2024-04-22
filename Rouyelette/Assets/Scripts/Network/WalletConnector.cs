using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using System;

public class WalletConnector : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectWalletAndRetrieveDetails();

    [DllImport("__Internal")]
    private static extern void GetWalletBalance(string address, string rpcUrl);

    string walletAddress;

    string rpcURL = "https://polygon-mumbai.g.alchemy.com/v2/c8QEf5b218YqVWVlX2t3RBRI-febo0tH";

    void Start()
    {
        // Call the JavaScript function to connect the wallet and retrieve details
        ConnectWalletAndRetrieveDetails();
    }

    // This method will be called by JavaScript to receive the wallet details
    public void ReceiveWalletDetails(string account)
    {
        Debug.Log("Wallet Address: " + account);
        walletAddress = account;

        Actions.GetWalletBalance += ReceviedWalletBalance;

       // StartCoroutine(GetBalanceCoroutine());
        // Handle the received wallet details here
    }

    private void ReceviedWalletBalance(string obj)
    {
        Debug.Log("Balance >>>" + obj);
    }
}
