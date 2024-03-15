using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObstacleInterface
{
    public void HitAction();
}

[RequireComponent(typeof(BoxCollider))]
public class Obstacle : MonoBehaviour
{
    [Range(0, 100f)]
    [SerializeField] float _collisionForce;

    [Range(0, 100f)]
    [SerializeField] float _jumpforce;

    BoxCollider _collider;

    public ObstacleInterface callback;

    private void OnEnable()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public void EnableColliders(bool enable)
    {
        _collider.enabled = enable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();

        // If the other object has a Rigidbody, apply force to it
        if (otherRigidbody != null)
        {
            Debug.LogWarning("Obstacle Hit !!!!!!!");

            // Calculate the force direction (away from the collision point)
           Vector3 forceDirection = (collision.contacts[0].point - transform.position).normalized;

           //Vector3 forceDirection = (this.transform.position - transform.position).normalized;

            // Apply force to the other Rigidbody
           otherRigidbody.AddForce(-forceDirection * _collisionForce, ForceMode.Impulse);
         // otherRigidbody.AddForce(Vector3.up * _jumpforce, ForceMode.Impulse);

            callback.HitAction();   
        }
    }

}
