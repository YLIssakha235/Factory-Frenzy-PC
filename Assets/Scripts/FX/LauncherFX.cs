using UnityEngine;

// PC-TRA-T2 – the launcher's eye shows its state, and the barrel recoils when it shoots.
// The launcher state machine calls ShowSearch(), ShowOrient(), ShowFire() and Recoil().
public class LauncherFX : MonoBehaviour
{
    Renderer eye;
    Transform barrel;
    Vector3 barrelStart;
    float timer = 1; // 1 = not recoiling

    void Start()
    {
        eye = transform.Find("Head/Eye").GetComponent<Renderer>();
        barrel = transform.Find("Head/Barrel");
        barrelStart = barrel.localPosition;
        ShowSearch();
    }

    public void ShowSearch() { SetEyeColor(Color.yellow); }
    public void ShowOrient() { SetEyeColor(new Color(1f, 0.5f, 0f)); } // orange
    public void ShowFire()   { SetEyeColor(Color.red); }

    void SetEyeColor(Color color)
    {
        eye.material.color = color;
        eye.material.SetColor("_EmissionColor", color);
    }

    public void Recoil()
    {
        timer = 0;
    }

    void Update()
    {
        if (timer < 0.2f)
        {
            timer += Time.deltaTime;
            // barrel goes back, then returns in 0.2 seconds
            float back = 1 - timer / 0.2f;
            barrel.localPosition = barrelStart - new Vector3(0, 0, 0.3f * back);
        }
    }
}
