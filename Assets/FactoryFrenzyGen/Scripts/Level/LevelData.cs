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
