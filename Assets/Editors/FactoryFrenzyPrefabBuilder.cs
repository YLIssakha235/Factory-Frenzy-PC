// Factory Frenzy – prefab builder
// Put this file in an "Editor" folder of your Unity project (e.g. Assets/Editor/FactoryFrenzyPrefabBuilder.cs).
// Then use the menu:  Factory Frenzy > Build Prefabs
// It creates Assets/FactoryFrenzyGen/{Materials, Meshes, Prefabs} with every platform and trap.
// Works with the Built-in pipeline and URP (Unity 2022.3).

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class FactoryFrenzyPrefabBuilder
{
    const string Root = "Assets/FactoryFrenzyGen";
    const string MatDir = Root + "/Materials";
    const string MeshDir = Root + "/Meshes";
    const string PrefabDir = Root + "/Prefabs";

    static bool IsURP => GraphicsSettings.currentRenderPipeline != null;

    // ------------------------------------------------------------------ menu

    [MenuItem("Factory Frenzy/Build Prefabs")]
    public static void BuildAll()
    {
        EnsureFolders();

        BuildStart();
        BuildFinish();
        BuildClassic();
        BuildMoving();
        BuildTrampoline();
        BuildCheckpoint();
        BuildBumper();
        BuildLauncher();
        BuildProjectile();
        BuildFan();
        BuildKillZone();
        BuildPlayerBean();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Factory Frenzy",
            "Prefabs created in " + PrefabDir + ".\n\nUse 'Factory Frenzy > Place All In Scene' to see them side by side.", "OK");
    }

    [MenuItem("Factory Frenzy/Place All In Scene")]
    public static void PlaceAll()
    {
        string[] names = { "Start", "Classic", "Moving", "Trampoline", "Checkpoint", "Bumper", "Launcher", "Fan", "Finish", "PlayerBean" };
        var parent = new GameObject("FactoryFrenzy_Showcase");
        Undo.RegisterCreatedObjectUndo(parent, "Place Factory Frenzy prefabs");
        float x = 0f;
        foreach (var n in names)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/{n}.prefab");
            if (prefab == null) { Debug.LogWarning($"Prefab {n} not found. Run 'Factory Frenzy > Build Prefabs' first."); continue; }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            go.transform.localPosition = new Vector3(x, 0f, 0f);
            x += 9f;
        }
        Selection.activeGameObject = parent;
    }

    // -------------------------------------------------------------- platforms

    // PC-PLA-P2 – green floor, white start line, a spawn point behind the line
    static void BuildStart()
    {
        var root = new GameObject("Start");
        Box("Platform", root.transform, Vector3.zero, new Vector3(6f, 0.6f, 5f), Mat("FF_Start", "#3DBE6B"));
        Box("StartLine", root.transform, new Vector3(0f, 0.32f, 1.6f), new Vector3(6f, 0.04f, 0.35f), Mat("FF_White", "#FFFFFF"), false);
        Empty("SpawnPoint", root.transform, new Vector3(0f, 0.3f, -1.2f));
        Save(root);
    }

    // PC-PLA-P3 – blue floor, checkered line, blue arch, finish trigger on the line
    static void BuildFinish()
    {
        var root = new GameObject("Finish");
        Box("Platform", root.transform, Vector3.zero, new Vector3(6f, 0.6f, 5f), Mat("FF_Finish", "#3D7BE0"));

        var checkers = Empty("Checkers", root.transform, new Vector3(0f, 0.32f, -1f));
        var white = Mat("FF_White", "#FFFFFF");
        for (int i = 0; i < 12; i++)
            for (int j = 0; j < 2; j++)
            {
                if ((i + j) % 2 == 1) continue;
                Box($"Checker_{i}_{j}", checkers, new Vector3(-2.75f + i * 0.5f, 0f, -0.125f + j * 0.25f), new Vector3(0.5f, 0.04f, 0.25f), white, false);
            }

        var arch = Empty("Arch", root.transform, new Vector3(0f, 0f, -1f));
        var archMat = Mat("FF_FinishArch", "#3D7BE0", emissionHex: "#10285A");
        Box("PostL", arch, new Vector3(-2.8f, 1.8f, 0f), new Vector3(0.3f, 3f, 0.3f), archMat);
        Box("PostR", arch, new Vector3(2.8f, 1.8f, 0f), new Vector3(0.3f, 3f, 0.3f), archMat);
        Box("Bar", arch, new Vector3(0f, 3.3f, 0f), new Vector3(5.9f, 0.4f, 0.3f), archMat);

        Trigger("FinishLine", root.transform, new Vector3(0f, 1.8f, -1f), new Vector3(5.4f, 3f, 0.5f));
        Save(root);
    }

    // PC-PLA-P4 – plain grey floor
    static void BuildClassic()
    {
        var root = new GameObject("Classic");
        Box("Platform", root.transform, Vector3.zero, new Vector3(4f, 0.6f, 4f), Mat("FF_Classic", "#8E97AA"));
        Save(root);
    }

    // PC-PLA-P5 – purple floor (movement script goes on the root)
    static void BuildMoving()
    {
        var root = new GameObject("Moving");
        Box("Platform", root.transform, Vector3.zero, new Vector3(3.5f, 0.5f, 3.5f), Mat("FF_Moving", "#A99BD6"));
        Save(root);
    }

    // PC-PLA-P6 – dark base + pink pad + bounce trigger
    static void BuildTrampoline()
    {
        var root = new GameObject("Trampoline");
        Box("Base", root.transform, Vector3.zero, new Vector3(3f, 0.3f, 3f), Mat("FF_TrampolineBase", "#4A5163"));
        Cyl("Pad", root.transform, new Vector3(0f, 0.21f, 0f), 1.25f, 0.12f, Mat("FF_TrampolinePad", "#FF8CC6"));
        Trigger("BounceTrigger", root.transform, new Vector3(0f, 0.45f, 0f), new Vector3(2.4f, 0.4f, 2.4f));
        Save(root);
    }

    // PC-PLA-P7 – grey floor + arch (OFF material). FF_CheckpointOn is created for your script to swap in.
    static void BuildCheckpoint()
    {
        var root = new GameObject("Checkpoint");
        Box("Platform", root.transform, Vector3.zero, new Vector3(4f, 0.6f, 4f), Mat("FF_Classic", "#8E97AA"));

        var off = Mat("FF_CheckpointOff", "#5B6375");
        Mat("FF_CheckpointOn", "#3DBE6B", emissionHex: "#1F7A42", emissionIntensity: 1.5f);

        var arch = Empty("Arch", root.transform, Vector3.zero);
        Box("PostL", arch, new Vector3(-1.9f, 1.6f, 0f), new Vector3(0.3f, 2.6f, 0.3f), off);
        Box("PostR", arch, new Vector3(1.9f, 1.6f, 0f), new Vector3(0.3f, 2.6f, 0.3f), off);
        Box("Bar", arch, new Vector3(0f, 3.0f, 0f), new Vector3(4.1f, 0.3f, 0.3f), off);

        Trigger("CheckpointTrigger", root.transform, new Vector3(0f, 1.6f, 0f), new Vector3(3.5f, 2.6f, 0.6f));
        Empty("RespawnPoint", root.transform, new Vector3(0f, 0.3f, 0.8f));
        Save(root);
    }

    // ------------------------------------------------------------------ traps

    // PC-TRA-T1 – pink pinball post with a white ring. Pivot at the bottom.
    static void BuildBumper()
    {
        var root = new GameObject("Bumper");
        Cyl("Body", root.transform, new Vector3(0f, 0.55f, 0f), 0.8f, 1.1f,
            Mat("FF_Trap", "#EF5DA8", emissionHex: "#5A1036"));
        MeshObj("Ring", root.transform, Torus("FF_Torus_Bumper", 0.86f, 0.08f),
            new Vector3(0f, 0.55f, 0f), new Vector3(90f, 0f, 0f), Mat("FF_White", "#FFFFFF"));
        Save(root);
    }

    // PC-TRA-T2 – pillar + rotating Head (turret + barrel + Muzzle) + detection sphere (radius 10)
    static void BuildLauncher()
    {
        var root = new GameObject("Launcher");
        Cyl("Pillar", root.transform, new Vector3(0f, 2f, 0f), 0.5f, 4f, Mat("FF_Pillar", "#3B4256"));

        var head = Empty("Head", root.transform, new Vector3(0f, 4f, 0f)); // rotate this around Y
        Cyl("HeadBase", head, new Vector3(0f, 0.18f, 0f), 0.75f, 0.35f, Mat("FF_HeadBase", "#2B3142"));
        Box("Turret", head, new Vector3(0f, 0.72f, 0f), new Vector3(1.1f, 0.75f, 1.2f), Mat("FF_Trap", "#EF5DA8", emissionHex: "#5A1036"));
        var barrel = Cyl("Barrel", head, new Vector3(0f, 0.75f, 1.1f), 0.2f, 1.3f, Mat("FF_Barrel", "#1D2130"), false);
        barrel.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        Empty("Muzzle", head, new Vector3(0f, 0.75f, 1.85f)); // spawn projectiles here, forward = Head.forward

        var detection = new GameObject("DetectionZone");
        detection.transform.SetParent(root.transform, false);
        detection.transform.localPosition = new Vector3(0f, 4f, 0f);
        var sphere = detection.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 10f;
        Save(root);
    }

    // Launcher projectile – yellow glowing ball, trigger + kinematic rigidbody (move it from a script)
    static void BuildProjectile()
    {
        var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.name = "Projectile";
        root.transform.localScale = Vector3.one * 0.56f;
        root.GetComponent<MeshRenderer>().sharedMaterial = Mat("FF_Projectile", "#FFE066", emissionHex: "#FFB000", emissionIntensity: 2f);
        root.GetComponent<SphereCollider>().isTrigger = true;
        var rb = root.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        Save(root);
    }

    // PC-TRA-T3 – pole + pink ring + 3 white blades + wind trigger + wind particles. Blows along +Z (forward).
    static void BuildFan()
    {
        var root = new GameObject("Fan");
        Cyl("Pole", root.transform, new Vector3(0f, 0.9f, 0f), 0.2f, 1.8f, Mat("FF_Pillar", "#3B4256"));

        var hub = Empty("Hub", root.transform, new Vector3(0f, 1.8f, 0f));
        MeshObj("Ring", hub, Torus("FF_Torus_Fan", 1.05f, 0.12f), Vector3.zero, Vector3.zero, Mat("FF_Trap", "#EF5DA8", emissionHex: "#5A1036"));

        var blades = Empty("Blades", hub, Vector3.zero); // rotate this around Z
        var bladeMat = Mat("FF_Blade", "#E8ECF4");
        for (int i = 0; i < 3; i++)
        {
            var pivot = Empty($"BladePivot_{i}", blades, Vector3.zero);
            pivot.localEulerAngles = new Vector3(0f, 0f, i * 120f);
            Box("Blade", pivot, new Vector3(0f, 0.45f, 0f), new Vector3(0.3f, 0.9f, 0.08f), bladeMat, false);
        }

        Trigger("WindZone", hub, new Vector3(0f, 0f, 3.2f), new Vector3(2.2f, 2.2f, 6f));

        var wind = new GameObject("WindParticles");
        wind.transform.SetParent(hub, false);
        var ps = wind.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.9f;
        main.startSpeed = 7f;
        main.startSize = 0.08f;
        main.startColor = Hex("#CFF4FF");
        main.maxParticles = 400;
        var emission = ps.emission;
        emission.rateOverTime = 150f;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(1.8f, 1.8f, 0.1f);
        wind.GetComponent<ParticleSystemRenderer>().sharedMaterial = ParticleMat("FF_Wind", "#CFF4FF");

        Save(root);
    }

    // PC-PLA-P4 – big invisible trigger under the level. Turn on DebugView's renderer to see it.
    static void BuildKillZone()
    {
        var root = new GameObject("KillZone");
        var bc = root.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(200f, 1f, 200f);
        var view = Box("DebugView", root.transform, Vector3.zero, new Vector3(200f, 0.05f, 200f), Mat("FF_KillZone", "#E5484D"), false);
        view.GetComponent<MeshRenderer>().enabled = false;
        Save(root);
    }

    // PC-PLA-P1 – "bean" visual (no collider). Put it as a child of the Starter Assets player and hide the robot mesh.
    static void BuildPlayerBean()
    {
        var root = new GameObject("PlayerBean");
        Prim(PrimitiveType.Capsule, "Body", root.transform, new Vector3(0f, 0.85f, 0f), new Vector3(0.84f, 0.85f, 0.84f),
            Mat("FF_Player", "#FFB547", smooth: 0.55f), false);
        Prim(PrimitiveType.Sphere, "Visor", root.transform, new Vector3(0f, 1.22f, 0.3f), new Vector3(0.66f, 0.36f, 0.32f),
            Mat("FF_Visor", "#1B2233", smooth: 0.9f, metal: 0.5f), false);
        Save(root);
    }

    // ---------------------------------------------------------------- helpers

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(Root)) AssetDatabase.CreateFolder("Assets", "FactoryFrenzyGen");
        foreach (var sub in new[] { "Materials", "Meshes", "Prefabs" })
            if (!AssetDatabase.IsValidFolder($"{Root}/{sub}")) AssetDatabase.CreateFolder(Root, sub);
    }

    static void Save(GameObject root)
    {
        PrefabUtility.SaveAsPrefabAsset(root, $"{PrefabDir}/{root.name}.prefab");
        Object.DestroyImmediate(root);
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }

    static Material Mat(string name, string hex, float smooth = 0.35f, float metal = 0.1f, string emissionHex = null, float emissionIntensity = 1f)
    {
        string path = $"{MatDir}/{name}.mat";
        Shader shader = IsURP ? Shader.Find("Universal Render Pipeline/Lit") : null;
        if (shader == null) shader = Shader.Find("Standard");

        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null) { m = new Material(shader); AssetDatabase.CreateAsset(m, path); }
        else m.shader = shader;

        var c = Hex(hex);
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smooth);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);

        if (emissionHex != null)
        {
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            m.SetColor("_EmissionColor", Hex(emissionHex) * emissionIntensity);
        }
        else
        {
            m.DisableKeyword("_EMISSION");
            if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", Color.black);
        }
        EditorUtility.SetDirty(m);
        return m;
    }

    static Material ParticleMat(string name, string hex)
    {
        string path = $"{MatDir}/{name}.mat";
        Shader shader = IsURP ? Shader.Find("Universal Render Pipeline/Particles/Unlit") : Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null) { m = new Material(shader); AssetDatabase.CreateAsset(m, path); }
        else m.shader = shader;

        var c = Hex(hex);
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
        EditorUtility.SetDirty(m);
        return m;
    }

    static Transform Empty(string name, Transform parent, Vector3 localPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        return go.transform;
    }

    static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool keepCollider = true)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        if (!keepCollider) Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }

    static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 size, Material mat, bool collider = true)
        => Prim(PrimitiveType.Cube, name, parent, pos, size, mat, collider);

    // Unity's cylinder is 2 high with radius 0.5 at scale 1, so we convert radius/height to a scale.
    static GameObject Cyl(string name, Transform parent, Vector3 pos, float radius, float height, Material mat, bool collider = true)
    {
        var go = Prim(PrimitiveType.Cylinder, name, parent, pos, new Vector3(radius * 2f, height / 2f, radius * 2f), mat, false);
        if (collider)
        {
            var mc = go.AddComponent<MeshCollider>();
            mc.convex = true;
        }
        return go;
    }

    static GameObject MeshObj(string name, Transform parent, Mesh mesh, Vector3 pos, Vector3 euler, Material mat)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localEulerAngles = euler;
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        return go;
    }

    static BoxCollider Trigger(string name, Transform parent, Vector3 center, Vector3 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var bc = go.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.center = center;
        bc.size = size;
        return bc;
    }

    // Torus lying in the XY plane (its hole faces +Z). R = ring radius, r = tube radius.
    static Mesh Torus(string name, float R, float r, int segments = 48, int sides = 12)
    {
        string path = $"{MeshDir}/{name}.asset";
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        bool isNew = mesh == null;
        if (isNew) mesh = new Mesh();
        mesh.Clear();
        mesh.name = name;

        var verts = new Vector3[(segments + 1) * (sides + 1)];
        var normals = new Vector3[verts.Length];
        var uvs = new Vector2[verts.Length];
        int v = 0;
        for (int s = 0; s <= segments; s++)
        {
            float a = s / (float)segments * Mathf.PI * 2f;
            var dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f);
            for (int k = 0; k <= sides; k++)
            {
                float b = k / (float)sides * Mathf.PI * 2f;
                var n = dir * Mathf.Cos(b) + Vector3.forward * Mathf.Sin(b);
                verts[v] = dir * R + n * r;
                normals[v] = n;
                uvs[v] = new Vector2(s / (float)segments, k / (float)sides);
                v++;
            }
        }

        var tris = new int[segments * sides * 6];
        int t = 0;
        for (int s = 0; s < segments; s++)
            for (int k = 0; k < sides; k++)
            {
                int a0 = s * (sides + 1) + k;
                int a1 = a0 + sides + 1;
                tris[t++] = a0; tris[t++] = a1; tris[t++] = a0 + 1;
                tris[t++] = a0 + 1; tris[t++] = a1; tris[t++] = a1 + 1;
            }

        mesh.vertices = verts;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();

        if (isNew) AssetDatabase.CreateAsset(mesh, path);
        else EditorUtility.SetDirty(mesh);
        return mesh;
    }
}
#endif
