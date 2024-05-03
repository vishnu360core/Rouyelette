using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryController : MonoBehaviour
{
    static HistoryController instance;

    public static HistoryController Instance { get { return instance; } }


    [SerializeField] GameObject _historyPanel;
    [SerializeField] GameObject _historyObject;

    [SerializeField] Transform _historyParent;

    List<History> histories = new List<History>();

    public int bets;
    public int profit;
    public int block;
    public string hash;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        
    }


    public void EnableHistory(bool enable)
    {
        _historyPanel.SetActive(enable);
    }


    public void HistoryGenerateAction()
    {
        int id  = histories.Count + 1;

        GameObject histObj = Instantiate(_historyObject, _historyParent);

        History history = histObj.GetComponent<History>();
        
        history.IntialiseValues(id,bets,profit,block,hash);

        histories.Add(history);

        bets = 0;
        profit = 0;
        block = 0;
        hash = "";
    }
}

[System.Serializable]
public class HistoryStat
{
    public int bets;
    public int profit;
    public int block;
    public string hash;
}
