using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

using Best.HTTP;

public  class APIHandler : MonoBehaviour
{

    private static APIHandler instance;

    public static APIHandler Instance {  get { return instance; } }

    Action<string> _onSuccess;
    Action<string> _onError;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    public  void GetSlot(string url, Action<string> onSuccess,Action<string> OnError)
    {
        //_onSuccess = onSuccess; ;
        //_onError = OnError; 

        StartCoroutine(GetRequest(url, onSuccess, OnError));
        //var request = new HTTPRequest(new Uri(url),HTTPMethods.Get, OnRequestFinished);

        //request.Send();
    }


    void OnRequestFinished(HTTPRequest req, HTTPResponse response)
    {
        if (response.IsSuccess)
        {
            _onSuccess.Invoke(response.DataAsText);
        }
        else
           _onError.Invoke(response.DataAsText);
    }

    IEnumerator GetRequest(string url, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {

            webRequest.SetRequestHeader("Access-Control-Allow-Origin", url);
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");
            webRequest.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type");
            webRequest.SetRequestHeader("strict-origin-when-cross-origin", "true");


            //webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");
            //webRequest.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            //webRequest.SetRequestHeader("Access-Control-Allow-Methods" ,"GET, POST, OPTIONS");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            else
                onError?.Invoke(webRequest.error);
        }
    }
}


[System.Serializable]
public class ResponseData
{
   public int result;
   public string hash;
   public int blockNumber;
}

