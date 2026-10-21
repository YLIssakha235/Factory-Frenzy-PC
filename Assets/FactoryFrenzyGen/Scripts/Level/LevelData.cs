using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class LevelData
{
    public string levelName;
    public List<LevelElement> elements = new List<LevelElement>();


    public static LevelData FromJson(string json)
    {
  
        if (json.Contains("\"elements\""))
        {
            return JsonUtility.FromJson<LevelData>(json);
        }

        VRLevelData vr = JsonUtility.FromJson<VRLevelData>(json);
        LevelData data = new LevelData();
        data.levelName = vr.levelName;
        foreach (VRLevelObject o in vr.objects)
        {
            LevelElement element = new LevelElement();
            element.type = o.itemType;
            element.position = o.position;
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
    public string type;              
    public Vector3 position;
    public Vector3 rotation;       
    public Vector3 scale = Vector3.one;
    public bool locked;           
    public Vector3 endPosition;     
    public float travelTime;        
}



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
    public Vector3 endPosition;      
    public float travelTime;         
}
