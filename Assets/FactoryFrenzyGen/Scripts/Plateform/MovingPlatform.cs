using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    // Déplacement par rapport à la position de départ
    public Vector3 endOffset = new Vector3(0, 0, 5);

    // Temps pour aller du départ à l'arrivée
    public float travelTime = 3f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Rigidbody rb;
    private float elapsedTime;

    void Start()
    {
        // Récupérer le Rigidbody de la plateforme
        rb = GetComponent<Rigidbody>();

        // Enregistrer les positions de départ et d'arrivée
        startPosition = rb.position;
        endPosition = startPosition + endOffset;
    }

    void FixedUpdate()
    {
        if (travelTime <= 0f) return;

        // Calculer le temps écoulé
        elapsedTime += Time.fixedDeltaTime;

        // Aller-retour entre 0 et 1
        float t = Mathf.PingPong(elapsedTime / travelTime, 1f);

        // Calculer la nouvelle position
        Vector3 newPosition = Vector3.Lerp(
            startPosition,
            endPosition,
            t
        );

        // Déplacer le Rigidbody cinématique
        rb.MovePosition(newPosition);
    }
}