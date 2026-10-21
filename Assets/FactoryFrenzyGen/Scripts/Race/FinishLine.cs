using UnityEngine;


public class FinishLine : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        RaceManager race = FindObjectOfType<RaceManager>();
        if (race != null)
        {
            race.PlayerFinished();
        }
    }
}
