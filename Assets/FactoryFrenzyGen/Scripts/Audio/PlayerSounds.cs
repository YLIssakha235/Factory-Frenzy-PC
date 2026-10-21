using UnityEngine;


public class PlayerSounds : MonoBehaviour
{
    public float stepDistance = 1.6f;   

    Vector3 lastPosition;
    bool wasOnGround = true;
    float fallSpeed;                    
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

        if (jumpDistance > 5f)
        {
            SoundFX.Play("respawn");
            fallSoundPlayed = false;
            airTime = 0;
            return;
        }

        bool onGround = IsOnGround();

        if (wasOnGround && !onGround && velocity.y > 2f) SoundFX.Play("jump");

        if (!wasOnGround && onGround && fallSpeed > 3f) SoundFX.Play("land", Mathf.Clamp01(fallSpeed / 12f));

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

    bool IsOnGround()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.35f, ~0, QueryTriggerInteraction.Ignore);
    }
}
