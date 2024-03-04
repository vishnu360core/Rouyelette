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

    public static Action<Transform> MoveTowardTarget;

    public static Action ResetAction;

    public static Action<bool, Slot.BoardSlotMethod> EnableHoverAction;

    public static Action<int> ChipSelectAction;

    public static Action ResetHoverAction;

    public static Action<Slot.BoardSlotMethod> BoardHoverAction;

    public static Action<Slot> BoardSlotAction;
}
