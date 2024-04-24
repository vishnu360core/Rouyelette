using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    [SerializeField] TMP_Text red_Bet;
    [SerializeField] TMP_Text black_Bet;

  
    public void UpdateStat(Slot.ColorType type, int number)
    {
        Debug.LogWarning("Stat Number >>>" + number);

        switch (type) 
        {
            case Slot.ColorType.red:
                red_Bet.text = number.ToString();
                black_Bet.text = "";

                break;

            case Slot.ColorType.black:
                black_Bet.text = number.ToString();
                red_Bet.text = "";

                break;
        }
    }
}
