using UnityEngine;

// PC-PLA-P4 – big invisible trigger under the level.
// When the player falls into it, he goes back to the last checkpoint.
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerPhysics>().Respawn();
        }
    }
}
