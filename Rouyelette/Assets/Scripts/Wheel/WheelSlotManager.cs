using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSlotManager : MonoBehaviour
{
   [SerializeField] List<Slot> slots = new List<Slot>();

    public Slot GetWheelSlot(int Number)
    {
        foreach(Slot slot in slots)
        {
            if(slot.SlotNumber == Number) 
            { 
                 return slot;   
            }
        }
       
        return null;
    }
}
