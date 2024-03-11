using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Range(0, 100f)]
    [SerializeField] float _collisionForce;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();

        // If the other object has a Rigidbody, apply force to it
        if (otherRigidbody != null)
        {
            // Calculate the force direction (away from the collision point)
            Vector3 forceDirection = (collision.contacts[0].point - transform.position).normalized;

            // Apply force to the other Rigidbody
            otherRigidbody.AddForce(forceDirection * _collisionForce, ForceMode.Impulse);
        }
    }

}
