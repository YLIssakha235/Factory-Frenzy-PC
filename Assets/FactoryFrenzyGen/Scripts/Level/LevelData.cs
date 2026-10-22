using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string levelName;
    public List<LevelElement> elements = new List<LevelElement>();

   
    public static Vector3 vrMoveAxis = Vector3.right;
    public static float vrMoveDistance = 5f;
    public static float vrMoveTime = 3f;

    public static LevelData FromJson(string json)
    {
        // our format
        if (json.Contains("\"elements\""))
        {
            return JsonUtility.FromJson<LevelData>(json);
        }

        // VR editor format -> converted to our format
        VRLevelData vr = JsonUtility.FromJson<VRLevelData>(json);
        LevelData data = new LevelData();
        data.levelName = vr.levelName;
        foreach (VRLevelObject o in vr.objects)
        {
            LevelElement element = new LevelElement();
            element.type = o.itemType;
            element.position = o.position;
            // keep only the turn around Y: a platform tilted by the hand would make the player slide
            element.rotation = new Vector3(0, o.rotation.eulerAngles.y, 0);
            element.scale = o.scale;
            element.locked = o.isLocked;
            element.endPosition = o.endPosition;
            element.travelTime = o.travelTime;

            // [P1 fix] VR files have no path for Moving platforms: endPosition was (0,0,0),
            // so the platform flew towards the centre of the world. Give it a path around where it was placed.
            if (element.type == "Moving" && element.travelTime <= 0f)
            {
                Vector3 half = Quaternion.Euler(element.rotation) * vrMoveAxis.normalized * (vrMoveDistance / 2f);
                Vector3 placed = element.position;
                element.position = placed - half;      // one end of the path
                element.endPosition = placed + half;   // the other end
                element.travelTime = vrMoveTime;
            }

            data.elements.Add(element);
        }
        return data;
    }
}

[Serializable]
public class LevelElement
{
    public string type;              // prefab name: Start, Finish, Classic, Moving, Trampoline, Checkpoint, Bumper, Launcher, Fan
    public Vector3 position;
    public Vector3 rotation;         // euler angles in degrees
    public Vector3 scale = Vector3.one;
    public bool locked;              // used by the VR editor only
    public Vector3 endPosition;      // Moving platform only
    public float travelTime;         // Moving platform only (seconds, one way)
}

// ---------- the format written by the VR editor (Person 3)

[Serializable]
public class VRLevelData
{
    public string levelName;
    public List<VRLevelObject> objects = new List<VRLevelObject>();
}

[Serializable]
public class VRLevelObject
{
    public string itemType;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale = Vector3.one;
    public bool isLocked;
    public Vector3 endPosition;      // not in the VR file yet
    public float travelTime;         // not in the VR file yet
}