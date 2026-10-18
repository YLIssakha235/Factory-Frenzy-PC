using UnityEngine;
using StarterAssets;

public class Bumper : MonoBehaviour
{
   
    public float pushForce = 12f;

    public float upwardForce = 3f;

    private BumperFX bumperFX;

    void Start()
    {
        bumperFX = GetComponentInParent<BumperFX>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController player =
            other.GetComponentInParent<ThirdPersonController>();

        if (player == null) return;

        // Direction opposée au bumper
        Vector3 direction = player.transform.position -
                            transform.position;

        direction.y = 0f;
        direction = direction.normalized;

        Vector3 impulse = direction * pushForce +
                          Vector3.up * upwardForce;

       
        player.AddImpulse(impulse);

        if (bumperFX != null)
        {
            bumperFX.Pulse();
        }
    }
}