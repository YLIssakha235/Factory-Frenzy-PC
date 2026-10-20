using System;
using System.Collections.Generic;
using UnityEngine;

// The level file format, shared by the VR editor (export) and the PC game (import).
// Saved as JSON with JsonUtility.

[Serializable]
public class LevelData
{
    public string levelName;
    public List<LevelElement> elements = new List<LevelElement>();

    // Reads a level file. Works with BOTH formats:
    //   - our format:        "elements", "type", rotation in degrees (x, y, z)
    //   - the VR editor's:   "objects", "itemType", rotation as a quaternion (x, y, z, w), "isLocked"
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
            // quaternion -> degrees. We keep only the turn around Y: a platform tilted by the hand
            // (a few degrees in X or Z) would make the player slide, so it is put back flat.
            element.rotation = new Vector3(0, o.rotation.eulerAngles.y, 0);
            element.scale = o.scale;
            element.locked = o.isLocked;
            element.endPosition = o.endPosition;
            element.travelTime = o.travelTime;
            data.elements.Add(element);
        }
        return data;
    }
}

[Serializable]
public class LevelElement
{
    public string type;              // prefab name: Start, Finish, Classic, Moving, Trampoline,
                                     // Checkpoint, Bumper, Launcher, Fan
    public Vector3 position;
    public Vector3 rotation;         // euler angles in degrees
    public Vector3 scale = Vector3.one;
    public bool locked;              // used by the VR editor only
    public Vector3 endPosition;      // Moving platform only
    public float travelTime;         // Moving platform only (seconds)
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
    public Vector3 endPosition;      // not in his file yet (moving platform)
    public float travelTime;         // not in his file yet (moving platform)
}
