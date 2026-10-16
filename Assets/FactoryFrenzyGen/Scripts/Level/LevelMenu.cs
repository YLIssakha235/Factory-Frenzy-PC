using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// PC-LOB-L1 – main menu: shows one row per level file (.json) found in the Levels folder.
// Our 3 levels (Level1/2/3.json) and the levels made in VR are all in this folder.
// PLAY saves the file name and loads the Race scene, where the LevelLoader builds the level.
// Put it on an empty GameObject "LevelMenu" (or use Factory Frenzy > Build Level Menu).
public class LevelMenu : MonoBehaviour
{
    public Transform listContent;    // the object that holds the rows (has a Vertical Layout Group)
    public GameObject rowTemplate;   // one hidden row that we copy for each level
    public GameObject emptyText;     // "No levels found" message
    public string raceScene = "Race";

    void Start()
    {
        // the player controller locks the mouse during the race: unlock it to click the buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Refresh();
    }

    // REFRESH button: reads the Levels folder again and rebuilds the list
    public void Refresh()
    {
        // 1. remove the old rows (but keep the template)
        foreach (Transform child in listContent)
        {
            if (child.gameObject != rowTemplate) Destroy(child.gameObject);
        }
        rowTemplate.SetActive(false);

        // 2. find all the .json files
        string folder = LevelLoader.GetLevelsFolder();
        List<string> files = new List<string>();
        if (Directory.Exists(folder)) files.AddRange(Directory.GetFiles(folder, "*.json"));
        files.Sort(CompareFiles);

        // 3. one row per file
        int number = 1;
        foreach (string path in files)
        {
            LevelData data = ReadLevel(path);
            if (data == null) continue; // file is not a valid level: skip it

            CreateRow(number, Path.GetFileName(path), data);
            number++;
        }

        if (emptyText != null) emptyText.SetActive(number == 1); // no level found
        Debug.Log("LevelMenu: " + (number - 1) + " levels found in " + folder);
    }

    // our levels (Level1, Level2, Level3) first, then the others in alphabetical order
    int CompareFiles(string a, string b)
    {
        bool aIsOurs = Path.GetFileName(a).StartsWith("Level");
        bool bIsOurs = Path.GetFileName(b).StartsWith("Level");
        if (aIsOurs && !bIsOurs) return -1;
        if (bIsOurs && !aIsOurs) return 1;
        return string.Compare(a, b);
    }

    LevelData ReadLevel(string path)
    {
        try
        {
            return JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
        }
        catch
        {
            Debug.LogWarning("LevelMenu: cannot read " + path + " (not a valid level JSON)");
            return null;
        }
    }

    void CreateRow(int number, string fileName, LevelData data)
    {
        GameObject row = Instantiate(rowTemplate, listContent);
        row.name = "Row " + fileName;
        row.SetActive(true);

        string levelName = data.levelName;
        if (string.IsNullOrEmpty(levelName)) levelName = fileName;
        int objectCount = data.elements != null ? data.elements.Count : 0;

        SetText(row, "NumberBox/Number", number.ToString());
        SetText(row, "Name", levelName);
        SetText(row, "Info", fileName + "  -  " + objectCount + " objects");

        Button play = row.transform.Find("PlayButton").GetComponent<Button>();
        play.onClick.AddListener(() => PlayLevel(fileName));
    }

    void SetText(GameObject row, string childName, string value)
    {
        Transform child = row.transform.Find(childName);
        if (child != null) child.GetComponent<TextMeshProUGUI>().text = value;
    }

    // PLAY button of a row
    public void PlayLevel(string fileName)
    {
        Debug.Log("LevelMenu: play " + fileName);
        PlayerPrefs.SetString("SelectedLevel", fileName); // the LevelLoader reads this in the Race scene
        SceneManager.LoadScene(raceScene);
    }

    // QUIT button (works in the built game, not in the Unity editor)
    public void Quit()
    {
        Application.Quit();
    }
}
