using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;
using Thirdweb;
using Thirdweb.Examples;

public class WalletConnector : MonoBehaviour
{
    static WalletConnector instance;
    public static WalletConnector Instance { get { return instance; } }


    [DllImport("__Internal")]
    private static extern void Deduct(string address, string amountInEther);

    [DllImport("__Internal")]
    private static extern void Credit(string address, string amountInEther);

    [DllImport("__Internal")]
    private static extern void Copy(string text);


    [SerializeField] GameObject _walletConnectPanel;

    [SerializeField] TMP_Text walletText;
    [SerializeField] TMP_Text walletAddressText;

    [Header("Third-Web Wallet")]
    [SerializeField] Prefab_ConnectWallet connectWallet;

    string walletAddress;
    float walletBalance;

    public bool _walletConneted = false;

    string abi = @" [
	{
		""inputs"": [],
		""name"": ""placebid"",
		""outputs"": [],
		""stateMutability"": ""payable"",
		""type"": ""function""
	},
	{
		""inputs"": [
			{
				""internalType"": ""address payable"",
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""internalType"": ""uint256"",
				""name"": ""_amount"",
				""type"": ""uint256""
			}
		],
		""name"": ""rewardFunc"",
		""outputs"": [],
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""inputs"": [
			{
				""internalType"": ""address payable"",
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""internalType"": ""uint256"",
				""name"": ""_amount"",
				""type"": ""uint256""
			}
		],
		""name"": ""withdrawFunc"",
		""outputs"": [],
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""inputs"": [],
		""name"": ""checkBalance"",
		""outputs"": [
			{
				""internalType"": ""uint256"",
				""name"": """",
				""type"": ""uint256""
			}
		],
		""stateMutability"": ""view"",
		""type"": ""function""
	}
]";

    Contract contract;


    private void Awake()
    {
        if(instance == null)
          instance = this;

        DontDestroyOnLoad(this.gameObject);

        _walletConneted = false;
        // Call the JavaScript function to connect the wallet and retrieve details
        // ConnectWalletAndRetrieveAddress();

        Actions.GetWalletBalance += WalletBalance;

        Actions.Deduct_MAT += DeductWallet;
        Actions.Credit_MAT += CreditWallet;

        connectWallet.onConnected.AddListener(ReceiveWalletAddressAndBalance);
    }

     void Start()
     {
        var sdk = ThirdwebManager.Instance.SDK;

       contract = sdk.GetContract("0xd7059957411ad31a0453bba8de7371D0b9f096d5", abi);

     }

    private async void CreditWallet(string mat)
    {
        TransactionResult result = null;

        try
        {
            result = await contract.Write("rewardFunc", walletAddress, mat);
            Debug.LogWarning("Credit :" + result.ToString());

            StartCoroutine(Network.Instance.SendWallet(walletAddress));
        } 
        catch (Exception ex) 
        {
            if (result != null)
                return;

            Debug.Log("Credit Rejected !!!");

            StartCoroutine(Network.Instance.SendWallet(walletAddress));
        }
    }

    private async void DeductWallet(string mat)
    {
        Debug.LogWarning("Deduction happened !!!!!!!" + mat);

        TransactionResult result = null;

        try
        {
            result = await contract.Write("placebid", new TransactionRequest() { value = mat, gasLimit = "100000" });

            Debug.LogWarning("Deduct :" + result.ToString());

            StartCoroutine(Network.Instance.SendWallet(walletAddress));
        }
        catch (Exception ex) 
        {
            if (result != null)
                return;

            Deduct_Rejected();

            StartCoroutine(Network.Instance.SendWallet(walletAddress));
        }

       // StartCoroutine(Network.Instance.SendWallet(walletAddress));

        //Deduct(walletAddress, mat);
        // StartCoroutine(Network.Instance.SendWallet(walletAddress));
    }

    private void WalletBalance(float balance)
    {
       walletBalance = balance;
    }

    public void ReceiveWalletAddressAndBalance(string address)
    {
       

        Debug.Log("Wallet Address: " + address);

        walletAddress = address;

        Debug.Log(walletAddress.Length + " " + address.Length);

        string addressFirst = address.Substring(0,4);
        string addressLast = address.Substring(address.Length-4,4);

        string addressPrint = addressFirst +"...." + addressLast;

        //walletAddressText.text = "Wallet address: " + addressPrint;
        Network.Instance.SetId(addressPrint);

        Actions.walletAddress(address);

        //StartCoroutine(Network.Instance.SendWallet(address));

        _walletConnectPanel.SetActive(false);
        _walletConneted = true;
    }

    public void ShowWalletAddress(string addressPrint)
    {
        walletAddressText.text = addressPrint;
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
