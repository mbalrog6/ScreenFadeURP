#if UNITY_EDITOR

// This script was heavily influenced from the WarpedImagination Video: https://www.youtube.com/watch?v=p5DCv7loLbw
// This script was heavily influenced from the Bardent Video: https://www.youtube.com/watch?v=Ubuu4ZoTb5Y

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


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
        private bool _showFeatureButtons;
        private string _debugWarning;
        private enum Fade {In, Out}

        private int _defaultFontSize;
        private Color _bgColor;
        private Color NeutralColorButton;
        private Color ShouldPressColorButton;
        private Color ErrorColorButton;
        private Color ErrorLabelBackgroundColor;

        private AssetUtility util;
        private ScreenFadeFeatureList _screenFadeFeatureList;

        public void OnEnable()
        {
            _screenFade = (ScreenFade)target;

            ColorUtility.TryParseHtmlString("#537C96", out NeutralColorButton);
            ColorUtility.TryParseHtmlString("#117D52", out ShouldPressColorButton);
            ColorUtility.TryParseHtmlString("#A1180E", out ErrorColorButton);
            ColorUtility.TryParseHtmlString("#FF2F1C", out ErrorLabelBackgroundColor);
            
            util = new AssetUtility();
            _screenFadeFeatureList = new ScreenFadeFeatureList();
        }

        public override void OnInspectorGUI()
        {
            _defaultFontSize = GUI.skin.button.fontSize;
            
            if (!_screenFade.IsRendererFeatureSet && _showFeatureButtons == false)
            {
                _bgColor = GUI.backgroundColor;
                GUI.backgroundColor = ShouldPressColorButton;
                GUI.skin.button.fontSize = 17;
                
                // Display the Fix Now button if the ScreenFadeFeature Dependency in the ScreenFade Script is no set
                if (GUILayout.Button("Fix Now!", GUILayout.MinHeight(40f)))
                {
                    // make sure Renderers have all SubAssets Imported/Saved and are available. 
                    util.RefreshURPRendererAssets();
                    // retrieve all the relavent data about the ScreenFadeFeatures and populate the list.
                    util.FindScreenFadeFeatures(out _screenFadeFeatureList);

                    // No ScreenFadeFeature was found on any URP Renderer
                    if (_screenFadeFeatureList.Count <= 0)
                    {
                        _showRenderingFeatureMessage = true;
                        _debugWarning = "No ScreenFadeRendererFeature was found. " +
                                        "Make sure the ScreenFade Rendering Feature is added to your URP Renderer. " +
                                        "Cannot auto-fix this until you add the Renderer Feature.";
                        Debug.LogWarning(_debugWarning);
                    }

                    // ScreenFadeFeature were found but none of them were on the currently set Default Renderer
                    if (_screenFadeFeatureList.DefaultFeature == null && _screenFadeFeatureList.Count > 0)
                    {
                        _debugWarning =
                            "ScreenFadeFeatures were found, but none of them were on the Default Renderer. " +
                            "There is a good chance if you Test the Fade Nothing will happen, unless you are " +
                            "changing the Renderer in script, or have the camera set to a renderer that is not the " +
                            "GraphicsSetting's Default";
                        Debug.LogWarning(_debugWarning);
                        _showRenderingFeatureMessage = true;
                    }

                    // The only options is on the current Default Renderer and it is a URP Renderer So set it.
                    if (_screenFadeFeatureList.Count == 1 && _screenFadeFeatureList.DefaultFeature != null)
                    {
                        _screenFade.SetScreenFadeRendererFeature(_screenFadeFeatureList.DefaultFeature.Feature);
                        _showRenderingFeatureMessage = false;
                    }

                    // ScreenFadeFeatures were found.
                    if (_screenFadeFeatureList.Count > 0)
                    {
                        _showFeatureButtons = true;
                    }
                }
                GUI.backgroundColor = _bgColor;
                GUI.skin.button.fontSize = _defaultFontSize;
            }
            
            if (_showFeatureButtons)
            {
                _bgColor = GUI.backgroundColor;
                
                GUILayout.Label("Select Renderer For this ScreenFade Script");
                
                GUI.backgroundColor = ShouldPressColorButton;
                GUI.skin.button.fontSize = 17;
                
                if (_screenFadeFeatureList.DefaultFeature != null)
                {
                    if (GUILayout.Button(_screenFadeFeatureList.DefaultFeature.Renderer, GUILayout.MinHeight(40f)))
                    {
                        _screenFade.SetScreenFadeRendererFeature(_screenFadeFeatureList.DefaultFeature.Feature);
                        _showRenderingFeatureMessage = false;
                        _showFeatureButtons = false;
                    }
                }
                
                GUI.backgroundColor = NeutralColorButton;
                    
                foreach (var screenFade in _screenFadeFeatureList)
                {
                    if (screenFade == _screenFadeFeatureList.DefaultFeature) continue;
                        
                    if (GUILayout.Button(screenFade.Renderer))
                    {
                        _screenFade.SetScreenFadeRendererFeature(screenFade.Feature);
                        _showRenderingFeatureMessage = false;
                        _showFeatureButtons = false;
                    }
                }
                
                GUI.backgroundColor = ErrorColorButton;
                
                if (GUILayout.Button("Cancel"))
                {
                    _showRenderingFeatureMessage = false;
                    _showFeatureButtons = false;
                }

                GUI.backgroundColor = _bgColor;
            }
            
            if (_showRenderingFeatureMessage)
            {
                _bgColor = GUI.backgroundColor;
                GUI.backgroundColor = ErrorLabelBackgroundColor;
                GUILayout.TextArea(_debugWarning);
                GUI.backgroundColor = _bgColor;
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