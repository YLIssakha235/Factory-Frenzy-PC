using UnityEngine;

// Put this on the player (PlayerArmature).
// Traps use AddImpulse / AddForce to push the player.
// Checkpoints use SetCheckpoint, the kill zone uses Respawn.
public class PlayerPhysics : MonoBehaviour
{
    public float gravity = -20f;   // pulls the push back down
    public float friction = 3f;    // how fast a sideways push stops

    CharacterController controller;
    Vector3 pushVelocity;          // extra speed given by traps
    Vector3 respawnPosition;       // where we go back when we fall

    void Start()
    {
        controller = GetComponent<CharacterController>();
        respawnPosition = transform.position; // the start is the first checkpoint
    }

    // One push (bumper, trampoline, launcher)
    public void AddImpulse(Vector3 force)
    {
        pushVelocity += force;

        // tell the animation we were pushed (to do a flip)
        BeanAnimator animator = GetComponentInChildren<BeanAnimator>();
        if (animator != null)
        {
            animator.OnPushed(force);
        }
    }

    // A push that lasts (fan): call it every frame while inside the wind
    public void AddForce(Vector3 force)
    {
        pushVelocity += force * Time.deltaTime;
    }

    public void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
    }

    public void Respawn()
    {
        pushVelocity = Vector3.zero;

        // the CharacterController must be off to teleport the player
        controller.enabled = false;
        transform.position = respawnPosition;
        controller.enabled = true;
    }

    void Update()
    {
        // move the player with the push
        controller.Move(pushVelocity * Time.deltaTime);

        // slow down the sideways push
        pushVelocity.x -= pushVelocity.x * friction * Time.deltaTime;
        pushVelocity.z -= pushVelocity.z * friction * Time.deltaTime;

        // gravity on the upward push
        pushVelocity.y += gravity * Time.deltaTime;

        // on the ground: no more falling push
        if (controller.isGrounded && pushVelocity.y < 0)
        {
            pushVelocity.y = 0;
        }
    }
}
