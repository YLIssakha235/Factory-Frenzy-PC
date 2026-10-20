
using UnityEngine;

public class LauncherController : MonoBehaviour
{
    [Header("References")]
    public Transform head;
    public Transform player;
    public Transform muzzle;
    public GameObject projectilePrefab;

    [Header("Detection et rotation")]
    public float detectionRadius = 30f;
    public float maxAimAngle = 180f;
    public float aimSpeed = 90f;

    [Header("Recherche")]
    public float searchAngle = 45f;
    public float searchSpeed = 30f;

    [Header("Tir")]
    public float fireInterval = 2f;

    private Quaternion initialRotation;
    private float fireTimer;
    private RaceManager raceManager;

    void Start()
    {
        if (head != null)
            initialRotation = head.localRotation;

        raceManager = FindObjectOfType<RaceManager>();
    }

    void Update()
    {
        // Le canon attend le GO! du RaceManager
        if (raceManager != null && !raceManager.IsRaceRunning)
            return;

        if (head == null || player == null || muzzle == null)
            return;

        Vector3 direction = player.position - head.position;
        direction.y = 0f;

        // Joueur hors de portee : recherche
        if (direction.magnitude > detectionRadius)
        {
            float angle = Mathf.PingPong(
                Time.time * searchSpeed,
                searchAngle * 2f
            ) - searchAngle;

            head.localRotation = Quaternion.RotateTowards(
                head.localRotation,
                initialRotation * Quaternion.Euler(0f, angle, 0f),
                aimSpeed * Time.deltaTime
            );

            fireTimer = 0f;
            return;
        }

        // Direction actuelle du canon
        Vector3 cannonForward = muzzle.forward;
        cannonForward.y = 0f;

        if (direction.sqrMagnitude < 0.001f ||
            cannonForward.sqrMagnitude < 0.001f)
            return;

        // Calcul de l'angle vers le joueur
        float angleToPlayer = Vector3.SignedAngle(
            cannonForward,
            direction,
            Vector3.up
        );

        // Rotation progressive
        float rotationStep = Mathf.Clamp(
            angleToPlayer,
            -aimSpeed * Time.deltaTime,
            aimSpeed * Time.deltaTime
        );

        head.Rotate(Vector3.up, rotationStep, Space.World);

        // Tir automatique
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        if (muzzle == null || projectilePrefab == null)
            return;

        Vector3 shootDirection =
            (player.position + Vector3.up - muzzle.position).normalized;

        Instantiate(
            projectilePrefab,
            muzzle.position,
            Quaternion.LookRotation(shootDirection)
        );

        Debug.Log("Launcher : projectile tire !");
    }
}
