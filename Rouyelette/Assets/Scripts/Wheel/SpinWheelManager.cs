using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheelManager : MonoBehaviour,BallInterface,ObstacleInterface
{ 
    [SerializeField] Wheel _spinWheel;
    [SerializeField] Ball ball;

    [SerializeField] GameObject _colliders;

    [Space]
    [SerializeField] List<Obstacle> obstacles = new List<Obstacle>();


    Transform ballTarget;

    private void Start()
    {
        ball.callback = this;

        for (int i = 0; i < obstacles.Count; i++)
         obstacles[i].callback = this; 

            Actions.ResetAction += ResetAction;

        Actions.MoveTowardTarget += MoveTowardsTargetAction;

        Actions.ReachedDestination += ReachedTargetDestination;

        Actions.SetBallTarget += SetPinBallTarget;

        Actions.EnableObstacles += EnableObstacles;
    }

    public void HitAction() 
    {
        EnableObstacles(false);    
    }

    private void SetPinBallTarget(Transform transform)
    {
        ballTarget = transform;
        ball.SetTarget(transform);  
    }

    public void CompletedMovement()
    {
        StartCoroutine(EndAction());
    }

    void EnableObstacles(bool enable)
    {
        for(int i = 0; i < obstacles.Count; i++)
            obstacles[i].EnableColliders(enable);
    }

    private void ReachedTargetDestination()
    {
        ball.ReachedDestination();

        AudioManager.Instance.Audio.Stop(); 

       // wheelwithObstacles.enabled = true;
       // wheelwithoutObstacles.enabled = false;

        //_colliders.SetActive(true);
    }

    IEnumerator EndAction()
    {
        yield return new WaitUntil(() => _spinWheel.IsStopped);

        //ball.ResetAction();
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

        ballTarget = null;

        EnableObstacles(false);
    }

    public void SpinAction()
    {
        AudioManager.Instance.PlayClip(AudioManager.Clip.wheel);

        ball.EnableGravityAction();

        StartCoroutine(ball.RotateObject());

        _spinWheel.SpinAction();
    }
}
