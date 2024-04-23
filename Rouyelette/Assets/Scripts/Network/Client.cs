using DataCollector;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class Client : MonoBehaviour
{
    public PlayerData PlayerData ;

    public Transform _chipTransform;

    public bool taken = false;

    [Header("Player:")]
    [SerializeField] GameObject PlayerObject;
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _bet;

    public void EnablePlayer(bool enable)
    {
        PlayerObject.SetActive(enable);
    }

    public void UpdateName(string name)
    {
       _name.text = name;   
    }

    public void UpdateBet(int bets)
    {
        _bet.text = "Total Bets:" + bets.ToString();
    }
}
