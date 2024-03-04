using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent (typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    Rigidbody rb;

   [SerializeField] bool InAir = false;

   [SerializeField] Transform _resetTransform;
   [SerializeField] Transform _target;

    public Transform Target => _resetTransform;


    SphereCollider sphereCollider;

    /// <summary>
    /// Actions inmplemented on enable
    /// </summary>
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        sphereCollider = rb.GetComponent<SphereCollider>();

       // Actions.StoppedSpin += StopSpinAction;
    }


    public void ResetAction()
    {
        rb.isKinematic = true;

       // this.transform.localPosition =  _resetTransform.localPosition;
       // this.transform.localRotation = _resetTransform.localRotation;
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
            this.transform.DOMove(_target.transform.position, 0.4f);

            //this.transform.position = _target.transform.position;
        }

        //if (rb.angularVelocity.magnitude > 5.0f )
        //    Actions.ballHit();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //InAir = false;

       // Actions.ballHit();
    } 

    public void ReachedDestination()
    {
        transform.parent = _target;

        InAir = true;


        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }

    public void  MoveTowards(Transform t)
    {
       _target = t;

        rb.DOMove(t.position, 0.4f);
    }

}
