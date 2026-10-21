using System.Collections;
using UnityEngine;


public class LevelIntro : MonoBehaviour
{
    public float duration = 5f;  
    public float turns = 0.75f;  

    public IEnumerator Play()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

     
        Behaviour brain = cam.GetComponent("CinemachineBrain") as Behaviour;
        if (brain != null) brain.enabled = false;

        Bounds level = GetLevelBounds();
        Vector3 center = level.center;
        float levelSize = Mathf.Max(level.size.x, level.size.z);
        float radius = levelSize * 0.6f + 10;
        float height = levelSize * 0.35f + 8;

        CharacterController player = FindObjectOfType<CharacterController>();
        Vector3 endPosition = center;
        Vector3 endLookAt = center;
        if (player != null)
        {
            endPosition = player.transform.position - player.transform.forward * 5 + Vector3.up * 2.5f;
            endLookAt = player.transform.position + Vector3.up * 1.2f;
        }

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float u = time / duration; 

           
            float angle = u * turns * 360;
            Vector3 orbitPosition = center + Quaternion.Euler(0, angle, 0) * new Vector3(0, height, -radius);

            float landing = Mathf.Clamp01((u - 0.7f) / 0.3f);
            landing = landing * landing * (3 - 2 * landing); 

            cam.transform.position = Vector3.Lerp(orbitPosition, endPosition, landing);
            Vector3 lookAt = Vector3.Lerp(center, endLookAt, landing);
            cam.transform.LookAt(lookAt);

            yield return null;
        }

        if (brain != null) brain.enabled = true;
    }
   
    Bounds GetLevelBounds()
    {
        GameObject levelRoot = null;
        LevelLoader loader = FindObjectOfType<LevelLoader>();
        if (loader != null) levelRoot = loader.gameObject;
        if (levelRoot == null) levelRoot = GameObject.Find("Level");

        Bounds box = new Bounds(Vector3.zero, Vector3.zero);
        bool first = true;
        if (levelRoot != null)
        {
            foreach (Renderer part in levelRoot.GetComponentsInChildren<Renderer>())
            {
                if (!part.enabled) continue;                        
                if (part.GetComponentInParent<KillZone>() != null) continue;
                if (first) { box = part.bounds; first = false; }
                else box.Encapsulate(part.bounds);                 
            }
        }

        if (first)
        {
            StartLightsFX start = FindObjectOfType<StartLightsFX>();
            FinishFX finish = FindObjectOfType<FinishFX>();
            if (start != null) box = new Bounds(start.transform.position, Vector3.one);
            if (finish != null) box.Encapsulate(finish.transform.position);
        }
        return box;
    }
}