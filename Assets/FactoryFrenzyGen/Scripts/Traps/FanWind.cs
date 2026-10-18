using UnityEngine;
using StarterAssets;

public class FanWind : MonoBehaviour
{
    [SerializeField] private float windStrength = 5f;

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController player =
            other.GetComponentInParent<ThirdPersonController>();

        if (player == null) return;

        player.SetWindVelocity(transform.forward * windStrength);
    }

    private void OnTriggerExit(Collider other)
    {
        ThirdPersonController player =
            other.GetComponentInParent<ThirdPersonController>();

        if (player == null) return;

        player.SetWindVelocity(Vector3.zero);
    }
}