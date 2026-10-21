using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelMenu : MonoBehaviour
{
    public Transform listContent;    
    public GameObject rowTemplate;   
    public GameObject emptyText;    
    public string raceScene = "Race";

    void Start()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Refresh();
    }


    public void Refresh()
    {
        foreach (Transform child in listContent)
        {
            if (child.gameObject != rowTemplate) Destroy(child.gameObject);
        }
        rowTemplate.SetActive(false);

        
        string folder = LevelLoader.GetLevelsFolder();
        List<string> files = new List<string>();
        if (Directory.Exists(folder)) files.AddRange(Directory.GetFiles(folder, "*.json"));
        files.Sort(CompareFiles);

    
        int number = 1;
        foreach (string path in files)
        {
            LevelData data = ReadLevel(path);
            if (data == null) continue; 

            CreateRow(number, Path.GetFileName(path), data);
            number++;
        }

        if (emptyText != null) emptyText.SetActive(number == 1);
        Debug.Log("LevelMenu: " + (number - 1) + " levels found in " + folder);
    }

    
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
            return LevelData.FromJson(File.ReadAllText(path)); 
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

    
    public void PlayLevel(string fileName)
    {
        Debug.Log("LevelMenu: play " + fileName);
        PlayerPrefs.SetString("SelectedLevel", fileName); 
        SceneManager.LoadScene(raceScene);
    }

   
    public void Quit()
    {
        Application.Quit();
    }
}
