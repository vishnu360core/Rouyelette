using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;


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

   [Header("Transforms:")]
   [SerializeField] Transform _resetTransform;
   [SerializeField] Transform _target;
   [SerializeField] Transform _parent;


    [Header("Physics Settings:")]
    [Range(0, 500)]
    [SerializeField] float _parentRotateSpeed;
    [Range(0, 300f)]
    [SerializeField] float _brakeForce;

    [SerializeField] bool IsBrake = true;
    
    [SerializeField] bool IsGrounded = true;

    public Transform Target => _target;

    public BallInterface callback;

    /// <summary>
    /// Actions inmplemented on enable
    /// </summary>
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(Vector3.forward * 2, ForceMode.Impulse);

        // Actions.StoppedSpin += StopSpinAction;
    }

    public IEnumerator RotateObject()
    {
        while (!IsBrake)
        {
            // Calculate the rotation angle based on time and speed
            float rotationAngle = _parentRotateSpeed * Time.deltaTime;

            // Rotate the object around its own axis
            _parent.transform.Rotate(Vector3.forward, rotationAngle);

            // Wait for the next frame
            yield return null;
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }


    public void ResetAction()
    {
        InAir = false;
        IsBrake = false;
        EnablePhysics(false);

        EnableGravity(false);
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }


    public void EnablePhysics(bool enable)
    {
        rb.isKinematic = !enable;

        IsGrounded = !enable;

        rb.constraints = !enable ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
    }

    public IEnumerator  EnableGravityAction()
    {
        yield return new WaitForSeconds(4.0f);

        EnablePhysics(true);

        rb.constraints = RigidbodyConstraints.None;
        EnableGravity(true);

        IsBrake = true;

        StopCoroutine(RotateObject());
        StopAllCoroutines();

        Actions.EnableObstacles(true);

        // rb.AddForce(Vector3.right * 1, ForceMode.Impulse);
        //rb.AddForce(Vector3.up * 1.8f, ForceMode.Impulse);
        //rb.AddForce(Vector3.forward * 1f, ForceMode.Impulse);

        /// rb.AddForce(brakingForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Actions implemented on disable
    /// </summary>
    private void OnDisable()
    {
       // Actions.StoppedSpin -= StopSpinAction;
    }

    void StopSpinAction()
    {
       //InAir = true;
    }

    /// <summary>
    /// Enable the gravity
    /// </summary>
    /// <param name="enable"></param>
    public void EnableGravity(bool enable) 
    {
        rb.useGravity = enable; 
    }


    private void FixedUpdate()
    {
        if (InAir)
        {
           this.transform.DOMove(_target.transform.position, 0.05f).OnComplete(() => CompletedMovementAction());

            //this.transform.position = _target.transform.position;
        }


        if (IsGrounded)
        {
            this.gameObject.transform.localPosition = _resetTransform.localPosition;
        }

        //if(IsBrake)
        //{
        //    Vector3 brakingForce = -rb.velocity.normalized * 100;
        //    rb.AddForce(brakingForce, ForceMode.Impulse);
        //}


        //if(rb.constraints == RigidbodyConstraints.FreezePositionY)
        //if (rb.velocity.magnitude < 5.0f)
        //    rb.constraints = RigidbodyConstraints.None;
    }

    void CompletedMovementAction()
    {
        callback.CompletedMovement();
    }


    private void OnCollisionEnter(Collision collision)
    {
        //InAir = false;

        // Actions.ballHit();


       //rb.constraints = RigidbodyConstraints.FreezePositionY;
    } 

    public void ReachedDestination()
    {
       // transform.parent = _target;

         InAir = true;

       // DOTween.KillAll();

        //transform.rotation = Quaternion.identity;
        //transform.position = Vector3.zero;
    }

    public void  MoveTowards(Transform t)
    {
        //_target = t;
        //InAir = true;

       // IsBrake = false;

        rb.constraints = RigidbodyConstraints.None;

        if (_target != t)
            this.transform.position = Vector3.MoveTowards(this.transform.position, t.position, 0.001f);
        else
            this.transform.DOMove(_target.position, 1.0f).SetEase(Ease.Linear);



        //rb.MovePosition(t.position);

        //this.transform.DOMove(_target.position, 1.0f).SetEase(Ease.Linear);
    }

}
