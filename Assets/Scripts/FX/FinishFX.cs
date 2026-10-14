using UnityEngine;

// PC-PLA-P3 – confetti when the player finishes.
// Call Celebrate() when the player crosses the finish line.
// It plays a Particle System named "Confetti" if the Finish has one.
public class FinishFX : MonoBehaviour
{
    ParticleSystem confetti;

    void Start()
    {
        Transform child = transform.Find("Confetti");
        if (child != null)
        {
            confetti = child.GetComponent<ParticleSystem>();
        }
    }

    public void Celebrate()
    {
        if (confetti != null)
        {
            confetti.Play();
        }
    }
}
