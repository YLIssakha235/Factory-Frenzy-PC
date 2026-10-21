using UnityEngine;
using StarterAssets; 

public class LauncherController : MonoBehaviour
{
    [Header("References")]
    public Transform head;
    public Transform player;
    public Transform muzzle;
    public GameObject projectilePrefab;
    public string playerObjectName = "PlayerArmature"; 

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
    private bool playerInRange = false; 

    void Start()
    {
        
        player = null;
        ThirdPersonController controller = FindObjectOfType<ThirdPersonController>();
        if (controller != null)
        {
            player = controller.transform;
        }
        else
        {
     
            GameObject target = GameObject.Find(playerObjectName);
            if (target != null) player = target.transform;
        }
        //Debug.Log("Launcher : cible = " + (player != null ? player.name : "aucune"));
        if (head == null) head = transform.Find("Head");
        if (muzzle == null && head != null) muzzle = head.Find("Muzzle");

        if (player == null) //Debug.LogWarning("Launcher : joueur introuvable");
        if (head == null || muzzle == null) //Debug.LogWarning("Launcher : Head ou Muzzle introuvable dans " + name);

        if (head != null)
            initialRotation = head.localRotation;

        raceManager = FindObjectOfType<RaceManager>();
    }

    void Update()
    {
        if (raceManager != null && !raceManager.IsRaceRunning)
            return;

        if (head == null || player == null || muzzle == null)
            return;

        Vector3 direction = player.position - head.position;
        direction.y = 0f;

        if (direction.magnitude > detectionRadius)
        {
            playerInRange = false;

            float angle = Mathf.PingPong(Time.time * searchSpeed, searchAngle * 2f) - searchAngle;

            head.localRotation = Quaternion.RotateTowards(
                head.localRotation,
                initialRotation * Quaternion.Euler(0f, angle, 0f),
                aimSpeed * Time.deltaTime
            );

            fireTimer = 0f;
            return;
        }

        if (!playerInRange)
        {
            playerInRange = true;
            SoundFX.Play("launcher_alert", transform.position);
        }

        Vector3 cannonForward = muzzle.forward;
        cannonForward.y = 0f;

        if (direction.sqrMagnitude < 0.001f || cannonForward.sqrMagnitude < 0.001f)
            return;

        float angleToPlayer = Vector3.SignedAngle(cannonForward, direction, Vector3.up);

        float rotationStep = Mathf.Clamp(angleToPlayer, -aimSpeed * Time.deltaTime, aimSpeed * Time.deltaTime);

        head.Rotate(Vector3.up, rotationStep, Space.World);

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

        Vector3 shootDirection = (player.position + Vector3.up - muzzle.position).normalized;

        Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(shootDirection));

        SoundFX.Play("launcher_fire", transform.position); 
    }
}