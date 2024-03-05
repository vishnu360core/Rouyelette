using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public  class APIHandler : MonoBehaviour
{

    private static APIHandler instance;

    public static APIHandler Instance {  get { return instance; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    public  void GetSlot(string url, Action<string> onSuccess,Action<string> OnError)
    {
       StartCoroutine(GetRequest(url, onSuccess, OnError));
    }

    IEnumerator GetRequest(string url, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
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

