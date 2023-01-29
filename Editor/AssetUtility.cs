#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MB6.URP.Fade
{
    public class AssetUtility
    {
        private const string filterString = "t:UniversalRenderPipelineAsset";
        private const string filterStringURPData = "t:UniversalRendererData";
        private const string featureName = "ScreenFadeFeature";

        public string DefaultRendererName { get; private set; }
        public bool DefaultRendererIsURP { get; private set; }

        public AssetUtility()
        {
            var defaultRenderer = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset);
            DefaultRendererIsURP = false;

            if (defaultRenderer != null)
            {
                DefaultRendererName = defaultRenderer.name;
                DefaultRendererIsURP = true;
            }
        }

        public void FindScreenFadeFeatures(out ScreenFadeFeatureList featureList)
        {
            var listOfRenderers = AssetDatabase.FindAssets(filterString);
            var constructedFeatureList = new ScreenFadeFeatureList();

            foreach (var rendererGuid in listOfRenderers)
            {
                var rendererPath = AssetDatabase.GUIDToAssetPath(rendererGuid);
                var urpRendererAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(rendererPath);
                var urpRenderer = urpRendererAsset.GetRenderer(0) as UniversalRenderer;

                var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                List<ScriptableRendererFeature> features =
                    property.GetValue(urpRenderer) as List<ScriptableRendererFeature>;

                foreach (var feature in features)
                {
                    if (String.Compare(feature.name, featureName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (DefaultRendererIsURP)
                        {
                            if (String.Compare(urpRendererAsset.name, DefaultRendererName,
                                    StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                constructedFeatureList.AddFeature(new FeatureListElement(
                                        urpRendererAsset.name,
                                        feature as ScreenFadeFeature,
                                        true
                                    )
                                );
                                continue;
                            }
                        }

                        constructedFeatureList.AddFeature(new FeatureListElement(
                                urpRendererAsset.name,
                                feature as ScreenFadeFeature,
                                false
                            )
                        );
                    }
                }
            }

            featureList = constructedFeatureList;
        }

        public void FindURPRendererPaths(out RendererElementList rendererElementList)
        {
            var constructedRendererElementList = new RendererElementList();

            var listOfRendererGUIDs = AssetDatabase.FindAssets(filterStringURPData, new string[] {"Assets/"});
            if (listOfRendererGUIDs == null || listOfRendererGUIDs.Length == 0)
            {
                Debug.LogWarning("No URP Renderers Found");
                rendererElementList = constructedRendererElementList;
                return;
            }

            foreach (var rendererGuid in listOfRendererGUIDs)
            {
                var rendererPath = AssetDatabase.GUIDToAssetPath(rendererGuid);
                var urpRendererAsset = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
                var rendererPathName = Path.GetFileNameWithoutExtension(rendererPath);
                bool shouldSkip = false;

                foreach (var feature in urpRendererAsset.rendererFeatures)
                {
                    if (feature is ScreenFadeFeature)
                    {
                        shouldSkip = true;
                    }
                }

                if (shouldSkip) continue;

                constructedRendererElementList.AddRendererElement(new RendererListElement(rendererPathName, rendererPath));
            }

            if (constructedRendererElementList.Count <= 0)
            {
                Debug.LogWarning("All URP Renderers already had the ScreenFadeFeature Added");
            }
            
            rendererElementList = constructedRendererElementList;
        }
        
        public void RefreshURPRendererAssets()
        {
            var listOfRenderers = AssetDatabase.FindAssets(filterString);
            foreach (var rendererAssetGUID in listOfRenderers)
            {
                AssetDatabase.SaveAssetIfDirty(AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(AssetDatabase.GUIDToAssetPath(rendererAssetGUID)));
            }
        }
    }
}

#endif