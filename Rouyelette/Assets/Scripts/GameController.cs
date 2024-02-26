using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Actions.ballHit += BallGroundAction;
        Actions.BoardSelectAction += BoardSlotAction;
    }

    void BallGroundAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.ballHit);
    }


    void BoardSlotAction(Slot.BoardSlotType boardSlotType,Slot.BoardSlotMethod slotMethod,Slot.ColorType colorType)
    {
       switch(boardSlotType)
        {
            case Slot.BoardSlotType.integer:
                break;

            case Slot.BoardSlotType.text:

                Actions.EnableHoverAction(true, slotMethod);
                break;
        }
    }

   
}
