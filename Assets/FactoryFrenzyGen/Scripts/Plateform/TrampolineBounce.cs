using UnityEngine;
using StarterAssets;

public class TrampolineBounce : MonoBehaviour
{
    public float bounceForce = 12f;

    private TrampolineFX trampolineFX;

    void Start()
    {
        trampolineFX = GetComponentInParent<TrampolineFX>();
    }

    void OnTriggerEnter(Collider other)
    {
        ThirdPersonController player =
            other.GetComponent<ThirdPersonController>();

        if (player == null) return;

        player.AddImpulse(Vector3.up * bounceForce);

        if (trampolineFX != null)
        {
            trampolineFX.Bounce();
        }
    }
}