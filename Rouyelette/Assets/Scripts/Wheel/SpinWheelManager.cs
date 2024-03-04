using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheelManager : MonoBehaviour,BallInterface
{ 
    [SerializeField] Wheel _spinWheel;
    [SerializeField] Ball ball;


    private void Start()
    {
        ball.callback = this;


        Actions.ResetAction += ResetAction;

        Actions.MoveTowardTarget += MoveTowardsTargetAction;

        Actions.ReachedDestination += ReachedTargetDestination;
    }


    public void CompletedMovement()
    {
        StartCoroutine(EndAction());
    }

    private void ReachedTargetDestination()
    {
        ball.ReachedDestination();

    }

    IEnumerator EndAction()
    {
        yield return new WaitUntil(() => _spinWheel.IsStopped);

       Actions.EndedSpinAction();
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
        _spinWheel.ResetAction();   
    }

    public void SpinAction()
    {
        ball.EnablePhysics(true);
        _spinWheel.SpinAction();
    }
}
