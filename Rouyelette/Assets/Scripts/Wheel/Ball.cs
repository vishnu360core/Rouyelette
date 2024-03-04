using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;


public interface BallInterface
{
    public void CompletedMovement();
}


[RequireComponent(typeof(SphereCollider))]
[RequireComponent (typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    Rigidbody rb;

   [SerializeField] bool InAir = false;

   [SerializeField] Transform _resetTransform;
   [SerializeField] Transform _target;

    public Transform Target => _target;


    public BallInterface callback;

    /// <summary>
    /// Actions inmplemented on enable
    /// </summary>
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();


       // Actions.StoppedSpin += StopSpinAction;
    }


    public void ResetAction()
    {
        InAir = false;

        rb.isKinematic = true;

        this.transform.position = _resetTransform.position;
        //this.transform.rotation = _resetTransform.rotation;

        rb.constraints = RigidbodyConstraints.None;

    }


    public void EnablePhysics(bool enable)
    {
        rb.isKinematic = !enable;

        rb.constraints = !enable ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
    }

    

    /// <summary>
    /// Actions implemented on disable
    /// </summary>
    private void OnDisable()
    {
        Actions.StoppedSpin -= StopSpinAction;
    }

    void StopSpinAction()
    {
        InAir = true;
    }


    private void FixedUpdate()
    {
        if (InAir)
        {
            this.transform.DOMove(_target.transform.position, 0.8f).OnComplete(() => CompletedMovementAction());

            //this.transform.position = _target.transform.position;
        }

        //if (rb.angularVelocity.magnitude > 5.0f )
        //    Actions.ballHit();
    }

    void CompletedMovementAction()
    {
       // InAir = false;

        callback.CompletedMovement();
    }


    private void OnCollisionEnter(Collision collision)
    {
        //InAir = false;

       // Actions.ballHit();
    } 

    public void ReachedDestination()
    {
       // transform.parent = _target;

        InAir = true;

        //transform.rotation = Quaternion.identity;
        //transform.position = Vector3.zero;
    }

    public void  MoveTowards(Transform t)
    {
       _target = t;

        this.transform.DOMove(t.position, 2.5f);
    }

}
