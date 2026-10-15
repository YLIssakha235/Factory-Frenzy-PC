using UnityEngine;

// Animates the bean character with code (no animation files).
// Put it on the PlayerBean object (child of PlayerArmature).
public class BeanAnimator : MonoBehaviour
{
    Transform visual, armL, armR, legL, legR;
    CharacterController controller;

    float walkTime = 0;        // advances when we walk, used for the leg swing
    float squash = 0;          // squash when we land
    float flipTime = -1;       // -1 = not flipping
    float celebrateTime = -1;  // -1 = not celebrating
    bool wasOnGround = true;

    void Start()
    {
        controller = GetComponentInParent<CharacterController>(); // can be null in a test scene
        visual = FindChild("Visual");
        armL = FindChild("ArmL");
        armR = FindChild("ArmR");
        legL = FindChild("LegL");
        legR = FindChild("LegR");

        if (visual == null || armL == null || armR == null || legL == null || legR == null)
        {
            Debug.LogWarning("BeanAnimator: put this script on the PlayerBean (it needs Visual, ArmL, ArmR, LegL, LegR).");
        }
    }

    // looks for a child with this name, at any depth
    Transform FindChild(string childName)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == childName) return t;
        }
        return null;
    }

    // Called by PlayerPhysics when a trap pushes us
    public void OnPushed(Vector3 force)
    {
        Vector3 sideways = new Vector3(force.x, 0, force.z);
        if (sideways.magnitude > 8) // only big sideways hits (not the trampoline)
        {
            flipTime = 0;
        }
    }

    // Call it when the player finishes the race
    public void Celebrate()
    {
        celebrateTime = 0;
    }

    void Update()
    {
        if (visual == null || armL == null || armR == null || legL == null || legR == null) return;

        // without the player around it (test scene), the bean just stands still
        Vector3 velocity = Vector3.zero;
        bool onGround = true;
        if (controller != null)
        {
            velocity = controller.velocity;
            onGround = controller.isGrounded;
        }
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;

        // 1. WALK / RUN: legs and arms swing, faster when we go faster
        walkTime += Time.deltaTime * speed * 2.5f;
        float swing = Mathf.Sin(walkTime) * Mathf.Min(speed * 10, 45);
        float armLeft = -swing;
        float armRight = swing;

        // 2. IN THE AIR: arms up
        if (!onGround)
        {
            armLeft = -150;
            armRight = -150;
        }

        // 3. LANDING: squash the body a little
        if (onGround && !wasOnGround)
        {
            squash = 0.3f;
        }
        wasOnGround = onGround;
        squash = Mathf.MoveTowards(squash, 0, Time.deltaTime * 1.5f);

        // 4. HIT: one forward flip in 0.6 seconds
        float flip = 0;
        if (flipTime >= 0)
        {
            flipTime += Time.deltaTime;
            flip = flipTime / 0.6f * 360;
            if (flipTime > 0.6f) flipTime = -1;
        }

        // 5. CELEBRATE: hop and wave for 3 seconds
        float hop = 0;
        if (celebrateTime >= 0)
        {
            celebrateTime += Time.deltaTime;
            hop = Mathf.Abs(Mathf.Sin(celebrateTime * 6)) * 0.4f;
            armLeft = -160 + Mathf.Sin(celebrateTime * 14) * 20;
            armRight = -160 - Mathf.Sin(celebrateTime * 14) * 20;
            if (celebrateTime > 3) celebrateTime = -1;
        }

        // apply everything
        legL.localRotation = Quaternion.Euler(swing, 0, 0);
        legR.localRotation = Quaternion.Euler(-swing, 0, 0);
        armL.localRotation = Quaternion.Euler(armLeft, 0, 0);
        armR.localRotation = Quaternion.Euler(armRight, 0, 0);
        visual.localScale = new Vector3(1 + squash, 1 - squash, 1 + squash);
        visual.localPosition = new Vector3(0, hop, 0);
        visual.localRotation = Quaternion.Euler(flip, 0, 0);
    }
}
