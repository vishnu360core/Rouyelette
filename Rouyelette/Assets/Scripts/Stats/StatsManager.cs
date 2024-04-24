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
       // Actions.ResetAction += ResetAction;
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
        stat.GetComponent<RectTransform>().localScale = Vector3.one;

        stat.GetComponent<Stats>().UpdateStat(type,number);
    }
}
