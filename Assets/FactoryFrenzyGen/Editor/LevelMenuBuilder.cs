// Factory Frenzy – editor tools for the menu and the race scene. Put this file in Assets/Editor/.
// It is a TOOL (like the Race UI builder): it does not run in the game.
//
//  Factory Frenzy > Setup Race Scene   (open the Race scene first)
//      removes the hand-placed "Level" and adds a LevelLoader with all the prefabs
//  Factory Frenzy > Build Level Menu   (open the Menu scene first)
//      creates the menu screen (title, list of levels, Refresh, Quit), links it to LevelMenu
//      and adds Menu + Race to File > Build Settings

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class LevelMenuBuilder
{
    const string SpriteFolder = "Assets/UI/Sprites/";
    const string PrefabFolder = "Assets/FactoryFrenzyGen/Prefabs";

    static readonly Color Yellow = new Color(1f, 0.79f, 0.24f);        // #FFC93C
    static readonly Color Grey = new Color(0.56f, 0.6f, 0.69f);        // #8F9AB0
    static readonly Color Background = new Color(0.11f, 0.13f, 0.2f);  // #1B2233
    static readonly Color RowColor = new Color(0.17f, 0.2f, 0.27f);    // #2B3346

    static TMP_FontAsset font;

    // ================================================================ RACE SCENE

    [MenuItem("Factory Frenzy/Setup Race Scene")]
    static void SetupRaceScene()
    {
        // the level is now built from the JSON: remove the level placed by hand
        GameObject oldLevel = GameObject.Find("Level");
        if (oldLevel != null) Undo.DestroyObjectImmediate(oldLevel);

        LevelLoader loader = Object.FindObjectOfType<LevelLoader>();
        if (loader == null) loader = new GameObject("LevelLoader").AddComponent<LevelLoader>();

        List<GameObject> prefabs = new List<GameObject>();
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder }))
        {
            prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)));
        }
        loader.prefabs = prefabs.ToArray();

        EditorUtility.SetDirty(loader);
        EditorSceneManager.MarkSceneDirty(loader.gameObject.scene);
        Selection.activeGameObject = loader.gameObject;
        EditorUtility.DisplayDialog("Factory Frenzy", "LevelLoader ready with " + prefabs.Count + " prefabs.\nThe level will be built from the JSON when you press Play.\n\nSave this scene as \"Race\" (Ctrl+S).", "OK");
    }

    // ================================================================ MENU

    [MenuItem("Factory Frenzy/Build Level Menu")]
    static void BuildMenu()
    {
        font = FindFont();

        GameObject old = GameObject.Find("LevelMenuUI");
        if (old != null) Object.DestroyImmediate(old);

        LevelMenu menu = Object.FindObjectOfType<LevelMenu>();
        if (menu == null) menu = new GameObject("LevelMenu").AddComponent<LevelMenu>();

        // ---------- Canvas ----------
        GameObject canvasGO = new GameObject("LevelMenuUI", typeof(RectTransform));
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        Transform root = canvasGO.transform;
        MakeEventSystem();

        Vector2 center = new Vector2(0.5f, 0.5f);

        GameObject background = MakeImage("Background", root, null, center, Vector2.zero, Vector2.zero);
        Stretch(background, 0);
        background.GetComponent<Image>().color = Background;

        MakeText("Title", root, "FACTORY FRENZY", 130, Yellow, center, new Vector2(0, 405), new Vector2(1600, 160));
        GameObject banner = MakeImage("ChooseBanner", root, "banner_pink", center, new Vector2(0, 260), new Vector2(600, 110));
        MakeText("ChooseText", banner.transform, "CHOOSE A LEVEL", 52, Color.white, center, new Vector2(0, 6), new Vector2(560, 90));

        // ---------- panel with the scrolling list ----------
        GameObject panel = MakeImage("Panel", root, "panel", center, new Vector2(0, -105), new Vector2(1200, 560));

        GameObject scroll = new GameObject("LevelList", typeof(RectTransform));
        scroll.transform.SetParent(panel.transform, false);
        Stretch(scroll, 40);
        ScrollRect scrollRect = scroll.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 30;

        GameObject viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(scroll.transform, false);
        Stretch(viewport, 0);
        viewport.AddComponent<RectMask2D>(); // hides the rows outside the panel

        GameObject content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 18;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = contentRect;

        GameObject rowTemplate = MakeRowTemplate(content.transform);

        TextMeshProUGUI empty = MakeText("EmptyText", panel.transform, "No levels found\n<size=60%>Put .json files in the Levels folder</size>", 48, Grey, center, Vector2.zero, new Vector2(1000, 200));
        empty.gameObject.SetActive(false);

        // ---------- buttons ----------
        Button refresh = MakeButton("RefreshButton", root, "button_blue", "REFRESH", new Vector2(-175, -465), new Vector2(330, 110), 48);
        Button quit = MakeButton("QuitButton", root, "button_grey", "QUIT", new Vector2(175, -465), new Vector2(330, 110), 48);
        UnityEventTools.AddPersistentListener(refresh.onClick, new UnityAction(menu.Refresh));
        UnityEventTools.AddPersistentListener(quit.onClick, new UnityAction(menu.Quit));

        // ---------- link to the LevelMenu script ----------
        menu.listContent = content.transform;
        menu.rowTemplate = rowTemplate;
        menu.emptyText = empty.gameObject;

        AddScenesToBuildSettings(menu.raceScene);

        EditorUtility.SetDirty(menu);
        EditorSceneManager.MarkSceneDirty(menu.gameObject.scene);
        Selection.activeGameObject = canvasGO;
        EditorUtility.DisplayDialog("Factory Frenzy", "Level menu created.\n\nSave this scene as \"Menu\" (Ctrl+S).\nFile > Build Settings must have: Menu (first), Race.", "OK");
    }

    // one row: [number]  Level name / file - objects  [PLAY]   (hidden, the script copies it)
    static GameObject MakeRowTemplate(Transform parent)
    {
        GameObject row = MakeImage("RowTemplate", parent, null, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0, 118));
        row.GetComponent<Image>().color = RowColor;
        row.AddComponent<LayoutElement>().preferredHeight = 118;

        Vector2 left = new Vector2(0, 0.5f), right = new Vector2(1, 0.5f);

        GameObject numberBox = MakeImage("NumberBox", row.transform, null, left, new Vector2(70, 0), new Vector2(84, 84));
        numberBox.GetComponent<Image>().color = Background;
        MakeText("Number", numberBox.transform, "1", 44, Yellow, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(84, 84));

        TextMeshProUGUI levelName = MakeText("Name", row.transform, "Level name", 46, Color.white, left, new Vector2(510, 18), new Vector2(700, 60));
        levelName.alignment = TextAlignmentOptions.Left;
        TextMeshProUGUI info = MakeText("Info", row.transform, "Level.json  -  0 objects", 26, Grey, left, new Vector2(510, -30), new Vector2(700, 36));
        info.alignment = TextAlignmentOptions.Left;

        MakeButton("PlayButton", row.transform, "button_green", "PLAY", right, new Vector2(-130, 0), new Vector2(220, 92), 44);

        row.SetActive(false);
        return row;
    }

    // ================================================================ build settings

    // a scene can only be loaded if it is in File > Build Settings: Menu first, then Race, then the others
    static void AddScenesToBuildSettings(string raceScene)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

        string menuPath = EditorSceneManager.GetActiveScene().path;
        if (menuPath != "") scenes.Add(new EditorBuildSettingsScene(menuPath, true));
        else Debug.LogWarning("Level Menu: save this scene as \"Menu\", then click Build Level Menu again.");

        string racePath = FindScene(raceScene);
        if (racePath != null) scenes.Add(new EditorBuildSettingsScene(racePath, true));
        else Debug.LogWarning("Level Menu: no scene called \"" + raceScene + "\". Make it first (Setup Race Scene).");

        // keep the other scenes that were already there (Level1, Level2...)
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.path != menuPath && scene.path != racePath) scenes.Add(scene);
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    static string FindScene(string sceneName)
    {
        foreach (string guid in AssetDatabase.FindAssets(sceneName + " t:Scene"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path) == sceneName) return path;
        }
        return null;
    }

    // ================================================================ helpers (same as the Race UI builder)

    static TMP_FontAsset FindFont()
    {
        string[] guids = AssetDatabase.FindAssets("Lilita t:TMP_FontAsset");
        if (guids.Length > 0) return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
        return TMP_Settings.defaultFontAsset;
    }

    static void MakeEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        // Starter Assets uses the new Input System: use its UI module if it exists
        System.Type inputModule = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputModule != null) es.AddComponent(inputModule);
        else es.AddComponent<StandaloneInputModule>();
    }

    // fills the parent, with a margin
    static void Stretch(GameObject go, float margin)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(margin, margin);
        rect.offsetMax = new Vector2(-margin, -margin);
    }

    static RectTransform Place(GameObject go, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    static GameObject MakeImage(string name, Transform parent, string spriteName, Vector2 anchor, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        Place(go, parent, anchor, position, size);
        Image image = go.AddComponent<Image>();
        if (spriteName != null)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpriteFolder + spriteName + ".png");
            if (sprite == null) Debug.LogWarning("Level Menu: sprite not found: " + SpriteFolder + spriteName + ".png");
            image.sprite = sprite;
            if (sprite != null && sprite.border != Vector4.zero) image.type = Image.Type.Sliced;
        }
        return go;
    }

    static TextMeshProUGUI MakeText(string name, Transform parent, string text, float fontSize, Color color, Vector2 anchor, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        Place(go, parent, anchor, position, size);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget = false;
        return tmp;
    }

    static Button MakeButton(string name, Transform parent, string spriteName, string label, Vector2 anchor, Vector2 position, Vector2 size, float fontSize)
    {
        GameObject go = MakeImage(name, parent, spriteName, anchor, position, size);
        Button button = go.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.94f, 0.94f, 0.94f);
        colors.pressedColor = new Color(0.78f, 0.78f, 0.78f);
        button.colors = colors;
        MakeText("Text", go.transform, label, fontSize, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0, 5), size - new Vector2(20, 10));
        return button;
    }

    static Button MakeButton(string name, Transform parent, string spriteName, string label, Vector2 position, Vector2 size, float fontSize)
    {
        return MakeButton(name, parent, spriteName, label, new Vector2(0.5f, 0.5f), position, size, fontSize);
    }
}
#endif
