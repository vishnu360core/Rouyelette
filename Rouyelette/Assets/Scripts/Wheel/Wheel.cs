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
    [SerializeField] private float minSpeed = 20;
    [Range(0,1000f)]
    [SerializeField] private float maxSpeed = 100;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spin());
    }

    // Update is called once per frame
    void Update()
    {
      
    }


    IEnumerator Spin()
    {
        // Get a random speed from min and ma value
        float Speed = Random.Range(minSpeed, maxSpeed);

        if (resetPositionAfterSpin)
            angle = 0;

        while (Speed > 0)
        {
            angle += Time.deltaTime * Speed;
            angle %= 360;
            // if the speed higher than 0 the arrow decrease speed
            if (Speed > 0)
                Speed -= Time.deltaTime * arrowBrakeSpeed;

            // Apply the angle to the arrow
            transform.localEulerAngles = -Vector3.up * angle;


            yield return null;
        }


        Debug.Log("Stopped spin");
    }
}
