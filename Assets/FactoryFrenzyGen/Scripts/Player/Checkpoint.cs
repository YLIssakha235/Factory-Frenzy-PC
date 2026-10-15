using UnityEngine;

// PC-PLA-P7 – when the player walks through, this becomes his new respawn point.
// It is on the "CheckpointTrigger" object of the Checkpoint prefab.
public class Checkpoint : MonoBehaviour
{
    Transform respawnPoint;
    bool activated = false;

    void Start()
    {
        // "RespawnPoint" is next to this object in the Checkpoint prefab
        if (transform.parent != null)
        {
            respawnPoint = transform.parent.Find("RespawnPoint");
        }
        if (respawnPoint == null)
        {
            respawnPoint = transform; // if not found, respawn here
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;                   // only once
        if (!other.CompareTag("Player")) return; // only the player

        PlayerPhysics player = other.GetComponent<PlayerPhysics>();
        if (player == null) return;

        activated = true;
        player.SetCheckpoint(respawnPoint.position);

        // turn the arch green
        CheckpointFX fx = GetComponentInParent<CheckpointFX>();
        if (fx != null)
        {
            fx.Activate();
        }
    }
}
