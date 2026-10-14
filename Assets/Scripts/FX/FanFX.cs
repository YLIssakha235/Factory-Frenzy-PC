using UnityEngine;

// PC-TRA-T3 – the fan blades turn all the time.
public class FanFX : MonoBehaviour
{
    public float speed = 1500; // degrees per second
    Transform blades;

    void Start()
    {
        blades = transform.Find("Hub/Blades");
    }

    void Update()
    {
        blades.Rotate(0, 0, speed * Time.deltaTime);
    }
}
