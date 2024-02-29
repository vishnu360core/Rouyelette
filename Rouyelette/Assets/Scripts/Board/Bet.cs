using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bet
{
   public int betNumber;
   public  int betAmount;

    public int[] splitNumbers;

   public  Slot.BoardSlotMethod type;
}
