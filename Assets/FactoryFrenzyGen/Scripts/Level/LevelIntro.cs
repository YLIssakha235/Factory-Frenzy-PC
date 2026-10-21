using System.Collections;
using UnityEngine;

// Camera flight around the whole level before the countdown.
// Put it on an empty GameObject called "LevelIntro". RaceManager plays it automatically.
public class LevelIntro : MonoBehaviour
{
    public float duration = 5f;  // seconds
    public float turns = 0.75f;  // 0.75 = three quarters of a circle

    public IEnumerator Play()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        // Cinemachine normally moves the camera: we turn it off during the intro
        // (found by its name, so it works with every Cinemachine version)
        Behaviour brain = cam.GetComponent("CinemachineBrain") as Behaviour;
        if (brain != null) brain.enabled = false;

        // the box that contains ALL the objects of the level (works for any level, even one made in VR)
        Bounds level = GetLevelBounds();
        Vector3 center = level.center;
        float levelSize = Mathf.Max(level.size.x, level.size.z);
        float radius = levelSize * 0.6f + 10;
        float height = levelSize * 0.35f + 8;

        // where the camera must end: behind the player
        // (we look for the CharacterController: the ThirdPersonController is turned off during the intro)
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
            float u = time / duration; // 0 at the start, 1 at the end

            // 1. turn around the level, high in the sky
            float angle = u * turns * 360;
            Vector3 orbitPosition = center + Quaternion.Euler(0, angle, 0) * new Vector3(0, height, -radius);

            // 2. in the last 30%, come down smoothly behind the player
            float landing = Mathf.Clamp01((u - 0.7f) / 0.3f);
            landing = landing * landing * (3 - 2 * landing); // smooth start and stop

            cam.transform.position = Vector3.Lerp(orbitPosition, endPosition, landing);
            Vector3 lookAt = Vector3.Lerp(center, endLookAt, landing);
            cam.transform.LookAt(lookAt);

            yield return null; // wait for the next frame
        }

        // give the camera back to Cinemachine (it follows the player again)
        if (brain != null) brain.enabled = true;
    }
    // Finds the level objects (made by the LevelLoader, or the "Level" object placed by hand)
    // and returns the box around all their visible parts.
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
                if (!part.enabled) continue;                         // e.g. the invisible kill zone
                if (part.GetComponentInParent<KillZone>() != null) continue;
                if (first) { box = part.bounds; first = false; }
                else box.Encapsulate(part.bounds);                  // grow the box to contain this part
            }
        }

        if (first) // nothing found: use the Start and Finish like before
        {
            StartLightsFX start = FindObjectOfType<StartLightsFX>();
            FinishFX finish = FindObjectOfType<FinishFX>();
            if (start != null) box = new Bounds(start.transform.position, Vector3.one);
            if (finish != null) box.Encapsulate(finish.transform.position);
        }
        return box;
    }
}