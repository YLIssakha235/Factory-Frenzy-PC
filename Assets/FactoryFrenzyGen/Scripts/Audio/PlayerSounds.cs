using UnityEngine;

// Factory Frenzy – jump, landing, footsteps, fall and respawn sounds for the player.
// Put it on PlayerArmature (the object with the CharacterController).
// It only WATCHES the player's movement, so no other script needs to be changed.
public class PlayerSounds : MonoBehaviour
{
    public float stepDistance = 1.6f;   // meters between two footsteps

    Vector3 lastPosition;
    bool wasOnGround = true;
    float fallSpeed;                    // how fast we were falling (for the landing sound)
    float airTime;
    bool fallSoundPlayed;
    float distanceSinceStep;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (Time.deltaTime <= 0) return;
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        float jumpDistance = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // RESPAWN: the player moved more than 5 m in one frame = teleported back to a checkpoint
        if (jumpDistance > 5f)
        {
            SoundFX.Play("respawn");
            fallSoundPlayed = false;
            airTime = 0;
            return;
        }

        bool onGround = IsOnGround();

        // JUMP: we just left the ground going up
        if (wasOnGround && !onGround && velocity.y > 2f) SoundFX.Play("jump");

        // LAND: we just touched the ground after falling
        if (!wasOnGround && onGround && fallSpeed > 3f) SoundFX.Play("land", Mathf.Clamp01(fallSpeed / 12f));

        // FALL: in the air for a long time and falling fast = we missed the platform
        if (onGround)
        {
            airTime = 0;
            fallSoundPlayed = false;
        }
        else
        {
            airTime += Time.deltaTime;
            if (!fallSoundPlayed && airTime > 0.8f && velocity.y < -8f)
            {
                SoundFX.Play("fall", 0.7f);
                fallSoundPlayed = true;
            }
        }

        // FOOTSTEPS: one step every stepDistance meters walked on the ground
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        if (onGround && horizontalSpeed > 0.5f)
        {
            distanceSinceStep += horizontalSpeed * Time.deltaTime;
            if (distanceSinceStep > stepDistance)
            {
                distanceSinceStep = 0;
                SoundFX.Play("footstep_" + Random.Range(1, 4), 0.5f);
            }
        }

        if (!onGround) fallSpeed = -velocity.y;
        wasOnGround = onGround;
    }

    // a short ray down from the feet (a ray that starts inside our own collider does not hit it)
    bool IsOnGround()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.35f, ~0, QueryTriggerInteraction.Ignore);
    }
}
