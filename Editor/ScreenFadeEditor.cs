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
        private enum Fade {In, Out}

        public void OnEnable()
        {
            _screenFade = (ScreenFade)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10f);

            if (Application.isPlaying)
            {
                GUILayout.Space(10f);

                if (GUILayout.Button("Test Fade"))
                {
                    _screenFade.Fade(!_screenFade.IsFadedIn);
                }

                return;
            }

            if (_screenFade.HasFadeInEase())
            {
                if (GUILayout.Button("Remove FadeIn Ease Function"))
                {
                    _screenFade.SetFadeInEase(null);
                }
            }
            else
            {
                if (!_showFadeInEaseFunctions)
                {
                    if (GUILayout.Button("Add FadeIn Ease Function"))
                    {
                        _showFadeInEaseFunctions = true;
                        _showEaseInFoldOut = true;
                    }
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
                if (GUILayout.Button("Remove FadeOut Ease Function"))
                {
                    _screenFade.SetFadeOutEase(null);
                }
            }
            else
            {
                if (!_showFoldOutEaseFunctions)
                {
                    if (GUILayout.Button("Add FadeOut Ease Function"))
                    {
                        _showFoldOutEaseFunctions = true;
                        _showEaseOutFoldOut = true;
                    }
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
            if (GUILayout.Button("Cancel"))
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
        }

        private void ShowEaseFunctionsForFadeIn()
        {
            foreach (var easeFunction in _easeFunctions)
            {
                if (GUILayout.Button(easeFunction.Name))
                {
                    var obj = Activator.CreateInstance(easeFunction) as IEaseFunction;
                    _screenFade.SetFadeInEase(obj);
                    _showFadeInEaseFunctions = false;
                }
            }
        }

        private void ShowEaseFunctionsForFadeOut()
        {
            foreach (var easeFunction in _easeFunctions)
            {
                if (GUILayout.Button(easeFunction.Name))
                {
                    var obj = Activator.CreateInstance(easeFunction) as IEaseFunction;
                    _screenFade.SetFadeOutEase(obj);
                    _showFoldOutEaseFunctions = false;
                }
            }
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