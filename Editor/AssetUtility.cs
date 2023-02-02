#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = System.Object;

namespace MB6.URP.Fade
{
    public class AssetUtility
    {
        private const string filterString = "t:UniversalRenderPipelineAsset";

        public string DefaultPipelineName { get; private set; }
        public bool DefaultPipelineIsURP { get; private set; }

        public AssetUtility()
        {
            var defaultPipeline = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset);
            DefaultPipelineName = "NONE";
            DefaultPipelineIsURP = false;

            if (defaultPipeline != null)
            {
                DefaultPipelineName = defaultPipeline.name;
                DefaultPipelineIsURP = true;
            }
        }

        public void FindScreenFadeFeatures(out RendererDetailsList detailsList)
        {
            var pipelineAssets = GetRendererPipelineAssets<UniversalRenderPipelineAsset>();
            var constructedRendererDetailsList = new RendererDetailsList();

            foreach (var pipeline in pipelineAssets)
            {
                var IsDefaultPipeline = pipeline.name == DefaultPipelineName;
                var list = GetRendererDataList<UniversalRenderPipelineAsset, ScriptableRendererData[]>(pipeline) as ScriptableRendererData[];

                if (list != null)
                {
                    int i = 0;
                    foreach (var rendererData in list)
                    {
                        var index = GetDefaultRendererIndex<UniversalRenderPipelineAsset>(pipeline);

                        if(index.HasValue == false) continue;
                        
                        foreach (var feature in rendererData.rendererFeatures)
                        {
                            if (feature is ScreenFadeFeature)
                            {
                                var newDetail = new RendererDetails(
                                    pipeline.name,
                                    rendererData.name,
                                    feature as ScreenFadeFeature,
                                    i == index.Value,
                                    IsDefaultPipeline,
                                    index.Value
                                );
                                constructedRendererDetailsList.Add(newDetail);
                            }
                        }
                        i++;
                    }
                }
            }

            detailsList = constructedRendererDetailsList;
        }

        public void FindURPRendererPaths(out RendererDetailsList detailsList)
        {
            var pipelineAssets = GetRendererPipelineAssets<UniversalRenderPipelineAsset>();
            var constructedRendererDetailsList = new RendererDetailsList();
            
            if (pipelineAssets == null)
            {
                Debug.LogWarning("No URP Renderers Found");
                detailsList = constructedRendererDetailsList;
                return;
            }

            foreach (var pipeline in pipelineAssets)
            {
                // Set variable to true if current pipeline is the GraphicSettings Default Pipeline
                var IsDefaultPipeline = pipeline.name == DefaultPipelineName;
                // Use Reflection to get all the ScriptableRendererData (Renderers) in current pipeline.
                var list = GetRendererDataList<UniversalRenderPipelineAsset, ScriptableRendererData[]>(pipeline) as ScriptableRendererData[];
                bool shouldSkip = false;
                
                // Loop through each Renderer in current pipeline
                int i = 0;
                foreach (var rendererData in list)
                {
                    // Use Reflection to get the index for the Default Renderer in the RendererList for the current pipeline
                    var index = GetDefaultRendererIndex<UniversalRenderPipelineAsset>(pipeline);

                    // Check to make sure Renderer does not already have a ScreenFadeFeature
                    foreach (var feature in rendererData.rendererFeatures)
                    {
                        if (feature is ScreenFadeFeature)
                        {
                            shouldSkip = true;
                            break;
                        }
                    }

                    // if ScreenFadeFeature was found on this Renderer skip it.
                    if (shouldSkip)
                    {
                        shouldSkip = false;
                        i++;
                        continue;
                    }
                    
                    
                    if (index.HasValue)
                    {
                        var newDetail = new RendererDetails(
                            pipeline.name,
                            rendererData.name,
                            null,
                            i == index.Value,
                            IsDefaultPipeline,
                            index.Value
                        );
                        constructedRendererDetailsList.Add(newDetail);
                    }
                    i++;
                }
            }

            if (constructedRendererDetailsList.Count <= 0)
            {
                Debug.LogWarning("All URP Renderers already had the ScreenFadeFeature Added");
            }
            
            detailsList = constructedRendererDetailsList;
        }
        
        public void RefreshURPRendererAssets()
        {
            var listOfRenderers = AssetDatabase.FindAssets(filterString);
            foreach (var rendererAssetGUID in listOfRenderers)
            {
                AssetDatabase.SaveAssetIfDirty(AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(AssetDatabase.GUIDToAssetPath(rendererAssetGUID)));
            }
        }

        public List<T> GetRendererPipelineAssets<T>() where T : RenderPipelineAsset
        {
            var filter = "t:" + typeof(T).Name;
            string[] folders = { "Assets/" };
            var assetGuids = AssetDatabase.FindAssets(filter, folders);

            List<T> assetList = new List<T>();
            if (assetGuids.Length > 0)
            {
                foreach (var guid in assetGuids)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                    assetList.Add(asset);
                }
                return assetList;
            }

            return null;
        }

        #region Object GetRendererDataList
        // T = RenderPipelineAsset (used for the Reflection)
        // T2 = The return type for the field being matched against.
        // string fieldToMatch = The name of the field in T that is being querried for. 
        // Return Type: System.Object is used to return null or T2 (must cast to T2 to be useful by caller)
        public Object GetRendererDataList<T, T2>(T renderPipelineAsset, string fieldToMatch = "m_RendererDataList") where T : RenderPipelineAsset
        {
            var field = typeof(T).GetField(fieldToMatch,         
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                var rl = (T2)field.GetValue(renderPipelineAsset);
                return rl;
            }

            return null;
        }
        #endregion

        public int? GetDefaultRendererIndex<T>(T pipelineAsset, string filter = "m_DefaultRendererIndex") where T : RenderPipelineAsset
        {
            int? index = null;
            var field = typeof(T).GetField(filter, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                index = (int)field.GetValue(pipelineAsset);
            }

            return index;
        }
    }
}

#endif