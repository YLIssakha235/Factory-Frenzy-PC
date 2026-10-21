using UnityEngine;
using StarterAssets;


public class KillZone : MonoBehaviour
{
    void Start()
    {
        Collider zone = GetComponent<Collider>();
        if (zone == null) Debug.LogWarning("KillZone: add a Box Collider to " + name);
        else zone.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        ThirdPersonController player = other.GetComponentInParent<ThirdPersonController>();
        if (player == null) return;

        //Debug.Log("KillZone: the player fell -> respawn");
        player.Respawn();
    }
}
