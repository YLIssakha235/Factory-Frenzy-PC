using UnityEngine;

// PC-PLA-P2 – the 3 lights on the start gate.
// The countdown calls SetCountdown(3), (2), (1), then (0) for GO.
public class StartLightsFX : MonoBehaviour
{
    Renderer[] lights = new Renderer[3];

    void Start()
    {
        lights[0] = transform.Find("Gate/Light1").GetComponent<Renderer>();
        lights[1] = transform.Find("Gate/Light2").GetComponent<Renderer>();
        lights[2] = transform.Find("Gate/Light3").GetComponent<Renderer>();
    }

    // 3 → 1 red light, 2 → 2 red lights, 1 → 3 red lights, 0 → all green
    public void SetCountdown(int number)
    {
        for (int i = 0; i < 3; i++)
        {
            Color color = Color.gray;
            if (number == 0) color = Color.green;
            else if (i < 4 - number) color = Color.red;

            lights[i].material.color = color;
            lights[i].material.EnableKeyword("_EMISSION"); // make it glow
            lights[i].material.SetColor("_EmissionColor", color == Color.gray ? Color.black : color);
        }
    }
}
