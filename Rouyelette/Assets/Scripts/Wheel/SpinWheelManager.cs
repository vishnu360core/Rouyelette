using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheelManager : MonoBehaviour,BallInterface
{ 
    [SerializeField] Wheel _spinWheel;
    [SerializeField] Ball ball;

    [SerializeField] GameObject _colliders;


    Transform ballTarget;

    private void Start()
    {
        ball.callback = this;


        Actions.ResetAction += ResetAction;

        Actions.MoveTowardTarget += MoveTowardsTargetAction;

        Actions.ReachedDestination += ReachedTargetDestination;

        Actions.SetBallTarget += SetPinBallTarget;
    }

    private void SetPinBallTarget(Transform transform)
    {
        ballTarget = transform;
        //ball.SetTarget(transform);  
    }

    public void CompletedMovement()
    {
        StartCoroutine(EndAction());
    }

    private void ReachedTargetDestination()
    {
        ball.ReachedDestination();

       // wheelwithObstacles.enabled = true;
       // wheelwithoutObstacles.enabled = false;

        //_colliders.SetActive(true);
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
    private void MoveTowardsTargetAction(Slot slot)
    {
        Debug.Log("Moving !!!!!!!!!!!!!!" + slot.SlotNumber);

       // ball.EnablePhysics(false);

        ball.MoveTowards(slot.transform);
    }

    void ResetAction()
    {
        ball.ResetAction();
        _spinWheel.ResetAction();

       // _colliders.SetActive(false);

        ballTarget = null;
    }

    public void SpinAction()
    {
        ball.EnablePhysics(true);
        _spinWheel.SpinAction();
    }
}
