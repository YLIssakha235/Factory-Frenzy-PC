using UnityEngine;
using StarterAssets;


public class Checkpoint : MonoBehaviour
{
    Transform respawnPoint;
    bool activated = false;

    void Start()
    {
        Collider zone = GetComponent<Collider>();
        if (zone == null) Debug.LogWarning("Checkpoint: add a Box Collider (Is Trigger) to " + name);
        else zone.isTrigger = true;

        Transform checkpointRoot = transform;
        if (transform.parent != null) checkpointRoot = transform.parent;
        respawnPoint = checkpointRoot.Find("RespawnPoint");
        if (respawnPoint == null) respawnPoint = transform;

        //Debug.Log("Checkpoint: ready on " + name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        ThirdPersonController player = other.GetComponentInParent<ThirdPersonController>();
        if (player == null) return;

        activated = true;
        player.SetRespawnPosition(respawnPoint.position + Vector3.up * 0.5f);   
        //Debug.Log("Checkpoint: activated, new respawn point = " + respawnPoint.position);

        SoundFX.Play("checkpoint", transform.position);                         

        CheckpointFX fx = GetComponentInParent<CheckpointFX>();
        if (fx != null) fx.Activate();                                          
    }
}
