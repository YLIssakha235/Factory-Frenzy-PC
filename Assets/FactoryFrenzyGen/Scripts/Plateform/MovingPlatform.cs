
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Deplacement")]
    public Vector3 endOffset = new Vector3(0, 0, 5);
    public float travelTime = 3f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float elapsedTime;

    private CharacterController player;

    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + endOffset;

        player = FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        if (travelTime <= 0f)
            return;

        Vector3 oldPosition = transform.position;

        elapsedTime += Time.deltaTime;

        // Aller-retour entre 0 et 1
        float t = Mathf.PingPong(
            elapsedTime / travelTime,
            1f
        );

        // Deplacement progressif
        transform.position = Vector3.Lerp(
            startPosition,
            endPosition,
            Mathf.SmoothStep(0f, 1f, t)
        );

        Vector3 movement = transform.position - oldPosition;

        // Transporter le joueur lorsqu'il est dessus
        if (player != null && player.enabled && IsPlayerOnTop())
        {
            player.Move(movement + Vector3.down * 0.05f);
        }
    }

    bool IsPlayerOnTop()
    {
        Vector3 localPosition =
            transform.InverseTransformPoint(player.transform.position);

        bool insideX = Mathf.Abs(localPosition.x) < 1.9f;
        bool insideZ = Mathf.Abs(localPosition.z) < 1.9f;
        bool above = localPosition.y > 0f && localPosition.y < 1.2f;

        return insideX && insideZ && above;
    }
}
