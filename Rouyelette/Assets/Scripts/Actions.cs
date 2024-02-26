using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Actions 
{
    public static Action ballHit;

    public static Action StoppedSpin;

    public static Action<bool, Slot.BoardSlotMethod> EnableHoverAction;

    public static Action ResetHoverAction;

    public static Action<Slot.BoardSlotType,Slot.BoardSlotMethod,Slot.ColorType> BoardSelectAction;
}
