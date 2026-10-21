using System.IO;
using UnityEngine;
using StarterAssets;


public class LevelLoader : MonoBehaviour
{
    public string levelFile = "Level1.json"; 
    public GameObject[] prefabs;            

    void Awake()
    {
        string fileName = PlayerPrefs.GetString("SelectedLevel", levelFile);
        Load(fileName);
    }

    
    public static string GetLevelsFolder()
    {
        string normalFolder = Path.Combine(Application.streamingAssetsPath, "Levels");
        if (Directory.Exists(normalFolder)) return normalFolder;
        return Path.Combine(Application.dataPath, "FactoryFrenzyGen", "StreamingAssets", "Levels");
    }

    public static string GetLevelPath(string fileName)
    {
        return Path.Combine(GetLevelsFolder(), fileName);
    }

    public void Load(string fileName)
    {
        string path = GetLevelPath(fileName);
        if (!File.Exists(path))
        {
            //Debug.LogWarning("LevelLoader: file not found " + path);
            return;
        }

        string json = File.ReadAllText(path);
        LevelData data = LevelData.FromJson(json);

        Build(data, prefabs, transform);
        //Debug.Log("LevelLoader: loaded " + data.levelName + " (" + data.elements.Count + " elements)");
    }

    public static void Build(LevelData data, GameObject[] prefabs, Transform parent)
    {
        float lowestY = 0;
        Transform startPlatform = null;

        foreach (LevelElement element in data.elements)
        {
            GameObject prefab = FindPrefab(prefabs, element.type);
            if (prefab == null)
            {
                //Debug.LogWarning("LevelLoader: no prefab called " + element.type);
                continue;
            }

            GameObject go = Instantiate(prefab, element.position, Quaternion.Euler(element.rotation), parent);
            go.name = element.type;
            go.transform.localScale = element.scale;

            go.SendMessage("SetPath", element, SendMessageOptions.DontRequireReceiver);

            if (element.type == "Checkpoint")
            {
                Transform trigger = go.transform.Find("CheckpointTrigger");
                if (trigger != null && trigger.GetComponent<BoxCollider>() != null)
                {
                    trigger.GetComponent<BoxCollider>().size = new Vector3(3.6f, 3f, 3.6f);
                    trigger.GetComponent<BoxCollider>().center = new Vector3(0, 1.5f, 0);
                }
            }

            if (element.type == "Start") startPlatform = go.transform;
            if (element.position.y < lowestY) lowestY = element.position.y;
        }

        GameObject killZonePrefab = FindPrefab(prefabs, "KillZone");
        if (killZonePrefab != null)
        {
            GameObject killZone = Instantiate(killZonePrefab, new Vector3(0, lowestY - 8, 0), Quaternion.identity, parent);
            killZone.name = "KillZone";
            killZone.transform.localScale = new Vector3(1, 10, 1);
        }

        if (startPlatform != null)
        {
            PlacePlayer(startPlatform);
        }
    }

    static GameObject FindPrefab(GameObject[] prefabs, string type)
    {
        if (prefabs == null) return null;
        foreach (GameObject prefab in prefabs)
        {
            if (prefab != null && prefab.name == type) return prefab;
        }
        return null;
    }

    static void PlacePlayer(Transform startPlatform)
    {
        ThirdPersonController player = FindObjectOfType<ThirdPersonController>();
        if (player == null) return;

        Transform spawn = startPlatform.Find("SpawnPoint");
        Vector3 position = startPlatform.position + Vector3.up;
        if (spawn != null) position = spawn.position + Vector3.up * 0.1f;

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        player.transform.position = position;
        player.transform.rotation = startPlatform.rotation;
        if (controller != null) controller.enabled = true;

        player.SetRespawnPosition(position); 
    }
}
