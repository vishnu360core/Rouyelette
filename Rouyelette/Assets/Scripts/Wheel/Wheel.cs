using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    float angle;

    [SerializeField] private bool resetPositionAfterSpin;

    [Header("Settings:")]
    [Range(0f, 100f)]
    [SerializeField] private float arrowBrakeSpeed = 10;
    [Range(0,1000f)]
    [SerializeField] private float _speed = 20;

    [Space]
    [SerializeField] float _curreSpeed;  


     [SerializeField] bool _isStopped = false;
     public bool IsStopped => _isStopped;

     [HideInInspector]
     public bool _enableBrake = false; 

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(Spin());

        StartCoroutine(IdleRotation());
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void ResetAction()
    {
        _isStopped = false;
        _enableBrake= false;

        StopAllCoroutines();
        StartCoroutine(IdleRotation());
    }

    public void  SpinAction()
    {
        StopAllCoroutines();
        StartCoroutine(Spin());
    }
    IEnumerator Spin()
    {
        // Get a random speed from min and ma value
        // Speed = minSpeed;  /*Random.Range(minSpeed, maxSpeed);*/

        _curreSpeed = _speed;

        if (resetPositionAfterSpin)
            angle = 0;

        while (_curreSpeed > 0)
        {
            if(_curreSpeed < 250.0f)
                 Actions.StoppedSpin();

            angle += Time.deltaTime * _curreSpeed;
            angle %= 360;

            // if the speed higher than 0 the arrow decrease speed
            if (_curreSpeed > 0 && _enableBrake)
                _curreSpeed -= Time.deltaTime * arrowBrakeSpeed;

            // Apply the angle to the arrow
            transform.localEulerAngles = -Vector3.up * angle;


            yield return null;
        }

        _isStopped = true;  

        Debug.Log("Stopped spin");
    }

    /// <summary>
    /// Idle Rotation
    /// </summary>
    /// <returns></returns>
    IEnumerator IdleRotation()
    {
        _curreSpeed = 70f;

        angle = 0;

        while(true)
        {
            angle += Time.deltaTime * _curreSpeed;
            angle %= 360;

            transform.localEulerAngles = -Vector3.up * angle;

            yield return null;
        }
    }
}
