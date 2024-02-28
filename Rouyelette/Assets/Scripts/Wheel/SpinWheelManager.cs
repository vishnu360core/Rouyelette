using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheelManager : MonoBehaviour
{ 
    [SerializeField] Wheel _spinWheel;
    [SerializeField] Ball ball;


    private void Start()
    {
        Actions.ResetAction += ResetAction;
    }

    void ResetAction()
    {
        ball.ResetAction();
    }

    public void SpinAction()
    {
        ball.EnablePhysics(true);
        _spinWheel.SpinAction();
    }
}
