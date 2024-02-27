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
        Actions.BoardHoverAction += HoverBoardSlotAction;
        Actions.ChipSelectAction += ChipSelectAction;
    }

    void BallGroundAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.ballHit);
    }


    void HoverBoardSlotAction(Slot.BoardSlotMethod slotMethod = Slot.BoardSlotMethod.NULL)
    {
        Actions.EnableHoverAction(true, slotMethod);
    }


    void ChipSelectAction(Chip chip)
    {

    }
}
