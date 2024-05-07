using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Actions 
{
    public static Action ballHit;

    public static Action StoppedSpin;

    public static Action EndedSpinAction;

    public static Action<bool> EnableSlotSetectAction;

    public static Action ReachedDestination;

    public static Action<Slot> OnSlotAction;

    public static Action<Slot> MoveTowardTarget;

    public static Action ResetAction;

    public static Action<bool, Slot.BoardSlotMethod> EnableHoverAction;

    public static Action<int> ChipSelectAction;

    public static Action ResetHoverAction;

    public static Action<Slot.BoardSlotMethod> BoardHoverAction;

    public static Action<Slot> BoardSlotAction;

    public static Action<Transform> SetBallTarget;

    public static Action BoardSelectAction;

    public static Action <bool> EnablePlay;

    public static Action <Transform> DealerMoveAction;

    public static Action <bool> EnableObstacles;

    public static Action <bool> DealerSet;

    public static Action <string> GetGameData;

    public static Action <Bet> PlayerBets;

    public static Action<string> AddClient;

    public static Action<int> timerIndex;

    public static Action StartRoll;

    public static Action<float> GetWalletBalance;

    public static Action<int> BetData;

    public static Action <int,Slot.ColorType> ReadStats ;

    public static Action<int[]> ReadHistoryStat;

    public static Action<string> DeleteClient;

    public static Action <int> WinBetAction;

    public static Action<int> LossBetAction;

    public static Action<string> Credit_MAT;

    public static Action<string> Deduct_MAT;


    public static Action DeductionRejected;

    public static Action DeleteChip;

    public static Action<string> walletAddress;


    #region TABLE

    public static Action<string> TableStatus;

    #endregion
}
