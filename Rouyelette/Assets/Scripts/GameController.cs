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
    }

    void BallGroundAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.ballHit);
    }


   
}
