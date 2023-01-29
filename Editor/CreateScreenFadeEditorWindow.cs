#if UNITY_EDITOR

using MB6.URP.Fade;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CreateScreenFadeEditorWindow : EditorWindow
{
    private const string _menuName = "Assets/Create/MB6/Create URPScreenFade";
    private AssetUtility _util;
    private RendererElementList _rendererElementList;
    
    private Color NeutralColorButton;
    private Color ErrorColorButton;
    private Color _bgColor;
    
    [MenuItem(_menuName)]
    static void CreateWindow()
    {
        CreateScreenFadeEditorWindow window = ScriptableObject.CreateInstance<CreateScreenFadeEditorWindow>();
        window.position = new Rect(300f, 300f, 500f, 500f);
        window.ShowModalUtility();
    }

    private void OnGUI()
    {
        if (_util == null)
        {
            ColorUtility.TryParseHtmlString("#537C96", out NeutralColorButton);
            ColorUtility.TryParseHtmlString("#A1180E", out ErrorColorButton);
            
            _util = new AssetUtility();
            _rendererElementList = new RendererElementList();
            
            _util.RefreshURPRendererAssets();
            _util.FindURPRendererPaths(out _rendererElementList);
        }
        
        GUILayout.Space(30f);

        _bgColor = GUI.backgroundColor;
        GUI.backgroundColor = NeutralColorButton;
        var defaultFontSize = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = 17;
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Please Select Renderer to Add Feature To:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.skin.label.fontSize = defaultFontSize;
        
        GUILayout.Space(20f);
        
        defaultFontSize = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 17;

        if (_rendererElementList.Count > 0)
        {
            foreach (var renderer in _rendererElementList)
            {
                if (GUILayout.Button(renderer.Name, GUILayout.MinHeight(40f)))
                {
                    CreateFeature(renderer.Path);
                    Close();
                }
            }
        }
        else
        {
            GUILayout.Label("There are no URP Renderers without ScreenFadeFeatures.\n\r Please check your URP Renderers");
        }

        GUI.skin.button.fontSize = defaultFontSize;
        GUI.backgroundColor = ErrorColorButton;
        if (GUILayout.Button("Cancel", GUILayout.MinHeight(30f)))
        {
            Close();
        }

        GUI.backgroundColor = _bgColor;
    }

    private void CreateFeature(string assetPath)
    {
        UniversalRendererData rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(assetPath);

        // collect material
        string[] materialGuids = AssetDatabase.FindAssets("ScreenFade t:Material");
        if (materialGuids == null || materialGuids.Length == 0)
        {
            Debug.LogError("No Screen Fade material found");
            return;
        }

        Material material = GetMaterial();
            
        // create screen fade feature
        ScreenFadeFeature screenFadeFeature = ScriptableObject.CreateInstance<ScreenFadeFeature>();
        screenFadeFeature.name = "ScreenFadeFeature";

        FadeSettings fadeSettings = new FadeSettings();
        fadeSettings.Material = material;
        screenFadeFeature.Settings = fadeSettings;

        // add the feature
        AssetDatabase.AddObjectToAsset(screenFadeFeature, rendererData);
        rendererData.rendererFeatures.Add(screenFadeFeature);

        // select the forward renderer
        Selection.activeObject = rendererData;

        // refresh
        EditorUtility.SetDirty(rendererData);
        AssetDatabase.Refresh();

        // save so its available in the main camera
        AssetDatabase.SaveAssets();
    }

    public Material GetMaterial()
    {
        string[] path;
        if (AssetDatabase.IsValidFolder("Packages/com.mb6.screenfadeurp/Runtime/"))
        {
            path = new string[] { "Assets/", "Packages/com.mb6.screenfadeurp/Runtime/" };
        }
        else
        {
            path = new string[] { "Assets/" };
        }
        var results = AssetDatabase.FindAssets("ScreenFadeURP t:Material", path);
        if (results.Length > 0)
        {
            return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(Material)) as Material;
        }

        return null;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

#endif
