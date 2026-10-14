using UnityEngine;

// PC-PLA-P7 – the flag waves, and Activate() turns the arch green.
// Checkpoint.cs calls Activate() when the player passes.
public class CheckpointFX : MonoBehaviour
{
    Transform flag;
    bool isOn = false;

    void Start()
    {
        flag = transform.Find("Arch/Flag");
    }

    public void Activate()
    {
        isOn = true;

        // every part of the arch becomes green and glows
        foreach (Renderer part in transform.Find("Arch").GetComponentsInChildren<Renderer>())
        {
            part.material.color = Color.green;
            part.material.EnableKeyword("_EMISSION");
            part.material.SetColor("_EmissionColor", Color.green * 0.5f);
        }
    }

    void Update()
    {
        // the flag waves (faster when activated)
        float speed = isOn ? 8 : 2.5f;
        flag.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * speed) * 12, 0);
    }
}
