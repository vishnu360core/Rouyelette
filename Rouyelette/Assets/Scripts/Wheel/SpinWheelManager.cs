using System;
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

        Actions.MoveTowardTarget += MoveTowardsTargetAction;

        Actions.ReachedDestination += ReachedTargetDestination;
    }

    private void ReachedTargetDestination()
    {
        ball.ReachedDestination();
    }

    /// <summary>
    /// Action impleemnted on moving towards the target
    /// </summary>
    /// <param name="transform"></param>
    private void MoveTowardsTargetAction(Transform transform)
    {
        Debug.Log("Moving !!!!!!!!!!!!!!");

        ball.EnablePhysics(false);

        ball.MoveTowards(transform);
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
