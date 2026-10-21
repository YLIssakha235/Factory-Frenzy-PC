// Factory Frenzy – builds the Settings panel (sound) and the Pause menu, in the same style as the rest of the UI.
// Put this file in Assets/Editor/. It is an editor TOOL: it does not run in the game.
//
//   Open the Race scene  -> Factory Frenzy > Build Pause + Settings  : pause button, pause menu, settings panel (in RaceUI)
//   Open the Menu scene  -> Factory Frenzy > Build Pause + Settings  : SETTINGS button + settings panel (in LevelMenuUI)
// Run it again at any time: it replaces what it made before.

#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class PauseSettingsBuilder
{
    const string SpriteFolder = "Assets/UI/Sprites/";
    static readonly Color Yellow = new Color(1f, 0.79f, 0.24f);
    static readonly Color Grey = new Color(0.6f, 0.64f, 0.74f);
    static readonly Color Track = new Color(0.17f, 0.2f, 0.27f);
    static readonly Color Dim = new Color(0.04f, 0.05f, 0.09f, 0.6f);
    static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
    static TMP_FontAsset font;

    [MenuItem("Factory Frenzy/Build Pause + Settings")]
    static void Build()
    {
        font = FindFont();
        MakeEventSystem();

        GameObject raceUI = GameObject.Find("RaceUI");
        GameObject menuUI = GameObject.Find("LevelMenuUI");
        if (raceUI == null && menuUI == null)
        {
            EditorUtility.DisplayDialog("Factory Frenzy", "No RaceUI or LevelMenuUI in this scene.\nBuild the Race UI or the Level Menu first.", "OK");
            return;
        }

        Transform canvas = (raceUI != null ? raceUI : menuUI).transform;

        // remove what this tool made before (Transform.Find also finds hidden panels, GameObject.Find does not)
        foreach (string old in new[] { "SettingsPanel", "PausePanel", "PauseButton", "SettingsButton" })
        {
            Transform found = canvas.Find(old);
            if (found != null) Object.DestroyImmediate(found.gameObject);
        }
        GameObject oldPause = GameObject.Find("PauseMenu");
        if (oldPause != null) Object.DestroyImmediate(oldPause);
        SettingsPanel settings = BuildSettingsPanel(canvas);

        if (raceUI != null) BuildPause(canvas, settings);
        else BuildMenuButton(canvas, settings);

        settings.gameObject.SetActive(false);
        settings.transform.SetAsLastSibling(); // always drawn on top
        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        EditorUtility.DisplayDialog("Factory Frenzy", (raceUI != null ? "Pause menu + settings added to RaceUI." : "Settings button + panel added to the menu.") + "\n\nSave the scene (Ctrl+S).", "OK");
    }

    // ------------------------------------------------------------------ SETTINGS PANEL

    static SettingsPanel BuildSettingsPanel(Transform canvas)
    {
        GameObject root = FullScreenDim("SettingsPanel", canvas);
        SettingsPanel panel = root.AddComponent<SettingsPanel>();

        MakeImage("Panel", root.transform, "panel", Center, new Vector2(0, -10), new Vector2(920, 660));
        GameObject banner = MakeImage("TitleBanner", root.transform, "banner_pink", Center, new Vector2(0, 300), new Vector2(560, 120));
        MakeText("Title", banner.transform, "SETTINGS", 64, Color.white, Center, new Vector2(0, 6), new Vector2(520, 100));

        MakeRow(root.transform, "All", "ALL SOUNDS", 150, out panel.allToggle, out panel.allSlider, out panel.allValue);
        MakeRow(root.transform, "Music", "MUSIC", 20, out panel.musicToggle, out panel.musicSlider, out panel.musicValue);
        MakeRow(root.transform, "Effects", "EFFECTS", -110, out panel.effectsToggle, out panel.effectsSlider, out panel.effectsValue);

        Button back = MakeButton("BackButton", root.transform, "button_yellow", "BACK", new Vector2(0, -250), new Vector2(300, 100), 50);
        UnityEventTools.AddPersistentListener(back.onClick, new UnityAction(panel.Close));
        EditorUtility.SetDirty(panel);
        return panel;
    }

    // one row:  LABEL   [toggle]   [------slider------]   80%
    static void MakeRow(Transform parent, string name, string label, float y, out Toggle toggle, out Slider slider, out TextMeshProUGUI value)
    {
        TextMeshProUGUI text = MakeText(name + "Label", parent, label, 40, Color.white, Center, new Vector2(-280, y), new Vector2(260, 70));
        text.alignment = TextAlignmentOptions.Left;
        toggle = MakeToggle(name + "Toggle", parent, new Vector2(-110, y));
        slider = MakeSlider(name + "Slider", parent, new Vector2(120, y), new Vector2(340, 36));
        value = MakeText(name + "Value", parent, "100%", 36, Yellow, Center, new Vector2(355, y), new Vector2(120, 60));
    }

    // ------------------------------------------------------------------ PAUSE (race scene)

    static void BuildPause(Transform canvas, SettingsPanel settings)
    {
        PauseMenu pause = new GameObject("PauseMenu").AddComponent<PauseMenu>();
        pause.settings = settings;

        // small pause button under the best time (for the mouse)
        Button pauseButton = MakeButton("PauseButton", canvas, "button_grey", "II", new Vector2(-90, -200), new Vector2(110, 96), 48, new Vector2(1, 1));
        UnityEventTools.AddPersistentListener(pauseButton.onClick, new UnityAction(pause.Pause));

        GameObject panel = FullScreenDim("PausePanel", canvas);
        MakeImage("Panel", panel.transform, "panel", Center, new Vector2(0, -20), new Vector2(640, 800));
        GameObject banner = MakeImage("TitleBanner", panel.transform, "banner_pink", Center, new Vector2(0, 340), new Vector2(520, 120));
        MakeText("Title", banner.transform, "PAUSED", 64, Color.white, Center, new Vector2(0, 6), new Vector2(480, 100));

        Vector2 size = new Vector2(440, 100);
        Button resume = MakeButton("ResumeButton", panel.transform, "button_green", "RESUME", new Vector2(0, 190), size, 50);
        Button settingsButton = MakeButton("SettingsButton", panel.transform, "button_blue", "SETTINGS", new Vector2(0, 70), size, 50);
        Button restart = MakeButton("RestartButton", panel.transform, "button_yellow", "RESTART", new Vector2(0, -50), size, 50);
        Button menu = MakeButton("MenuButton", panel.transform, "button_pink", "MAIN MENU", new Vector2(0, -170), size, 50);
        Button quit = MakeButton("QuitButton", panel.transform, "button_grey", "QUIT GAME", new Vector2(0, -290), size, 50);

        UnityEventTools.AddPersistentListener(resume.onClick, new UnityAction(pause.Resume));
        UnityEventTools.AddPersistentListener(settingsButton.onClick, new UnityAction(pause.OpenSettings));
        UnityEventTools.AddPersistentListener(restart.onClick, new UnityAction(pause.Restart));
        UnityEventTools.AddPersistentListener(menu.onClick, new UnityAction(pause.GoToMenu));
        UnityEventTools.AddPersistentListener(quit.onClick, new UnityAction(pause.QuitGame));

        pause.pausePanel = panel;
        panel.SetActive(false);
        panel.transform.SetAsLastSibling();
        EditorUtility.SetDirty(pause);
    }

    // ------------------------------------------------------------------ SETTINGS button (menu scene)

    static void BuildMenuButton(Transform canvas, SettingsPanel settings)
    {
        // make room: REFRESH left, SETTINGS middle, QUIT right
        MoveButton(canvas, "RefreshButton", new Vector2(-360, -465), new Vector2(320, 110));
        MoveButton(canvas, "QuitButton", new Vector2(360, -465), new Vector2(320, 110));
        Button button = MakeButton("SettingsButton", canvas, "button_yellow", "SETTINGS", new Vector2(0, -465), new Vector2(320, 110), 48);
        UnityEventTools.AddPersistentListener(button.onClick, new UnityAction(settings.Open));
    }

    static void MoveButton(Transform canvas, string name, Vector2 position, Vector2 size)
    {
        Transform t = canvas.Find(name);
        if (t == null) return;
        RectTransform rect = t.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    // ------------------------------------------------------------------ UI pieces

    static Toggle MakeToggle(string name, Transform parent, Vector2 position)
    {
        GameObject go = MakeImage(name, parent, "button_grey", Center, position, new Vector2(72, 72));
        Toggle toggle = go.AddComponent<Toggle>();
        GameObject check = MakeImage("On", go.transform, "button_green", Center, new Vector2(0, 2), new Vector2(50, 50));
        toggle.targetGraphic = go.GetComponent<Image>();
        toggle.graphic = check.GetComponent<Image>();  // the green square = ON
        toggle.isOn = true;
        return toggle;
    }

    static Slider MakeSlider(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        Place(go, parent, Center, position, size);
        Slider slider = go.AddComponent<Slider>();

        GameObject background = MakeImage("Background", go.transform, "pill", Center, Vector2.zero, Vector2.zero);
        Stretch(background, 0, 0);
        background.GetComponent<Image>().color = Track;

        GameObject fillArea = new GameObject("FillArea", typeof(RectTransform));
        fillArea.transform.SetParent(go.transform, false);
        Stretch(fillArea, 6, 6);
        GameObject fill = MakeImage("Fill", fillArea.transform, "pill", Center, Vector2.zero, Vector2.zero);
        fill.GetComponent<Image>().color = Yellow;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handleArea = new GameObject("HandleArea", typeof(RectTransform));
        handleArea.transform.SetParent(go.transform, false);
        Stretch(handleArea, 20, 0);
        GameObject handle = MakeImage("Handle", handleArea.transform, "pill", Center, Vector2.zero, new Vector2(44, 12));
        handle.GetComponent<Image>().color = Color.white;

        slider.fillRect = fillRect;
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
        return slider;
    }

    static GameObject FullScreenDim(string name, Transform canvas)
    {
        GameObject go = MakeImage(name, canvas, null, Center, Vector2.zero, Vector2.zero);
        Stretch(go, 0, 0);
        go.GetComponent<Image>().color = Dim; // also blocks clicks on the game behind
        return go;
    }

    // ------------------------------------------------------------------ helpers (same as the Race UI builder)

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
        System.Type inputModule = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputModule != null) es.AddComponent(inputModule);
        else es.AddComponent<StandaloneInputModule>();
    }

    static void Stretch(GameObject go, float marginX, float marginY)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(marginX, marginY);
        rect.offsetMax = new Vector2(-marginX, -marginY);
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
            if (sprite == null) Debug.LogWarning("Pause/Settings: sprite not found: " + SpriteFolder + spriteName + ".png");
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

    static Button MakeButton(string name, Transform parent, string spriteName, string label, Vector2 position, Vector2 size, float fontSize)
    {
        return MakeButton(name, parent, spriteName, label, position, size, fontSize, Center);
    }

    static Button MakeButton(string name, Transform parent, string spriteName, string label, Vector2 position, Vector2 size, float fontSize, Vector2 anchor)
    {
        GameObject go = MakeImage(name, parent, spriteName, anchor, position, size);
        Button button = go.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.94f, 0.94f, 0.94f);
        colors.pressedColor = new Color(0.78f, 0.78f, 0.78f);
        button.colors = colors;
        MakeText("Text", go.transform, label, fontSize, Color.white, Center, new Vector2(0, 5), size - new Vector2(20, 10));
        return button;
    }
}
#endif
