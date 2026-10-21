using UnityEngine;
using StarterAssets;


public class BeanAnimator : MonoBehaviour
{
    Transform visual, armL, armR, legL, legR;
    CharacterController controller;

    float walkTime = 0;        
    float squash = 0;         
    float hitTime = -1;        
    Vector3 hitAxis;           
    float celebrateTime = -1; 
    bool wasOnGround = true;
    Vector3 lastPosition;     

    void Start()
    {
        controller = GetComponentInParent<CharacterController>(); 
        lastPosition = transform.position;
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

    
    Transform FindChild(string childName)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == childName) return t;
        }
        return null;
    }


    public void OnPushed(Vector3 force)
    {
        Vector3 sideways = new Vector3(force.x, 0, force.z);
        if (sideways.magnitude > 8) 
        {
            hitTime = 0;
            squash = 0.35f; 

         
            Vector3 localPush = transform.InverseTransformDirection(sideways.normalized);
            hitAxis = Vector3.Cross(Vector3.up, localPush);
        }
    }

 
    public void Celebrate()
    {
        celebrateTime = 0;
    }

    void Update()
    {
        if (visual == null || armL == null || armR == null || legL == null || legR == null) return;

      
        Vector3 velocity = Vector3.zero;
        if (Time.deltaTime > 0)
        {
            velocity = (transform.position - lastPosition) / Time.deltaTime;
        }
        lastPosition = transform.position;

        bool onGround = true;
        if (controller != null && controller.enabled)
        {
            onGround = controller.isGrounded;
        }
      

        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;

        StarterAssetsInputs input = GetComponentInParent<StarterAssetsInputs>();

        if (input != null && input.move.sqrMagnitude < 0.01f)
        {
            speed = 0f;
        }


        walkTime += Time.deltaTime * speed * 2.5f;
        float swing = Mathf.Sin(walkTime) * Mathf.Min(speed * 10, 45);
        float armLeft = -swing;
        float armRight = swing;

        if (!onGround)
        {
            armLeft = -150;
            armRight = -150;
        }

       
        if (onGround && !wasOnGround)
        {
            squash = 0.3f;
        }
        wasOnGround = onGround;
        squash = Mathf.MoveTowards(squash, 0, Time.deltaTime * 1.5f);

        float hop = 0;
        if (celebrateTime >= 0)
        {
            celebrateTime += Time.deltaTime;
            hop = Mathf.Abs(Mathf.Sin(celebrateTime * 6)) * 0.4f;
            armLeft = -160 + Mathf.Sin(celebrateTime * 14) * 20;
            armRight = -160 - Mathf.Sin(celebrateTime * 14) * 20;
            if (celebrateTime > 3) celebrateTime = -1;
        }

        Quaternion bodyRotation = Quaternion.identity;
        if (hitTime >= 0)
        {
            hitTime += Time.deltaTime;

            if (hitTime < 0.8f)
            {
                float u = hitTime / 0.8f;
                float angle = 360 * (1 - (1 - u) * (1 - u));
                bodyRotation = Quaternion.AngleAxis(angle, hitAxis);

                armLeft = -90 + Mathf.Sin(hitTime * 40) * 60;
                armRight = -90 - Mathf.Sin(hitTime * 40) * 60;
                swing = Mathf.Sin(hitTime * 35) * 50;
            }
            else if (hitTime < 1.4f)
            {
                float fade = 1 - (hitTime - 0.8f) / 0.6f;
                bodyRotation = Quaternion.Euler(0, 0, Mathf.Sin(hitTime * 20) * 15 * fade);
            }
            else
            {
                hitTime = -1; 
            }
        }

        legL.localRotation = Quaternion.Euler(swing, 0, 0);
        legR.localRotation = Quaternion.Euler(-swing, 0, 0);
        armL.localRotation = Quaternion.Euler(armLeft, 0, 0);
        armR.localRotation = Quaternion.Euler(armRight, 0, 0);
        visual.localScale = new Vector3(1 + squash, 1 - squash, 1 + squash);

        Vector3 belly = new Vector3(0, 0.85f, 0);
        visual.localRotation = bodyRotation;
        visual.localPosition = belly - bodyRotation * belly + new Vector3(0, hop, 0);
    }
}