using UnityEngine;


public class FinishFX : MonoBehaviour
{
    ParticleSystem confetti;

    void Start()
    {
        confetti = GetComponentInChildren<ParticleSystem>();
        if (confetti == null) Debug.LogWarning("FinishFX: no Particle System (Confetti) inside " + name);
    }

    public void Celebrate()
    {
        if (confetti != null) confetti.Play();
    }
}
