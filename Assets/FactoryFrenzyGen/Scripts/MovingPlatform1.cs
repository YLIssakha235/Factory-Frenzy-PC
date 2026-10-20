using UnityEngine;

// PC-PLA-P5 – plateforme mouvante : va et vient entre deux points et transporte le joueur.
// A mettre sur la racine du prefab "Moving". PAS de Rigidbody sur la plateforme.
//
// Principe (tout dans le même Update, donc pas de tremblement) :
//   1. on déplace la plateforme
//   2. si le joueur est debout dessus, on le déplace exactement de la même quantité
public class MovingPlatform1 : MonoBehaviour
{
    public Vector3 endOffset = new Vector3(5, 0, 0);  // arrivée = départ + endOffset (utilisé sans fichier JSON)
    public float travelTime = 3f;                     // secondes pour aller du départ à l'arrivée

    Vector3 startPosition;
    Vector3 endPosition;
    float timer;
    CharacterController player;

    // Appelé par le LevelLoader (niveau JSON fait en VR), juste après la création, donc AVANT Start
    public void SetPath(LevelElement element)
    {
        endOffset = element.endPosition - element.position;
        if (element.travelTime > 0f) travelTime = element.travelTime;
    }

    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + endOffset;

        // le joueur = l'objet qui a le CharacterController
        player = FindObjectOfType<CharacterController>();

        // un Rigidbody ferait transporter le joueur une 2e fois par le ThirdPersonController
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            Debug.LogWarning("MovingPlatform: Rigidbody retiré de " + name + " (pas besoin, ce script transporte le joueur)");
            Destroy(body);
        }
    }

    void Update()
    {
        if (travelTime <= 0f) return;

        // 1. déplacer la plateforme : t va 0 → 1 → 0 → 1 ... et ralentit aux deux bouts
        Vector3 oldPosition = transform.position;
        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / travelTime, 1f);
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0f, 1f, t));

        // 2. transporter le joueur s'il est debout dessus
        Vector3 movement = transform.position - oldPosition;
        if (player != null && player.enabled && IsPlayerOnTop())
        {
            // + un petit peu vers le bas : le joueur touche la plateforme, donc isGrounded reste vrai
            // (sinon l'animation croit qu'il est en l'air et agite les bras à chaque image)
            player.Move(movement + Vector3.down * 0.05f);
        }
    }

    // Le joueur est-il debout sur la plateforme ? (dessus de 3.5 x 3.5 m, hauteur du dessus 0.25 m)
    bool IsPlayerOnTop()
    {
        // position du joueur vue depuis la plateforme (marche même si elle est tournée ou agrandie)
        Vector3 local = transform.InverseTransformPoint(player.transform.position);
        bool insideX = Mathf.Abs(local.x) < 1.9f;
        bool insideZ = Mathf.Abs(local.z) < 1.9f;
        bool justAbove = local.y > 0f && local.y < 1.2f;
        return insideX && insideZ && justAbove;
    }
}