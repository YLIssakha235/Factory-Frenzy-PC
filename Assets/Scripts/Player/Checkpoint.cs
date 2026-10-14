using UnityEngine;

// PC-PLA-P7 – when the player walks through, this becomes his new respawn point.
// It is on the "CheckpointTrigger" object of the Checkpoint prefab.
public class Checkpoint : MonoBehaviour
{
    Transform respawnPoint;
    bool activated = false;

    void Start()
    {
        // "RespawnPoint" is a child of the Checkpoint prefab
        respawnPoint = transform.parent.Find("RespawnPoint");
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;                   // only once
        if (!other.CompareTag("Player")) return; // only the player

        activated = true;
        other.GetComponent<PlayerPhysics>().SetCheckpoint(respawnPoint.position);

        // turn the arch green
        CheckpointFX fx = GetComponentInParent<CheckpointFX>();
        if (fx != null)
        {
            fx.Activate();
        }
    }
}
