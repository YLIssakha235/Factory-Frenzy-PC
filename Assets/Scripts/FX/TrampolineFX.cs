using UnityEngine;

// PC-PLA-P6 – the pink pad squashes when the player bounces.
// The trampoline script calls Bounce().
public class TrampolineFX : MonoBehaviour
{
    Transform pad;
    Vector3 normalSize;
    float timer = 1; // 1 = not animating

    void Start()
    {
        pad = transform.Find("Pad");
        normalSize = pad.localScale;
    }

    public void Bounce()
    {
        timer = 0;
    }

    void Update()
    {
        if (timer < 0.3f)
        {
            timer += Time.deltaTime;
            // flat at the start, back to normal after 0.3 seconds
            float flat = 1 - timer / 0.3f;
            pad.localScale = new Vector3(normalSize.x, normalSize.y * (1 - 0.6f * flat), normalSize.z);
        }
    }
}
