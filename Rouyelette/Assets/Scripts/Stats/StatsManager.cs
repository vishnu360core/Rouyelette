using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class StatsManager : MonoBehaviour
{
    [Header("StatPrefab:")]
    [SerializeField] GameObject _betStatPrefab;

    [Header("Content:")]
    [SerializeField] Transform _content;

    List<Stats> Stats = new List<Stats>();


    private void OnEnable()
    {
        Actions.ReadStats += GetStatsAction;
        Actions.ReadHistoryStat += GetStatHistoryAction;
        // Actions.ResetAction += ResetAction;
    }

    private void GetStatHistoryAction(int[] bets)
    {
        for (int i = 0; i < bets.Length; i++) 
        { 
           Slot.ColorType colorType = GetColorCode(bets[i]);
           GetStatsAction(bets[i], colorType);
        }

        Actions.BetData(bets[bets.Length - 1]);
    }

    Slot.ColorType GetColorCode(int index) 
    {
                if (index == 0
                 || index == 1
                 || index == 3
                 || index == 5
                 || index == 7
                 || index == 9
                 || index == 12
                 || index == 14
                 || index == 16
                 || index == 18
                 || index == 18
                 || index == 19
                 || index == 21
                 || index == 23
                 || index == 25
                 || index == 27
                 || index == 30
                 || index == 32
                 || index == 34
                 || index == 36)
        {
            return Slot.ColorType.red;
        }
        else return Slot.ColorType.black;
    
    }


    private void ResetAction()
    {
        for (int i = _content.childCount - 1; i >= 0; i--)
        {
            Destroy(_content.GetChild(i).gameObject);
        }

        Stats.Clear();
    }

    private void GetStatsAction(int number, Slot.ColorType type)
    {
        GameObject stat = Instantiate(_betStatPrefab);

        
        stat.GetComponent<Transform>().parent = _content;

        RectTransform rect = stat.GetComponent<RectTransform>();

        rect.localScale = Vector3.one;
        rect.rotation = new Quaternion(0,0,0,0);
        rect.localPosition = Vector3.zero;

        stat.GetComponent<Stats>().UpdateStat(type,number);
    }
}
