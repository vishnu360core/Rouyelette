using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent (typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    Rigidbody rb;

    bool InAir = true;

    /// <summary>
    /// Actions inmplemented on enable
    /// </summary>
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        Actions.StoppedSpin += StopSpinAction;
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
            return;

        if (rb.angularVelocity.magnitude > 5.0f )
            Actions.ballHit();
    }

    private void OnCollisionEnter(Collision collision)
    {
        InAir = false;

       // Actions.ballHit();
    }
}
