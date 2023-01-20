#if UNITY_EDITOR

// This script was heavily influenced from the WarpedImagination Video: https://www.youtube.com/watch?v=p5DCv7loLbw
// This script was heavily influenced from the Bardent Video: https://www.youtube.com/watch?v=Ubuu4ZoTb5Y

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MB6.URP.Fade
{
    [CustomEditor(typeof(ScreenFade))]
    public class ScreenFadeEditor : Editor
    {
        private static List<Type> _easeFunctions = new List<Type>();
        private ScreenFade _screenFade;
        private bool _showFadeInEaseFunctions;
        private bool _showFoldOutEaseFunctions;
        private bool _showEaseInFoldOut;
        private bool _showEaseOutFoldOut;
        private bool _showRenderingFeatureMessage;
        private enum Fade {In, Out}

        private int _defaultFontSize;
        private int _buttonFontSize = 25;
        private Color _bgColor;
        private Color NeutralColorButton;
        private Color ShouldPressColorButton;
        private Color ErrorColorButton;
        private Color ErrorLabelBackgroundColor;

        public void OnEnable()
        {
            _screenFade = (ScreenFade)target;

            ColorUtility.TryParseHtmlString("#537C96", out NeutralColorButton);
            ColorUtility.TryParseHtmlString("#117D52", out ShouldPressColorButton);
            ColorUtility.TryParseHtmlString("#A1180E", out ErrorColorButton);
            ColorUtility.TryParseHtmlString("#FF2F1C", out ErrorLabelBackgroundColor);
        }

        public override void OnInspectorGUI()
        {
            _defaultFontSize = GUI.skin.button.fontSize;

            if (!_screenFade.IsRendererFeatureSet)
            {
                _bgColor = GUI.backgroundColor;
                GUI.backgroundColor = ShouldPressColorButton;
                GUI.skin.button.fontSize = 17;
                if (GUILayout.Button("Fix Now!", GUILayout.MinHeight(40f)))
                {
                    var result = GetScreenFadeFeature();
                    if (result == null)
                    {
                        Debug.LogWarning("No ScreenFadeRendererFeature was found. " +
                                         "Make sure the ScreenFade Rendering Feature is added to your URP Renderer." +
                                         "Cannot auto-fix this until you add the Renderer Feature.");
                        _showRenderingFeatureMessage = true;
                    }
                    else
                    {
                        _screenFade.SetScreenFadeRendererFeature(result);
                        _showRenderingFeatureMessage = false;
                    }
                }
                GUI.backgroundColor = _bgColor;
                GUI.skin.button.fontSize = _defaultFontSize;

                if (_showRenderingFeatureMessage)
                {
                    _bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = ErrorLabelBackgroundColor;
                    GUILayout.TextArea("- No ScreenFadeFeature was found.\n\n" +
                                       "- Make sure the ScreenFade Rendering Feature is added to your URP Renderer.\n\n" +
                                       "- Cannot auto-fix this until you add the Renderer Feature.");
                    GUI.backgroundColor = _bgColor;
                }
            }
            
            base.OnInspectorGUI();
            GUILayout.Space(10f);

            if (Application.isPlaying)
            {
                GUILayout.Space(10f);

                _bgColor = GUI.color;
                GUI.color = NeutralColorButton;
                GUI.skin.button.fontSize = 30;
                if (GUILayout.Button("Test Fade", GUILayout.MinHeight(50f)))
                {
                    _screenFade.Fade(!_screenFade.IsFadedIn);
                }
                GUI.skin.button.fontSize = _defaultFontSize;
                GUI.color = _bgColor;

                return;
            }

            if (_screenFade.HasFadeInEase())
            {
                _bgColor = GUI.backgroundColor;
                GUI.backgroundColor = NeutralColorButton;
                GUI.skin.button.fontSize = 17;
                if (GUILayout.Button("Remove FadeIn Ease Function", GUILayout.MinHeight(40f)))
                {
                    _screenFade.SetFadeInEase(null);
                }
                GUI.backgroundColor = _bgColor;
                GUI.skin.button.fontSize = _defaultFontSize;
            }
            else
            {
                if (!_showFadeInEaseFunctions)
                {
                    _bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = ShouldPressColorButton;
                    GUI.skin.button.fontSize = 17;
                    if (GUILayout.Button("Add FadeIn Ease Function", GUILayout.MinHeight(40f)))
                    {
                        _showFadeInEaseFunctions = true;
                        _showEaseInFoldOut = true;
                    }
                    GUI.backgroundColor = _bgColor;
                    GUI.skin.button.fontSize = _defaultFontSize;
                }
                else
                {
                    _showEaseInFoldOut = EditorGUILayout.Foldout(_showEaseInFoldOut, "Ease Functions (FadeIn):");
                    if (_showEaseInFoldOut)
                    {
                        ShowEaseFunctionsForFadeIn();
                        ShowCancelButton(Fade.In);
                        GUILayout.Space(10f);
                    }
                }
            }


            if (_screenFade.HasFadeOutEase())
            {
                _bgColor = GUI.backgroundColor;
                GUI.backgroundColor = NeutralColorButton;
                GUI.skin.button.fontSize = 17;
                if (GUILayout.Button("Remove FadeOut Ease Function", GUILayout.MinHeight(40f)))
                {
                    _screenFade.SetFadeOutEase(null);
                }
                GUI.backgroundColor = _bgColor;
                GUI.skin.button.fontSize = _defaultFontSize;
            }
            else
            {
                if (!_showFoldOutEaseFunctions)
                {
                    _bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = ShouldPressColorButton;
                    GUI.skin.button.fontSize = 17;
                    if (GUILayout.Button("Add FadeOut Ease Function", GUILayout.MinHeight(40f)))
                    {
                        _showFoldOutEaseFunctions = true;
                        _showEaseOutFoldOut = true;
                    }
                    GUI.backgroundColor = _bgColor;
                    GUI.skin.button.fontSize = _defaultFontSize;
                }
                else
                {
                    _showEaseOutFoldOut = EditorGUILayout.Foldout(_showEaseOutFoldOut, "Ease Functions (FadeOut):");
                    if (_showEaseOutFoldOut)
                    {
                        ShowEaseFunctionsForFadeOut();
                        ShowCancelButton(Fade.Out);
                        GUILayout.Space(10f);
                    }
                }
            }
        }

        private void ShowCancelButton(Fade fadeIn)
        {
            GUI.skin.button.fontSize = 15;
            _bgColor = GUI.backgroundColor;
            GUI.backgroundColor = ErrorColorButton;
            
            if (GUILayout.Button("Cancel", GUILayout.MinHeight(25f)))
            {
                if (fadeIn == Fade.In)
                {
                    _showFadeInEaseFunctions = false;
                }
                else
                {
                    _showFoldOutEaseFunctions = false;
                }
            }

            GUI.backgroundColor = _bgColor;
            GUI.skin.button.fontSize = _defaultFontSize;
        }

        private void ShowEaseFunctionsForFadeIn()
        {
            GUI.skin.button.fontSize = 15;
            foreach (var easeFunction in _easeFunctions)
            {
                if (GUILayout.Button(easeFunction.Name))
                {
                    var obj = Activator.CreateInstance(easeFunction) as IEaseFunction;
                    _screenFade.SetFadeInEase(obj);
                    _showFadeInEaseFunctions = false;
                }
            }
            GUI.skin.button.fontSize = _defaultFontSize;
        }

        private void ShowEaseFunctionsForFadeOut()
        {
            GUI.skin.button.fontSize = 15;
            foreach (var easeFunction in _easeFunctions)
            {
                if (GUILayout.Button(easeFunction.Name))
                {
                    var obj = Activator.CreateInstance(easeFunction) as IEaseFunction;
                    _screenFade.SetFadeOutEase(obj);
                    _showFoldOutEaseFunctions = false;
                }
            }
            GUI.skin.button.fontSize = _defaultFontSize;
        }

        public ScreenFadeFeature GetScreenFadeFeature()
        {
            AssetDatabase.Refresh();
            var resultingGuid = AssetDatabase.FindAssets("ScreenFadeFeature t:ScriptableRendererFeature",
                new string[] { "Assets/" });
            if (resultingGuid.Length > 0)
            {
                var rendererFeature = AssetDatabase.LoadAssetAtPath<ScriptableRendererFeature>(
                    AssetDatabase.GUIDToAssetPath(resultingGuid[0])) as ScreenFadeFeature;
                return rendererFeature;
            }
            
            return null;
        }

        [DidReloadScripts]
        public static void OnRecompile()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(assembly => assembly.GetTypes());
            var filteredTypes = types.Where(
                type => typeof(IEaseFunction).IsAssignableFrom(type) && type.IsClass
            );

            _easeFunctions = filteredTypes.ToList();
        }
    }
}

#endif