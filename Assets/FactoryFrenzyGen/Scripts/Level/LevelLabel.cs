using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLabel : MonoBehaviour
{
    void Start()
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        if (text == null) return;

        string name;
        LevelLoader loader = FindObjectOfType<LevelLoader>();
        if (loader != null)
        {
            name = PlayerPrefs.GetString("SelectedLevel", loader.levelFile);
            name = Path.GetFileNameWithoutExtension(name);
        }
        else
        {
            name = SceneManager.GetActiveScene().name;
        }

        text.text = MakeLabel(name);
    }

   
    string MakeLabel(string name)
    {
        string lower = name.ToLower().Replace(" ", "").Replace("_", "");
        if (lower.StartsWith("level"))
        {
            string number = "";
            foreach (char c in lower.Substring(5))
            {
                if (char.IsDigit(c)) number += c;
                else break;
            }
            if (number != "") return "LEVEL " + number;
        }
        return "VR LEVEL " ;
    }
}
