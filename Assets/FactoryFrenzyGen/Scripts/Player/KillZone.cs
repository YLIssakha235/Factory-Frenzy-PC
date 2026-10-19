using UnityEngine;
using StarterAssets;

public class KillZone : MonoBehaviour
{
    void Start()
    {
        // Vérifier que le collider est un trigger
        Collider zone = GetComponent<Collider>();

        if (zone == null)
            Debug.LogWarning("KillZone: add a Box Collider to " + name);
        else
            zone.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Récupérer le contrôleur du joueur
        ThirdPersonController player =
            other.GetComponentInParent<ThirdPersonController>();

        if (player == null) return;

        Debug.Log("KillZone: player fell -> respawn");

        // Faire réapparaître le joueur
        player.Respawn();
    }
}