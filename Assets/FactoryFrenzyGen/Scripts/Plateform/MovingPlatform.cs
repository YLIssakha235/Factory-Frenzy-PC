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
    private float lastPlayerY;                     

    
    public void SetPath(LevelElement element)
    {
        endOffset = element.endPosition - element.position;
        if (element.travelTime > 0f) travelTime = element.travelTime;
    }

    void Start()
    {
  
        startPosition = transform.position;
        endPosition = startPosition + endOffset;

       
        player = FindObjectOfType<CharacterController>();
        if (player != null) lastPlayerY = player.transform.position.y;

        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            Debug.LogWarning("MovingPlatform: Rigidbody retiré de " + name + " (pas besoin, ce script transporte le joueur)");
            Destroy(body);
        }
    }

    void Update()
    {
        if (travelTime <= 0f)
            return;

        Vector3 oldPosition = transform.position;

        elapsedTime += Time.deltaTime;
        float t = Mathf.PingPong(elapsedTime / travelTime, 1f);

        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0f, 1f, t));

        Vector3 movement = transform.position - oldPosition;

        if (player != null && player.enabled)
        {
            bool goingUp = player.transform.position.y > lastPlayerY + 0.01f;

            
            if (!goingUp && IsPlayerOnTop())
            {
                player.Move(movement + Vector3.down * 0.05f);
            }

            lastPlayerY = player.transform.position.y;
        }
    }

    bool IsPlayerOnTop()
    {
        Vector3 localPosition = transform.InverseTransformPoint(player.transform.position);

        bool insideX = Mathf.Abs(localPosition.x) < 1.9f;
        bool insideZ = Mathf.Abs(localPosition.z) < 1.9f;
        bool standing = localPosition.y > 0f && localPosition.y < 0.4f;   // les pieds touchent le dessus

        return insideX && insideZ && standing;
    }
}