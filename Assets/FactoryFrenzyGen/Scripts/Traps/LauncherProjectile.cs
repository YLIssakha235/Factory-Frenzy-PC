using UnityEngine;
using StarterAssets;


public class LauncherProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 5f;
    public float pushForce = 15f;
    public float upwardForce = 5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = false;   
    }

    void Start()
    {
        Destroy(gameObject, lifetime);

        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shield")
        {
            Debug.Log("Projectile absorbe par le mur !");
            Destroy(gameObject);
            return;
        }

        ThirdPersonController player = other.GetComponentInParent<ThirdPersonController>();

        if (player != null)
        {
            Debug.Log("Projectile : joueur touche !");

            Vector3 force = transform.forward * pushForce;
            force.y = upwardForce;

            player.AddImpulse(force);

            SoundFX.Play("projectile_hit", transform.position);   
            Destroy(gameObject);
        }
    }
}