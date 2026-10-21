using UnityEngine;

// PC-PLA-P5 – plateforme mouvante : va et vient entre deux points et transporte le joueur.
// A mettre sur la racine du prefab "Moving". PAS de Rigidbody sur la plateforme.
//
// Principe (tout dans le même Update, donc pas de tremblement) :
//   1. on déplace la plateforme
//   2. si le joueur est DEBOUT dessus (pas en train de sauter), on le déplace de la même quantité
public class MovingPlatform : MonoBehaviour
{
    [Header("Deplacement")]
    public Vector3 endOffset = new Vector3(0, 0, 5);  // arrivée = départ + endOffset (utilisé sans fichier JSON)
    public float travelTime = 3f;                     // secondes pour aller du départ à l'arrivée

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float elapsedTime;

    private CharacterController player;
    private float lastPlayerY;                        // hauteur du joueur à l'image précédente (pour savoir s'il saute)

    // Appelé par le LevelLoader (niveaux JSON / VR), juste après la création, donc AVANT Start
    public void SetPath(LevelElement element)
    {
        endOffset = element.endPosition - element.position;
        if (element.travelTime > 0f) travelTime = element.travelTime;
    }

    void Start()
    {
        // Enregistrer les positions
        startPosition = transform.position;
        endPosition = startPosition + endOffset;

        // Recuperer le joueur
        player = FindObjectOfType<CharacterController>();
        if (player != null) lastPlayerY = player.transform.position.y;

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
        if (travelTime <= 0f)
            return;

        // Ancienne position de la plateforme
        Vector3 oldPosition = transform.position;

        // Aller-retour : t va 0 → 1 → 0 → 1 ...
        elapsedTime += Time.deltaTime;
        float t = Mathf.PingPong(elapsedTime / travelTime, 1f);

        // Deplacement progressif (ralentit aux deux bouts)
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0f, 1f, t));

        // Calculer le deplacement effectue
        Vector3 movement = transform.position - oldPosition;

        if (player != null && player.enabled)
        {
            // le joueur monte = il saute : on ne le touche pas (sinon le saut devient faible)
            bool goingUp = player.transform.position.y > lastPlayerY + 0.01f;

            // Transporter le joueur seulement s'il est DEBOUT dessus
            // (+ un petit peu vers le bas pour qu'il reste "au sol" pour l'animation)
            if (!goingUp && IsPlayerOnTop())
            {
                player.Move(movement + Vector3.down * 0.05f);
            }

            lastPlayerY = player.transform.position.y;
        }
    }

    // Le joueur est-il debout sur la plateforme ? (dessus de 3.5 x 3.5 m, hauteur du dessus 0.25 m)
    bool IsPlayerOnTop()
    {
        // position du joueur vue depuis la plateforme (marche même si elle est tournée)
        Vector3 localPosition = transform.InverseTransformPoint(player.transform.position);

        bool insideX = Mathf.Abs(localPosition.x) < 1.9f;
        bool insideZ = Mathf.Abs(localPosition.z) < 1.9f;
        bool standing = localPosition.y > 0f && localPosition.y < 0.4f;   // les pieds touchent le dessus

        return insideX && insideZ && standing;
    }
}